using UnityEngine;
using System.Collections;

public class SoundBank : MonoBehaviour
{
  private string  __soundBank;
  private uint    __bankID;

  public string SoundBankGetSet
  {
    get { return __soundBank; }
    set { __soundBank = value; }
  }

  public void LoadUp(string s)
  {
    __soundBank = s;
    AkSoundEngine.LoadBank(__soundBank, AkSoundEngine.AK_DEFAULT_POOL_ID, out __bankID);
  }

  public void CleanUp()
  {
    AkSoundEngine.UnloadBank(__soundBank, (System.IntPtr)__bankID);
  }
}