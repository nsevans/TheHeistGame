using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {

	public Dialogue[] dialogue;

	public void TriggerDialogue(){
		FindObjectOfType<DialogueManager>().StartDialogue(dialogue);

	}

	void OnTriggerEnter(Collider other){
		if(other.tag == "Player"){
			TriggerDialogue();
			gameObject.SetActive(false);
		}
	}



}
