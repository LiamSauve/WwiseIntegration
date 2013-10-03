using UnityEngine;
using System.Collections;

public class RTPCEvent : MonoBehaviour
{
  private SoundBank __sndbnk;
  public string desiredSoundBank;
  public TextMesh rtpcMesh;
  private float rtpcValue = 0.0f;
  private float delta = 0.001f;

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
      AkSoundEngine.PostEvent("Play_playlist", this.gameObject);
    }

    RTPCUpdate();
  }

  void RTPCUpdate()
  {
    if (Input.GetKey(KeyCode.Z))
    {
      rtpcValue -= delta;
      checkRTPCBounds();
    }

    if (Input.GetKey(KeyCode.X))
    {
      rtpcValue += delta;
      checkRTPCBounds();
    }

    rtpcMesh.text = rtpcValue.ToString();

    AkSoundEngine.SetRTPCValue("intensity", rtpcValue);
  }

  void checkRTPCBounds()
  {
    if (rtpcValue > 4.0f) rtpcValue = 4.0f;
    if (rtpcValue < 0.0f) rtpcValue = 0.0f;
  }
}
