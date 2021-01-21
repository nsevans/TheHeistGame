using System.Collections;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	private Camera mainCamera;											//The camera above the Player's head

	public float rotateSpeed = 7.0f;

	private float moveSpeed;											//The speed multiplyer of the Player when they're walking, running or crouching
	[SerializeField]
	private readonly float WALK_MULT = 1.4f;							//The speed multiplyer of the Player when they're walking
	[SerializeField]
	private readonly float RUN_MULT = 2.5f;								//The speed multiplyer of the Player when they're running
	[SerializeField]
	private readonly float CROUCH_MULT = 0.9f;							//The speed multiplyer of the Player when they're crouching
	private readonly float BACK_MULT = 0.75f;							//The speed multiplyer of the Player when they're moving backwards
	private readonly float LEFT_MULT = 0.95f;							//The speed multiplyer of the Player when they're moving left
	private readonly float RIGHT_MULT = 0.95f;							//The speed multiplyer of the Player when they're moving right
	private Vector3 movement;
	private Rigidbody rb;												//The player's rigidbody

	public InventoryController inventoryController;

	public AudioClip[] footsteps;
	private float nextStepTime;
	private float stepCycle;
	private float stepInterval;

	// Use this for initialization
	void Start () {
		mainCamera = FindObjectOfType<Camera>();
		
		moveSpeed = WALK_MULT;
		movement = new Vector3();
		rb = GetComponent<Rigidbody>();

		stepCycle = 0f;
		stepInterval = 1f;
		nextStepTime = stepCycle + stepInterval;
	}

	// Update is called once per frame
	void Update () {
		RotatePlayer();
		//Interactions
		if(Input.GetKeyDown(KeyCode.E)){
			Interact();
		}



		float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
		if(mouseWheel > 0f && !inventoryController.emptyItem.GetComponent<EmptyHandController>().isCarrying){	//Scroll Up
			int currentItem = inventoryController.GetCurrentItemIndex();
			currentItem = (currentItem + 1) % inventoryController.GetInventory().Length;
			inventoryController.SetCurrentItem(inventoryController.GetInventory()[currentItem]);
			inventoryController.SetCurrentItemIndex(currentItem);
		}else if(mouseWheel < 0f && !inventoryController.emptyItem.GetComponent<EmptyHandController>().isCarrying){	//Scroll Down
			int currentItem = inventoryController.GetCurrentItemIndex();
			currentItem = ((currentItem - 1) % inventoryController.GetInventory().Length + inventoryController.GetInventory().Length) % inventoryController.GetInventory().Length;
			inventoryController.SetCurrentItem(inventoryController.GetInventory()[currentItem]);
			inventoryController.SetCurrentItemIndex(currentItem);
		}

		if(Input.GetMouseButtonDown(0)){
			inventoryController.UseInventoryItem();
		}
	}

	void FixedUpdate(){
		MovePlayer();
		if(movement != Vector3.zero){
			if(moveSpeed == WALK_MULT){
				stepCycle += (WALK_MULT) * Time.deltaTime;
				GetComponent<AudioSource>().volume = 0.5f;
			}else if(moveSpeed == RUN_MULT){
				stepCycle += (RUN_MULT) * Time.deltaTime;
				GetComponent<AudioSource>().volume = 0.8f;
			}else{//moveSpeed = CROUCH_MULT
				stepCycle += (CROUCH_MULT) * Time.deltaTime;
				GetComponent<AudioSource>().volume = 0.2f;
			}
		}
		if(!(stepCycle < nextStepTime)){
			nextStepTime = stepCycle + stepInterval;
			PlayWalkingSound();
		}
	}

	void LateUpdate(){
		UpdateCameraPosition();
	}

	/*
	 * Move the Player forwar, backward, left or right in relation to the direction the Player is currently facing
	 */
	void MovePlayer(){
		movement = new Vector3();
		//Player speed (Running, Crouching/Sneaking, and Walking)
		if(Input.GetKey(KeyCode.LeftShift)){
			moveSpeed = RUN_MULT;
		}else if(Input.GetKey(KeyCode.LeftControl)){
			moveSpeed = CROUCH_MULT;
		}else if(!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl)){
			moveSpeed = WALK_MULT;
		}
		if(Input.GetKey(KeyCode.W)){
			movement += transform.TransformDirection(Vector3.forward * moveSpeed);
		}
		if(Input.GetKey(KeyCode.S)){
			movement += transform.TransformDirection(Vector3.back* BACK_MULT);
		}
		if(Input.GetKey(KeyCode.A)){
			movement += transform.TransformDirection(Vector3.left * LEFT_MULT * moveSpeed);
		}
		if(Input.GetKey(KeyCode.D)){
			movement += transform.TransformDirection(Vector3.right * RIGHT_MULT * moveSpeed);
		}
		rb.velocity = movement;
	}

	/*
	 * Rotate the direction the Player is facing with the mouse
	 */
	void RotatePlayer(){
		Vector3 mousePos = Input.mousePosition;
		Vector3 playerPos = Camera.main.WorldToScreenPoint(transform.position);
		Vector3 dir = mousePos - playerPos;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(-angle+90f, Vector3.up);
	}

	void Interact(){
		RaycastHit hit;
		if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 1f)){
			if(hit.collider.tag == "Door"){
				OpenDoor(hit.collider);
			}else if(hit.collider.tag == "Enemy Knocked Out"){
				//Drag enemy
				//print("Hit");
				Vector3 playerPos = transform.position;
				playerPos.x = playerPos.x - transform.localScale.x;
				hit.transform.position = playerPos;
			}
		}
	}

	void OnTriggerEnter(Collider other){
		if(other.tag == "Ceiling"){
			SetOpacity(other, 0f);
		}
	}

	void OnTriggerExit(Collider other){
		if(other.tag == "Ceiling"){
			SetOpacity(other, 1f);
		}
	}

	//Change the opacity of an object
	public void SetOpacity(Collider other, float opacity){
		Color color = other.GetComponent<Renderer>().material.color;
		color.a = opacity;
		other.GetComponent<Renderer>().material.color = color;
	}

	/*
	 * Animate the door to open or to close
	 */
	void OpenDoor(Collider other){
			other.GetComponent<DoorController>().ToggleDoor();
	}

	/*
	 * Move the camera with the Player
	 */
	void UpdateCameraPosition(){
		Vector3 newPos = new Vector3(transform.position.x, mainCamera.transform.position.y, transform.position.z);
		mainCamera.transform.position = newPos;
	}

	void PlayWalkingSound(){
		int n = Random.Range(1, footsteps.Length);
		GetComponent<AudioSource>().PlayOneShot(footsteps[n]);
		AudioClip temp = footsteps[n];
		footsteps[n] = footsteps[0];
		footsteps[0] = temp;
	}

}