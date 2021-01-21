using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.UIElements;
using UnityEngine.SceneManagement;
using System.Runtime.Remoting;

public class CutSceneController : MonoBehaviour {

	public Light redLight;
	public Light blueLight;
	public float flashInterval = 0.3f;

	public Text endOfSceneText;
	public GameObject foreground;
	private Animator foregroundAnimator;
	public Animator dialogueAnimator;
	private bool conversationEnded;

	// Use this for initialization
	void Start () {
		foregroundAnimator = foreground.GetComponent<Animator>();
		endOfSceneText.enabled = false;
		foreground.SetActive(false);
		redLight.enabled = true;
		blueLight.enabled = false;
		InvokeRepeating("FlashingLights",0.0f,flashInterval);
		conversationEnded = false;
	}

	void Update(){
		if(conversationEnded && foregroundAnimator.GetCurrentAnimatorStateInfo(0).IsName("FadeOut")){
			if(SceneManager.GetActiveScene().buildIndex + 1 >= SceneManager.sceneCountInBuildSettings){
				SceneManager.LoadScene(0);
			}else{
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
			}
		}
	}

	void FlashingLights(){
		redLight.enabled = !redLight.enabled;
		blueLight.enabled = !blueLight.enabled;
	}

	public void NextScene(){
		if(!dialogueAnimator.GetBool("isOpen")){
			endOfSceneText.enabled = true;
			
			foreground.SetActive(true);
			foregroundAnimator.SetBool("fadeOut", true);

			StartCoroutine(Wait(foregroundAnimator.GetCurrentAnimatorStateInfo(0).length*2));
		}
	}

	IEnumerator Wait(float time){
		yield return new WaitForSeconds(time);
		conversationEnded = true;
	}
}
