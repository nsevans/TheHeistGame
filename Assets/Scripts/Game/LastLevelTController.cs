using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LastLevelTController : MonoBehaviour {

	public GameObject firstDialoguePoint;
	public GameObject lastDialoguePoint;
	public GameObject itemToSteal;

	//public GameObject endLevelCheckpoint;

	void Start(){
		//endLevelCheckpoint.SetActive(false);
	}

	//void Update(){
	//	if(buttonText.text == "Exit" && !firstDialoguePoint.activeInHierarchy){
	//		itemToSteal.SetActive(false);
	//	}
	//}

	void Update(){
		if(!firstDialoguePoint.activeInHierarchy && !lastDialoguePoint.activeInHierarchy){
			
			itemToSteal.SetActive(false);
			//endLevelCheckpoint.SetActive(true);
		}
	}

}
