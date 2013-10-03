//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;
using System.IO;


// This class is used for returning correct path strings for retrieving various soundbank file locations,
// based on Unity usecases. The main concerns include platform sub-folders and path separator conventions.
// The class makes path string retrieval transparent for all platforms in all contexts. Clients of the class
// only needs to use the public methods to get physically correct path strings after setting flag isToUsePosixPathSeparator. By default, the flag is turned off for non-buildPipeline usecases.
//
// Unity usecases:
// - A BuildPipeline user context uses POSIX path convention for all platforms, including Windows and Xbox360.
// - Other usecases use platform-specific path conventions.

public class AkBankPath
{

	private static string basePath = Path.Combine("Audio", "GeneratedSoundBanks");
	private static bool isToUsePosixPathSeparator = false;
	private static bool isToAppendTrailingPathSeparator = true;
	
	static AkBankPath ()
	{
		isToUsePosixPathSeparator = false;
	}

	public static void UsePosixPath() { isToUsePosixPathSeparator = true; }
	public static void UsePlatformSpecificPath() { isToUsePosixPathSeparator = false; }
	
	public static void SetToAppendTrailingPathSeparator(bool add) { isToAppendTrailingPathSeparator = add; }

#if !UNITY_METRO
	public static bool Exists(string path)
	{
		DirectoryInfo basePathDir = new DirectoryInfo(path);
		return basePathDir.Exists;
	}
#endif // #if !UNITY_METRO

	public static string GetBasePath() { return basePath; }
	
	public static string GetFullBasePath() 
	{
		// Get full path of base path
#if UNITY_ANDROID && ! UNITY_EDITOR
		// Wwise Android SDK now loads SoundBanks from APKs.
 		string fullBasePath = basePath;
#elif UNITY_PS3 && ! UNITY_EDITOR
		// NOTE: Work-around for Unity PS3 (up till 3.5.2) bug: Application.streamingAssetsPath points to wrong location: /app_home/PS3_GAME/USRDIR/Raw
		const string StreamingAssetsPath = "/app_home/PS3_GAME/USRDIR/Media/Raw";
		string fullBasePath = Path.Combine(StreamingAssetsPath, basePath);
#else
		string fullBasePath = Path.Combine(Application.streamingAssetsPath, basePath);
#endif
		LazyAppendTrailingSeparator(ref fullBasePath);
		LazyConvertPathConvention(ref fullBasePath);
		return fullBasePath;
	}
	
	public static string GetPlatformBasePath()
	{
		// Combine base path with platform sub-folder
		string platformBasePath = Path.Combine(GetFullBasePath(), GetPlatformSubDirectory());
		
		LazyAppendTrailingSeparator(ref platformBasePath);

		LazyConvertPathConvention(ref platformBasePath);

		return platformBasePath;
	}

	static public string GetPlatformSubDirectory()
	{
		string platformSubDir = "Undefined platform sub-folder";
#if UNITY_STANDALONE_WIN
		// NOTE: Due to a Unity3.5 bug, projects upgraded from 3.4 will have malfunctioning platform preprocessors
		// We have to use Path.DirectorySeparatorChar to know if we are in the Unity Editor for Windows or Mac.
		if (Path.DirectorySeparatorChar == '/')
			platformSubDir = "Mac";
		else 
			platformSubDir = "Windows";
#elif UNITY_STANDALONE_OSX
		if (Path.DirectorySeparatorChar == '/')
			platformSubDir = "Mac";
		else 
			platformSubDir = "Windows";
#elif UNITY_XBOX360
	#if UNITY_EDITOR
		// In Unity Editor, we cannot play XBox360 banks.
		// Use Windows instead.
		platformSubDir = "Windows"; 
	#else
		platformSubDir = "XBox360";
	#endif // #if UNITY_EDITOR
#elif UNITY_IOS
	#if UNITY_EDITOR
		// Use Mac for all Editor simulation.
		platformSubDir = "Mac";
	#else
		platformSubDir = "iOS";
	#endif // #if UNITY_EDITOR
#elif UNITY_ANDROID
	#if UNITY_EDITOR
		// Use Mac or Windows for all Editor simulation.
		if (Path.DirectorySeparatorChar == '/')
			platformSubDir = "Mac";
		else 
			platformSubDir = "Windows";
	#else
		platformSubDir = "Android";
	#endif // #if UNITY_EDITOR
#elif UNITY_PS3
	#if UNITY_EDITOR
		// WG-21730 In Unity Editor, we cannot play PS3 banks.
		// Use Windows instead.
		platformSubDir = "Windows"; 
	#else
		platformSubDir = "PS3";
	#endif // #if UNITY_EDITOR
#elif UNITY_METRO
	platformSubDir = "Windows";
#endif

		return platformSubDir;
	}

	public static void LazyConvertPathConvention(ref string path)
	{
		if (isToUsePosixPathSeparator)
			ConvertToPosixPath(ref path);
		else
		{
#if !UNITY_METRO
			if (Path.DirectorySeparatorChar == '/')
				ConvertToPosixPath(ref path);
			else
				ConvertToWindowsPath(ref path);
#else
			ConvertToWindowsPath(ref path);
#endif // #if !UNITY_METRO
		}
	} 
	
	public static void ConvertToWindowsPath(ref string path)
    {
        path.Trim();
        path = path.Replace("/", "\\");
        path = path.TrimStart('\\');
    }
	
	public static void ConvertToPosixPath(ref string path)
    {
        path.Trim();
        path = path.Replace("\\", "/");
        path = path.TrimStart('\\');
    }
	
	public static void LazyAppendTrailingSeparator(ref string path)
	{
		if ( ! isToAppendTrailingPathSeparator )
			return;
#if !UNITY_METRO
		if ( ! path.EndsWith(Path.DirectorySeparatorChar.ToString()) )
        {
            path += Path.DirectorySeparatorChar;
        }
#else
		if ( ! path.EndsWith("\\") )
        {
            path += "\\";
        }
#endif // #if !UNITY_METRO
	}
}

