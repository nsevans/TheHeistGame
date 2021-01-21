using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Diagnostics;

public class DialogueManager : MonoBehaviour {

	public Text nameText;
	public Text dialogueText;
	public Text nextButtonText;

	public Animator animator;

	private Queue<string> names;
	private Queue<string> sentences;

	// Use this for initialization
	void Start () {
		names = new Queue<string>();
		sentences = new Queue<string>();
	}

	public void StartDialogue(Dialogue[] dialogues){

		animator.SetBool("isOpen",true);
		if(FindObjectOfType<PlayerController>() != null){
			FindObjectOfType<PlayerController>().enabled = false;
		}
		if(FindObjectOfType<PauseController>() != null){
			FindObjectOfType<PauseController>().enabled = false;
		}
		Time.timeScale = 0;
		sentences.Clear();

		nextButtonText.text = "Continue";

		foreach(Dialogue d in dialogues){
			foreach(string s in d.sentences){
				names.Enqueue(d.name);
				sentences.Enqueue(s);
			}
				
		}
		DisplayNextSentence();
		
	}

	public void DisplayNextSentence(){
		if(sentences.Count == 0){
			EndDialogue();
			return;
		}
		if(sentences.Count == 1){
			nextButtonText.text = "Exit";
		}

		string name = names.Dequeue();
		nameText.text = name;

		string sentence = sentences.Dequeue();
		//dialogueText.text = sentence;
		StopAllCoroutines();
		StartCoroutine(TypeSentence(sentence));
	}

	IEnumerator TypeSentence(string sentence){
		dialogueText.text = "";
		foreach(char letter in sentence.ToCharArray()){
			dialogueText.text += letter;
			yield return null;
		}
	}

	void EndDialogue(){
		animator.SetBool("isOpen",false);
		if(FindObjectOfType<PlayerController>() != null){
			FindObjectOfType<PlayerController>().enabled = true;
		}
		if(FindObjectOfType<PauseController>() != null){
			FindObjectOfType<PauseController>().enabled = true;
		}
		Time.timeScale = 1;
	}

}
