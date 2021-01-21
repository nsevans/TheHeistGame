using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TimeChangeCountdownController : MonoBehaviour {

	public bool isDay;													//True when the time is day

	private List<GameObject> changableLights = new List<GameObject>();	//List of all lights that change depending on the time of day
	private List<GameObject> changableDoors = new List<GameObject>();
	private List<GameObject> changableNPCs = new List<GameObject>();

	public RawImage timeUI;
	public Texture sunImageUI;
	public Texture moonImageUI;
	//public Text cooldownUI;

	public Material dayMaterial;
	public Material nightMaterial;

	public Material dayBox;
	public Material nightBox;

	public AudioClip timeChangeSound;

	public Text cooldownUI;
	public float cooldownRate = 5.0f;
	private float nextUseTime;

	// Use this for initialization
	void Start () {
		nextUseTime = Time.time;

		changableLights.AddRange(GameObject.FindGameObjectsWithTag("Changable Light"));
		changableDoors.AddRange(GameObject.FindGameObjectsWithTag("Changable Door"));
		changableNPCs.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
		ToggleObjects();
	}
	
	// Update is called once per frame
	void Update () {
		if(nextUseTime - Time.time <= 0.0f){
			cooldownUI.text = "Cooldown Time: 0s";
		}else{
			cooldownUI.text = "Cooldown Time: " + Math.Round(nextUseTime - Time.time)+"s";
		}
	}

	public void ToggleObjects(){
		ToggleDoors();
		ToggleLights();
		ToggleNPCs();

		//Change Material depending on time of day
		if(isDay){
			timeUI.texture = sunImageUI;
			transform.GetChild(0).GetComponent<MeshRenderer>().material = dayMaterial;
			RenderSettings.skybox = dayBox;
		}else{
			timeUI.texture = moonImageUI;
			transform.GetChild(0).GetComponent<MeshRenderer>().material = nightMaterial;
			RenderSettings.skybox = nightBox;
		}
		DynamicGI.UpdateEnvironment();
	}

	private void ToggleLights(){
		foreach(GameObject go in changableLights){
			if(go.name.Contains("Night")){
				go.SetActive(!isDay);
			} else if(go.name.Contains("Day")){
				go.SetActive(isDay);
			}
		}
	}

	private void ToggleDoors(){
		foreach(GameObject go in changableDoors){
			foreach(Transform child in go.transform){
				if(child.GetComponent<DoorController>() != null){
					if(child.GetComponent<DoorController>().IsOpen()){
						child.GetComponent<DoorController>().CloseDoor();
					}
					child.GetComponent<DoorController>().ToggleLock();
					child.GetComponent<DoorController>().navMeshUpdated = false;
				}
			}
		}
	}

	private void ToggleNPCs(){
		changableNPCs.AddRange(GameObject.FindGameObjectsWithTag("Enemy KOd"));
		foreach(GameObject go in changableNPCs){
			if(go.name.Contains("Night")){
				go.SetActive(!isDay);
			}else if(go.name.Contains("Day")){
				go.SetActive(isDay);
			}
		}
	}

	public void SetTimeChange(bool isDay){
		if(Time.time > nextUseTime){
			this.isDay = isDay;
			StartCoroutine(ToggleObjectsWithDelay());
			nextUseTime = Time.time + cooldownRate;
		}
	}

	public void ToggleTimeChange(){
		SetTimeChange(!isDay);
	}

	IEnumerator ToggleObjectsWithDelay(){
		GetComponent<AudioSource>().PlayOneShot(timeChangeSound);
		yield return new WaitForSeconds(1.0f);
		ToggleObjects();
	}

	float GetNextUseTime(){
		return nextUseTime;
	}

	void SetNextUseTime(){
		nextUseTime = Time.time + cooldownRate;
	}
}
