using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevelCheckpointController : MonoBehaviour {

	public LevelController levelController;

	void OnTriggerEnter(Collider other){
		if(other.tag == "Player"){
			levelController.SetLevelState(levelController.W_STATE);
		}
	}

}
