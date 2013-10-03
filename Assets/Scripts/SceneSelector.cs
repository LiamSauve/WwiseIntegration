using UnityEngine;
using System.Collections;

public class SceneSelector : MonoBehaviour
{
	void Update ()
  {
	  if(Input.GetKeyDown(KeyCode.Q))
    {
      Application.LoadLevel("SimpleEvent");
    }
    if (Input.GetKeyDown(KeyCode.W))
    {
      Application.LoadLevel("RandomContainer");
    }
    if (Input.GetKeyDown(KeyCode.E))
    {
      Application.LoadLevel("SequenceContainer");
    }
    if (Input.GetKeyDown(KeyCode.R))
    {
      Application.LoadLevel("RTPC");
    }
    if (Input.GetKeyDown(KeyCode.T))
    {
      Application.LoadLevel("Listener");
    }
	}
}
