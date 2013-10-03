using UnityEngine;
using System.Collections;

public class SimpleEvent : MonoBehaviour
{
  private SoundBank __sndbnk;
  public  string    desiredSoundBank;

  void Awake()
  {
    __sndbnk = this.gameObject.GetComponent<SoundBank>();

    if (desiredSoundBank == string.Empty)
    {
      Debug.LogError("Soundbank's empty, yo. Ya mean to do that?");
    }
    else
    {
      __sndbnk.LoadUp(desiredSoundBank);
    }
  }

  void OnDisable()
  {
    __sndbnk.CleanUp();
  }

	void Update()
  {
    if (Input.GetKeyDown(KeyCode.A))
    {
      AkSoundEngine.PostEvent("Play_Hit_Simple", this.gameObject);
    }
	}
}
