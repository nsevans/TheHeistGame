using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : MonoBehaviour {

	[SerializeField] private GameObject pausePanel;
	private bool paused;
	public PlayerController playerController;
	// Use this for initialization
	void Awake () {
		paused = false;
		pausePanel.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)){
			TogglePause();
		}
	}

	void TogglePause(){
		if(paused){
			Time.timeScale = 1;
			playerController.enabled = true;
			pausePanel.SetActive(false);
		} else{
			Time.timeScale = 0;
			playerController.enabled = false;
			pausePanel.SetActive(true);
		}
		paused = !paused;
	}

	public void ResumeGame(){
		TogglePause();
	}
}
