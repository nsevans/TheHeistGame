using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void StartGame(){
		//Go to first level
		Time.timeScale = 1;
		SceneManager.LoadScene(1);
	}

	public void QuitGame(){
		//Exit out of the application
		Application.Quit();
	}

	public void CreditsScene(){
		//Go to credits scene
		SceneManager.LoadScene("Credits");
	}

	public void ReturnToMenu(){
		SceneManager.LoadScene("MainMenu");
	}

	public void PlayTestScene(){
		SceneManager.LoadScene("TestScene");
	}
}
