using UnityEngine;
using System.Collections;

public class HUDSounds
{
    private static HUDSounds m_Instance;

    public static HUDSounds Instance()
    {
        if(m_Instance == null)
        {
            m_Instance = new HUDSounds();
            uint _bankID;
            AkSoundEngine.LoadBank("HUD", AkSoundEngine.AK_DEFAULT_POOL_ID, out _bankID);
        }

        return m_Instance;
    }

    public void HealthSound()
    {
        AkSoundEngine.PostEvent("Play_HUD_Health", Camera.main.gameObject);
    }

    public void TalismanSound()
    {
        AkSoundEngine.PostEvent("Play_HUD_Talisman", Camera.main.gameObject);
    }

    public void StartBreathSound()
    {
        AkSoundEngine.PostEvent("Play_HUD_BreathUse", Camera.main.gameObject);
    }

    public void StopBreathSound()
    {
        AkSoundEngine.PostEvent("Stop_HUD_BreathUse", Camera.main.gameObject);
    }
	
	public void StartDyingSounds()
	{
		Debug.Log("Fuck");
		AkSoundEngine.PostEvent("Start_Claes_Dying", Camera.main.gameObject);
	}
	
	public void StopDyingSounds()
	{
		Debug.Log("UnFuck");
		AkSoundEngine.PostEvent(3999912200, Camera.main.gameObject);
	}
	
}
