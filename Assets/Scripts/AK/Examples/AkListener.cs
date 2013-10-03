//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;

//Add this script on the game object that represent an audio listener.  It will track its position in Wwise.
//More information about Listeners in the Wwise SDK documentation : 
//Wwise SDK » Sound Engine Integration Walkthrough » Integrate Wwise Elements into Your Game » Integrating Listeners 
public class AkListener : MonoBehaviour
{
	public int listenerId = 0;	//Wwise supports up to 8 listeners.  [0-7]
	private Vector3 m_Position;
    private Vector3 m_Top;
	private Vector3 m_Front;
    
    void Update()
    {
        if (m_Position == transform.position && m_Front == transform.forward && m_Top == transform.up)
            return;	//Position didn't change, no need to update.

        m_Position = transform.position;
        m_Front = transform.forward;      
		m_Top = transform.up;

        //Update position
        AkSoundEngine.SetListenerPosition(    
		    transform.forward.x,
            transform.forward.y, 
            transform.forward.z,
		    transform.up.x,
            transform.up.y, 
            transform.up.z,
            transform.position.x, 
            transform.position.y, 
            transform.position.z,            
            (ulong)listenerId);
    }
}


