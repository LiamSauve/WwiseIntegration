//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
#pragma warning disable 0219, 0414

// This script deals with initialization, and frame updates of the Wwise audio engine.  
// It must be present on one Game Object at the beginning of the game to initialize the audio properly.
// It must be executed BEFORE any other monoBehaviors that use AkSoundEngine.
// For more information about Wwise initialization and termination see the Wwise SDK doc:
// Wwise SDK » Sound Engine Integration Walkthrough » Initialize the Different Modules of the Sound Engine 
// and also, check AK::SoundEngine::Init & Term.
public class AkGlobalSoundEngineInitializer : MonoBehaviour
{
    public string basePath = AkBankPath.GetBasePath();
    public string language = "English(US)";
    public int defaultPoolSize = 4096; //4 megs for the metadata pool
    public int lowerPoolSize = 2048; //2 megs for the processing pool
    public int streamingPoolSize = 1024; //1 meg for disk streaming.
    public float memoryCutoffThreshold = 0.9f;   //When reaching 90% of used memory, lowest priority sounds are killed.

	static private AkGlobalSoundEngineInitializer ms_Instance;
	
    void Awake()
    {
        if (ms_Instance != null)
            return; //Don't init twice
		
#if UNITY_ANDROID
        InitalizeAndroidSoundBankIO();
#endif

        Debug.Log("WwiseUnity: Initialize sound engine ...");

        //Use default properties for most SoundEngine subsystem.  
        //The game programmer should modify these when needed.  See the Wwise SDK documentation for the initialization.
        //These settings may very well change for each target platform.
        AkMemSettings memSettings = new AkMemSettings();
        memSettings.uMaxNumPools = 20;

        AkDeviceSettings deviceSettings = new AkDeviceSettings();
		AkSoundEngine.GetDefaultDeviceSettings(deviceSettings);

        AkStreamMgrSettings streamingSettings = new AkStreamMgrSettings();
        streamingSettings.uMemorySize = (uint)streamingPoolSize * 1024;

        AkInitSettings initSettings = new AkInitSettings();
        AkSoundEngine.GetDefaultInitSettings(initSettings);
        initSettings.uDefaultPoolSize = (uint)defaultPoolSize * 1024;

        AkPlatformInitSettings platformSettings = new AkPlatformInitSettings();
        AkSoundEngine.GetDefaultPlatformInitSettings(platformSettings);
        platformSettings.uLEngineDefaultPoolSize = (uint)lowerPoolSize * 1024;
        platformSettings.fLEngineDefaultPoolRatioThreshold = memoryCutoffThreshold;
#if UNITY_IOS && !UNITY_EDITOR
        platformSettings.bAppListensToInterruption = true;
#endif // #if UNITY_IOS && !UNITY_EDITOR

        AkMusicSettings musicSettings = new AkMusicSettings();
        AkSoundEngine.GetDefaultMusicSettings(musicSettings);

        AKRESULT result = AkSoundEngine.Init(memSettings, streamingSettings, deviceSettings, initSettings, platformSettings, musicSettings);
        if (result != AKRESULT.AK_Success)
        {
            Debug.LogError("WwiseUnity: Failed to initialize the sound engine. Abort.");
            return; //AkSoundEngine.Init should have logged more details.
        }

        AkBankPath.UsePlatformSpecificPath();
        string platformBasePath = AkBankPath.GetPlatformBasePath();
// Note: Android low-level IO uses relative path to "assets" folder of the apk as SoundBank folder.
// Unity uses full paths for general path checks. We thus don't use DirectoryInfo.Exists to test 
// our SoundBank folder for Android.
#if !UNITY_ANDROID && !UNITY_METRO
        if ( ! AkBankPath.Exists(platformBasePath) )
        {
            string errorMsg = string.Format("WwiseUnity: Failed to find soundbank folder: {0}. Abort.", platformBasePath);
            Debug.LogError(errorMsg);
            return;
        }
#endif // #if !UNITY_ANDROID

        AkSoundEngine.SetBasePath(platformBasePath);
		AkSoundEngine.SetCurrentLanguage(language);
		
		result = AkCallbackManager.Init();
		if (result != AKRESULT.AK_Success)
        {
            Debug.LogError("WwiseUnity: Failed to initialize Callback Manager. Terminate sound engine.");
			AkSoundEngine.Term();			
			return;
        }
		
		AkCallbackManager.SetMonitoringCallback(ErrorLevel.ErrorLevel_All, null);
		
		Debug.Log("WwiseUnity: Sound engine initialized.");
		
		//The sound engine should not be destroyed once it is initialized.
		DontDestroyOnLoad(this);
		ms_Instance = this;
		
        //Load the init bank right away.  Errors will be logged automatically.
        uint BankID;
        result = AkSoundEngine.LoadBank("Init.bnk", AkSoundEngine.AK_DEFAULT_POOL_ID, out BankID);
        if (result != AKRESULT.AK_Success)
        {
            Debug.LogError("WwiseUnity: Failed load Init.bnk with result: " + result.ToString());
        }
    }
	
	void OnDestroy()
    {	
    	ms_Instance = null;
		// Do nothing. AkGlobalSoundEngineTerminator handles sound engine termination.
    }
	
	void OnApplicationQuit()
	{
		// Do nothing. This is called before OnDestroy().
	}
	
	void OnEnable()
	{
		//The sound engine was not terminated normally.  Make this instance the one that will manage
		//the updates and termination.
		//This happen when Unity resets everything when a script changes.
		if (ms_Instance == null && AkSoundEngine.IsInitialized())		
			ms_Instance = this;		
	}
	

	//Use LateUpdate instead of Update() to ensure all gameobjects positions, listener positions, environements, RTPC, etc are set before finishing the audio frame.
    void LateUpdate()
    {
		//Execute callbacks that occured in last frame (not the current update)		
		if (ms_Instance != null)
		{
			AkCallbackManager.PostCallbacks();
			AkSoundEngine.RenderAudio();
		}
    }

#if UNITY_ANDROID
    private bool InitalizeAndroidSoundBankIO()
    {
        JavaVM.AttachCurrentThread();

        // Find main activity..
        IntPtr cls_Activity = JNI.FindClass("com/unity3d/player/UnityPlayer");
        int fid_Activity = JNI.GetStaticFieldID(cls_Activity, "currentActivity", "Landroid/app/Activity;");
        IntPtr obj_Activity = JNI.GetStaticObjectField(cls_Activity, fid_Activity);
        if ( obj_Activity == IntPtr.Zero )
        {
            Debug.LogError("WwiseUnity: Failed to get UnityPlayer activity. Abort.");
            return false;
        }
        
        // Create a JavaClass object...
        const string AkPackageClass = "com/audiokinetic/aksoundengine/SoundBankIOInitalizerJavaClass";
        IntPtr cls_JavaClass = JNI.FindClass(AkPackageClass);
        if ( cls_JavaClass == IntPtr.Zero )
        {
            Debug.LogError("WwiseUnity: Failed to find Java class. Check if plugin JAR file is available in Assets/Plugins/Android. Abort.");
            return false;
        }

        int mid_JavaClass = JNI.GetMethodID(cls_JavaClass, "<init>", "(Landroid/app/Activity;)V");

        IntPtr obj_JavaClass = JNI.NewObject(cls_JavaClass, mid_JavaClass, obj_Activity);


        // Create a global reference to the JavaClass object and fetch method id(s)..
        IntPtr soundBankIOInitalizerJavaClass = JNI.NewGlobalRef(obj_JavaClass);
        int setAssetManagerMethodID = JNI.GetMethodID(cls_JavaClass, "SetAssetManager", "()I");

        if ( setAssetManagerMethodID == 0 )
        {
            Debug.LogError("WwiseUnity: Failed to find Java class method. Check method name and signature in JNI query. Abort.");
            return false;
        }

        // get the Java String object from the JavaClass object
        IntPtr ret = JNI.CallObjectMethod(soundBankIOInitalizerJavaClass, setAssetManagerMethodID);
        if ( ret != IntPtr.Zero )
        {
            Debug.LogError("WwiseUnity: Failed to set AssetManager for Android SoundBank low-level IO handler.");
            return false;
        }

        return true;
        
    }

    void OnApplicationFocus(bool focus)
    {
        if (ms_Instance != null)
        {
            if ( focus )
            {
                uint id = AkSoundEngine.PostEvent("Resume_All_Global", null);
            }
            else
            {
                uint id = AkSoundEngine.PostEvent("Pause_All_Global", null);
            }

            AkSoundEngine.RenderAudio();
        }

    }
#endif


#if UNITY_IOS && !UNITY_EDITOR
    void OnApplicationPause(bool pause)
    {
        AkSoundEngine.ListenToAudioSessionInterruption(pause);
    }
#endif // #if UNITY_IOS && !UNITY_EDITOR

}
