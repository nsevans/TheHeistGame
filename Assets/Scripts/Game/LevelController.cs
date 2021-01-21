using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour {

	private bool playing;
	public readonly string P_STATE = "playing";
	private bool levelComplete;
	public readonly string W_STATE = "complete";
	private bool caught;
	public readonly string C_STATE = "caught";

	private List<GameObject> checkPoints;
	private Vector3 currentCheckpoint;
	public GameObject itemStolenCheckpoint;
	public GameObject endOfLevelCheckpoint;
	//public List<Vector3> guardPositions;

	private List<GameObject> dialoguePoints;
	public GameObject itemStolenDialoguePoint;
	public GameObject endOfLevelDialoguePoint;

	private bool pointsEnabled;

	public GameObject itemToSteal;

	public GameObject player;

	public GameObject losePanel;
	public GameObject winPanel;

	// Use this for initialization
	void Start () {
		pointsEnabled = false;

		checkPoints = new List<GameObject>();
		checkPoints.AddRange(GameObject.FindGameObjectsWithTag("Checkpoint"));

		player.GetComponent<PlayerController>().enabled = true;
		GetComponent<PauseController>().enabled = true;

		Time.timeScale = 1;

		dialoguePoints = new List<GameObject>();
		dialoguePoints.AddRange(GameObject.FindGameObjectsWithTag("Dialogue"));

		DisableDialogueAndCheckpointMeshRenderers();
		currentCheckpoint = player.transform.position;
		losePanel.SetActive(false);
		winPanel.SetActive(false);
		SetLevelState(P_STATE);
	}
	
	// Update is called once per frame
	void Update () {
		if(playing){
			if(!itemToSteal.activeInHierarchy && !pointsEnabled){
				itemStolenDialoguePoint.SetActive(true);
				itemStolenCheckpoint.SetActive(true);
				endOfLevelDialoguePoint.SetActive(true);
				endOfLevelCheckpoint.SetActive(true);
				pointsEnabled = true;
			}
		}else if(levelComplete){
			DisplayLevelCompleteScreen();
		}else if(caught){
			//Pause game and display lose screen
			DisplayCaughtScreen();
		}
	}

	public void SetLevelState(string state){
		//switch(state){
		if(state.Equals(P_STATE)){
			playing = true;
			levelComplete = false;
			caught = false;
		}else if(state.Equals(W_STATE)){
			playing = false;
			levelComplete = true;
			caught = false;
		}else if(state.Equals(C_STATE)){
			playing = false;
			levelComplete = false;
			caught = true;
		}
	}

	public string GetCurrentState(){
		if(playing){
			return P_STATE;
		}else if(levelComplete){
			return W_STATE;
		}else{
			return C_STATE;
		}

	}

	public void UpdateCurrentCheckpoint(Vector3 pos){
		currentCheckpoint = pos;
	}

	void DisplayCaughtScreen(){
		Time.timeScale = 0;
		player.GetComponent<PlayerController>().enabled = false;
		GetComponent<PauseController>().enabled = false;
		losePanel.SetActive(true);

	}

	void DisplayLevelCompleteScreen(){
		Time.timeScale = 0;
		player.GetComponent<PlayerController>().enabled = false;
		GetComponent<PauseController>().enabled = false;
		winPanel.SetActive(true);
	}

	public void RestartAtCheckpoint(){
		Time.timeScale = 1;
		SetLevelState(P_STATE);
		player.transform.position = currentCheckpoint;
		player.GetComponent<PlayerController>().enabled = true;
		//foreach(GameObject go in GameObject.FindGameObjectsWithTag("Enemy")){
		//	go.GetComponent<GuardController>().GoToNextPoint();
		//}
		GetComponent<PauseController>().enabled = true;

		losePanel.SetActive(false);
	}

	public void QuitGame(){
		Time.timeScale = 1;
		//print("Return to menu scene");
		player.GetComponent<PlayerController>().enabled = true;
		GetComponent<PauseController>().enabled = true;
		SceneManager.LoadScene("MainMenu");
	}

	public void NextLevel(){
		Time.timeScale = 1;
		player.GetComponent<PlayerController>().enabled = true;
		GetComponent<PauseController>().enabled = true;
		if(SceneManager.GetActiveScene().buildIndex + 1 >= SceneManager.sceneCountInBuildSettings){
			SceneManager.LoadScene(0);
		}else{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		}
	}

	void DisableDialogueAndCheckpointMeshRenderers(){
		foreach(GameObject cp in checkPoints){
			cp.GetComponent<MeshRenderer>().enabled = false;
		}
		itemStolenCheckpoint.GetComponent<MeshRenderer>().enabled = false;
		itemStolenCheckpoint.SetActive(false);
		endOfLevelCheckpoint.GetComponent<MeshRenderer>().enabled = false;
		endOfLevelCheckpoint.SetActive(false);
		foreach(GameObject dp in dialoguePoints){
			dp.GetComponent<MeshRenderer>().enabled = false;
		}
		itemStolenDialoguePoint.GetComponent<MeshRenderer>().enabled = false;
		itemStolenDialoguePoint.SetActive(false);
		endOfLevelDialoguePoint.GetComponent<MeshRenderer>().enabled = false;
		endOfLevelDialoguePoint.SetActive(false);
	}
}
