using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.AI;

public class DoorController : MonoBehaviour {

	public bool locked;
	[HideInInspector]
	public bool navMeshUpdated;

	private Animator animator;

	public AudioClip openDoor;
	public AudioClip closeDoor;
	public AudioClip openAndCloseDoor;
	public AudioClip lockedDoor;
	private AudioSource audioSource;

	void Awake(){
		animator = GetComponent<Animator>();
		audioSource = GetComponent<AudioSource>();
		audioSource.volume = 0.5f;
		navMeshUpdated = false;
	}

	void Update(){
		if(!navMeshUpdated || !gameObject.name.Contains("Changable")){
			UpdateDoorNavMesh(locked);
			navMeshUpdated = true;
		}
	}

	public void OpenDoor(){
		if(!locked && !animator.GetCurrentAnimatorStateInfo(0).IsName("OpenDoor")){
			animator.SetBool("isOpen", true);
			audioSource.PlayOneShot(openDoor);
		}else{
			audioSource.PlayOneShot(lockedDoor);
		}
	}

	public void CloseDoor(){
		if(!animator.GetCurrentAnimatorStateInfo(0).IsName("CloseDoor")){
			animator.SetBool("isOpen", false);
			audioSource.PlayOneShot(closeDoor);
		}
	}

	public void OpenAndCloseDoor(){
		if(!animator.GetCurrentAnimatorStateInfo(0).IsName("OpenAndCloseDoor")){
			animator.Play("OpenAndCloseDoor");
			audioSource.PlayOneShot(openAndCloseDoor);

		}
	}

	public bool IsOpen(){
		return animator.GetBool("isOpen");
	}

	public void ToggleDoor(){
		bool isOpen = animator.GetBool("isOpen");
		if(isOpen){
			CloseDoor();
		}else{
			OpenDoor();
		}
	}

	public void ToggleLock(){
		locked = !locked;
	}

	public void PlayOpenDoor(){
		audioSource.PlayOneShot(openDoor);
	}

	public void PlayCloseDoor(){
		audioSource.PlayOneShot(closeDoor);
	}

	void UpdateDoorNavMesh(bool isLocked){
		foreach(Transform child in transform){
			child.GetComponent<NavMeshObstacle>().carving = isLocked;
		}
	}
}
