using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BatonController : MonoBehaviour {

	public GameObject batonAnimator;
	private bool isSwinging = false;									//True while the Player is swinging their baton
	public float swingReach = 0.6f;										//How far the Player can swing their baton in front of themselves
	public GameObject unconsciousNightGuard;
	public GameObject unconsciousDayGuard;
	public AudioClip swing;
	public AudioClip hit;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SwingBaton(){
		if(isSwinging){
			//yield break;
			return;
		}
		isSwinging = true;
		batonAnimator.GetComponent<Animator>().SetTrigger("Active");
		GetComponent<AudioSource>().PlayOneShot(swing);
		isSwinging = false;
	}

	void OnTriggerEnter(Collider other){
		if(other.tag == "Enemy"){
			GetComponent<AudioSource>().PlayOneShot(hit);
			Vector3 guardPos = other.transform.position;
			if(other.name.Contains("Night")){
				Instantiate(unconsciousNightGuard, guardPos, unconsciousNightGuard.transform.rotation);
			} else if(other.name.Contains("Day")){
				Instantiate(unconsciousDayGuard, guardPos, unconsciousDayGuard.transform.rotation);
			}

			other.GetComponent<MeshRenderer>().enabled = false;
			other.GetComponent<BoxCollider>().enabled = false;
			other.GetComponent<AudioSource>().enabled = false;
			other.transform.GetChild(0).gameObject.SetActive(false);

			if(other.GetComponent<GuardController>() != null){
				other.GetComponent<GuardController>().enabled = false;
			}
			if(other.GetComponent<GuardFieldOfView>() != null){
				other.GetComponent<GuardFieldOfView>().enabled = false;
			}
			if(other.GetComponent<NavMeshAgent>()){
				other.GetComponent<NavMeshAgent>().enabled = false;
			}
			if(other.transform.GetChild(1) != null)
				other.transform.GetChild(1).gameObject.SetActive(false);

			if(other.transform.GetChild(2) != null){
				other.transform.GetChild(2).gameObject.SetActive(false);
			}
		}
	}
}
