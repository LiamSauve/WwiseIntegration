//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;

//This component is added automatically to all Unity Game Object that are passed to Wwise API (see AkSoundEngine.cs).  
//It manages registration of the game object inside the Wwise Sound Engine
public class AkGameObject : MonoBehaviour {

	void Awake()
    {				
        //Register a Game Object in the sound engine, with its name.		
        AkSoundEngine.RegisterGameObj(gameObject, gameObject.name);
			
		//Set the original position
		AkSoundEngine.SetObjectPosition(
            gameObject, 
            transform.position.x, 
            transform.position.y, 
            transform.position.z, 
            transform.forward.x,
            transform.forward.y, 
            transform.forward.z);
    }

    void OnDestroy()
    {		
		if (AkSoundEngine.IsInitialized())
        {
        	AkSoundEngine.UnregisterGameObj(gameObject);
        }
    }
}
