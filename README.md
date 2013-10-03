Wwise/Unity3D Integration

This project is intended strictly for educational purposes, initially shown as a presentation for Algonquin College's game development program.
It was initially shown on October 1st, 2013
It contains proprietary scripts from AudioKinetic, however everything is attainable through their website.
https://www.audiokinetic.com/

Be warned, yo. As I have done this is a short amount of time, there may be some mistakes. If you run into trouble... well, I have faith in you.

GLHF.

Setting up the plugin in Unity
- Get the plugin from the AK site
- Create the Folder structure Assets/Plugins
- Place the DLL in Assets/Plugins
- Create the Folder structure Assets/StreamingAssets/Audio (We will need this later)
- Go to the dropdown menu Edit -> Project Setting -> Script Execution Order
- In the Script Execution order inspector, add AKGlobalSoundEngineInitializer.cs, AKGameObject, AKGameObjectTracker, and AKGlobalSoundEngineTerminator.cs
- In your initial scene in unity, create an Empty Game Object and add the components AKGlobalSoundEngineInitializer.cs and AKGlobalSoundEngineTerminator.cs
- Create a GameObject with both the Initializer and the Terminator on it. Make sure this is active in whichever scene you want the engine to initialize in.
- You will not have to initialize again for the remainder of the project.
- That’s it! From here, you can import your Wwise project and make things happen.

Setting up and generating a Wwise project with a simple sound event
- Open Wwise - The initial dialog will have standard new or open previous project. Projects, like Unity, just folders 
- Drag an audio file into Wwise under the Default Work Unit tab.
- Right click on the file and select Convert. Select Windows.
- Right click on the file and select New Event->Play
- In the dropdown menu click Layouts->Soundbank to start generating the project to bring to Unity
- Click User Settings. Click Override Project SoundBank Settings. Click Generate Header File.
- Under Platform, click Windows and under Languages, click English
- Click New. Create and name a new soundbank.
- Toggle Default Work Unit and make sure the soundbank is also toggled. Click and drag any events into your Soundbank.
- Click Update. (I’m paranoid)
- Click Generate.
- Open the project directory and find GeneratedSoundBanks
- Copy that folder to Assets/StreamingAssets/Audio directory in your unity project.
- You can now load and call your Wwise events in Unity!
- Load the sound bank using the AkSoundEngine.LoadBank function
- Play the sound using the AkSoundEngine.PostEvent function