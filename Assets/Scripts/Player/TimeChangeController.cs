using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;
using System;

public class TimeChangeController : MonoBehaviour {

	public TimeChangeCountdownController tccController;

	void SetTimeChange(bool isDay){
		tccController.SetTimeChange(isDay);
	}

	public void ToggleTimeChange(){
		tccController.SetTimeChange(!tccController.isDay);
	}
}
