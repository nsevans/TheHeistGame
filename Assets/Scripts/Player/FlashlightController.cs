using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightController : MonoBehaviour {

	public Light flashlight;
	public AudioClip click;

	public void ToggleFlashlight(){
		SetFlashlight(!flashlight.enabled);
	}


	public void SetFlashlight(bool isOn){
		GetComponent<AudioSource>().PlayOneShot(click);
		flashlight.enabled = isOn;
	}
}
