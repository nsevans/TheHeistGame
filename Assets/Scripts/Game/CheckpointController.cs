using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckpointController : MonoBehaviour {

	public LevelController levelController;
	public Text checkpointText;
	private bool checkpointEntered;

	void Start(){
		checkpointEntered = false;
	}

	void OnTriggerEnter(Collider other){
		if(other.tag == "Player" && !checkpointEntered){
			//Set Checkpoint
			StartCoroutine(CheckpointHit());
			levelController.UpdateCurrentCheckpoint(other.transform.position);
			checkpointEntered = true;
		}
	}

	IEnumerator CheckpointHit(){
		checkpointText.gameObject.SetActive(true);
		yield return new WaitForSeconds(1f);
		checkpointText.gameObject.SetActive(false);
		gameObject.SetActive(false);
	}
}
