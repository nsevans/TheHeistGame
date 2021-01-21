using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.IO;

public class GuardController : MonoBehaviour {

	public Transform[] points;								//Points that the guard will navigate to through the Navigation Mesh
	private int destPoint = 0;								//The current destination point that the Guard is navigating to
	private NavMeshAgent agent;								//The Navigation Mesh Agent

	private GameObject player;								//The Player Game Object
	private Vector3 playersLastKnownPos;					//The Player's last known position, used for when the Guard is searching for or chasing the Player
	private Vector3 target;

	private GuardFieldOfView fovController;					//The script used to draw the field of view of the guard and collects the collisions in the field of view
	private List<Transform> targetsInView;					//A list of transforms of the Guards and Player in the field of view of the Guard
	private Collider[] targetsInRadius;						//A list of colliders of the Guards and Player in the radius of the guard

	public LevelController levelController;					

	//States of the Guard, only one is true at a time
	private bool patrolling;								//True when the Guard is walking from destination point to destination point
	public readonly string P_STATE = "patrolling";			
	private bool chasing;									//True when the Guard is chasing the Player and the Player is in the guard's field of view
	public readonly string C_STATE = "chasing";
	private bool searching;									//True when the Guard was chasing the Player and the Player is no longer in the Guard's field of view
	public readonly string S_STATE = "searching";

	private float maxSearchTime;							//The maximum time the Guard will spend searching for the Player before returning back to their patrolling state
	private float currentSearchTime;						//Tracks how long the Guard has been searching for

	public float patrolSpeed = 1f;							//The speed of the Guard while it's in the patrolling state
	private Quaternion startRotation;						//The starting rotation of the guard, used when the guard stands in one place;

	public float chaseSpeed = 3f;							//The speed of the Guard while it's in the chasing state
	public float chaseRotationSpeed = 7f;					//The rotation speed of the Guard while it's chasing the Player

	public float searchSpeed = 2f;							//The speed of the Guard while it's in the searching state
	private bool isRotating;								//True when the Coroutine RotateGuard() is rotating
	public float searchRotateSpeed = 4f;					//The rotation speed of the Guard while it's are searching for the Player

	public float stoppingDistance = 0.2f;					//The distance the Guard stops from it's target when in the searching state
	private bool searchingForPlayerNearGuard = false;

	public AudioClip[] footsteps;
	private float nextStepTime;
	private float stepCycle;
	private float stepInterval;

	void Awake(){
		startRotation = transform.rotation;
	}

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent>();
		
		player = GameObject.FindGameObjectWithTag("Player");
		target = new Vector3();

		fovController = GetComponent<GuardFieldOfView>();

		maxSearchTime = 8f;
		currentSearchTime = 0f;

		isRotating = false;

		stepCycle = 0f;
		stepInterval = 1f;
		nextStepTime = stepCycle + stepInterval;

		SetCurrentState(P_STATE);
	}
	
	// Update is called once per frame
	void Update () {
		if(levelController.GetCurrentState() == levelController.P_STATE){
		//	print(GetCurrentState());
			CheckForPlayerInView();						//Check if the Player enters the Guard's view every frame
			CheckForward();								//Check directly infront of the Guard to perform actions depending on what's infront of the Guard
			if(patrolling){								//If the Guard is in the patrolling state
				PatrolArea();
				//ResetSearchTime();
			}else if(chasing){							//If the Guard is in the chasing state
				ChasePlayer();
				//ResetSearchTime();
			}else if(searching){						//If the Guard is in the searching state
				SearchForPlayer(target);
				UpdateTimeSpentSearching();
			}	
		}
		if(levelController.GetCurrentState() == levelController.C_STATE){
			SetCurrentState(P_STATE);
			StopCoroutine(RotateGuard());
			playersLastKnownPos = points[destPoint].position;
			agent.destination = points[destPoint].position;
			target = points[destPoint].position;

			destPoint = (destPoint + 1) % points.Length;
			currentSearchTime = maxSearchTime + 1;
		}
	}

	void FixedUpdate(){
		if(agent.velocity != Vector3.zero){
			if(agent.speed == patrolSpeed){
				stepCycle += (patrolSpeed) * Time.deltaTime;
				GetComponent<AudioSource>().volume = 0.4f;
			}else if(agent.speed == chaseSpeed){
				stepCycle += (2f * chaseSpeed) * Time.deltaTime;
				GetComponent<AudioSource>().volume = 0.8f;
			}else{//agent.speed == searchSpeed
				stepCycle += (1.5f * searchSpeed) * Time.deltaTime;
				GetComponent<AudioSource>().volume = 0.6f;
			}
		}
		if(!(stepCycle < nextStepTime)){
			nextStepTime = stepCycle + stepInterval;
			PlayWalkingSound();
		}
	}

	/*
	 * Check directly infront of the Guard with a Raycast to see if there is an object
	 */
	void CheckForward(){
		RaycastHit hit;
		if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 0.2f)){
			if(hit.transform.tag == "Player"){																//If the Raycast hits the Player																			//The Player is caught and Game Over
				levelController.SetLevelState(levelController.C_STATE);
			}
		}else if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 1)){
			if(hit.transform.tag == "Door" && !hit.transform.GetComponent<DoorController>().IsOpen()){				//If the Raycast hits a door that is closed
				if(!hit.transform.gameObject.GetComponent<DoorController>().locked){								//If the door is unlocked
					if(patrolling){																					//If the Guard is patrolling
						hit.transform.GetComponent<DoorController>().OpenAndCloseDoor();							//The Guard opens and then closes the door
					} else{																							//If the Guard is searching or chasing
						hit.transform.GetComponent<DoorController>().OpenDoor();									//The Guard will only open the door
					}
				}
			}
		}else if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 4)){ 
			if(hit.transform.tag == "Enemy KOd" && patrolling){
				target = hit.transform.position;
				//stoppingDistance = 2.0f;
				searchingForPlayerNearGuard = true;
				SetCurrentState(S_STATE);
			}
		}
	}

	/*
	 * When the player is in the enemy's view, the enemy will follow the player
	 */
	void ChasePlayer(){
		//print("Chasing Player");
		agent.speed = chaseSpeed;
		playersLastKnownPos = player.transform.position;
		agent.destination = playersLastKnownPos;
		Quaternion playerRotation = Quaternion.LookRotation(playersLastKnownPos - transform.position);

		transform.rotation = Quaternion.Slerp(transform.rotation, playerRotation, Time.deltaTime * chaseRotationSpeed);

	}

	//Search the immediate area for the player
	void SearchForPlayer(Vector3 moveTowardsTarget){
		//print("Searching for Player");
		agent.speed = searchSpeed;
		if(!searchingForPlayerNearGuard && agent.remainingDistance < stoppingDistance){
			agent.destination = moveTowardsTarget;
		}
		if(!agent.pathPending && agent.remainingDistance < stoppingDistance){
			StartCoroutine(RotateGuard());
		}
	}

	IEnumerator RotateGuard(){
		agent.destination = transform.position;
		if(isRotating){
			yield break;
		}
		isRotating = true;

		int turnDirection = (UnityEngine.Random.value > 0.5f)? -1 : 1;

		Vector3 newRot = transform.eulerAngles + (turnDirection * new Vector3(0, 180, 0));
		Vector3 currentRot = transform.eulerAngles;

		float counter = 0;
		float duration = maxSearchTime/searchRotateSpeed;
		while(counter < duration && GetCurrentState() == S_STATE){
			counter += Time.deltaTime;
			transform.eulerAngles = Vector3.Lerp(currentRot, newRot, Mathf.Sin(counter/duration)*1.2f);
			yield return null;
		}

		turnDirection = (UnityEngine.Random.value > 0.5f)? -1 : 1;

		newRot = transform.eulerAngles + (turnDirection * new Vector3(0, 359, 0));
		currentRot = transform.eulerAngles;

		counter = 0;
		duration = maxSearchTime/(searchRotateSpeed/2);
		while(counter < duration && GetCurrentState() == S_STATE){
			counter += Time.deltaTime;
			transform.eulerAngles = Vector3.Lerp(currentRot, newRot, Mathf.Sin(counter/duration)*1.2f);
			yield return null;
		}
		isRotating = false;
	}
		

	//Navigate to the destination set by the Nav Mesh Agent
	public void PatrolArea(){
		agent.speed = patrolSpeed;
		if(points.Length == 1){
			if(Vector3.Distance(transform.position, points[0].position) < 0.5f){
				if(Mathf.Abs(Quaternion.Angle(transform.rotation, startRotation)) > 1e-3f){
					transform.rotation = Quaternion.Lerp(transform.rotation, startRotation, Time.deltaTime * chaseRotationSpeed);
				}
				return;
			}else if(Vector3.Distance(transform.position, points[0].position) >= 0.5f){
				agent.destination = points[0].position;
				return;
			}
		}
		else if(!agent.pathPending && agent.remainingDistance < 0.2f){
			//print("Patrolling area");
			if(points.Length == 0){
				return;
			}
			agent.destination = points[destPoint].position;
			destPoint = (destPoint + 1) % points.Length;
		}
	}

	//When the enemy collides with the player
	void OnTriggerEnter(Collider other){
		if(other.tag == "Player"){
			transform.LookAt(other.transform);
		}
	}

	//Set the current State based on what the enemy can and cannot see
	void CheckForPlayerInView(){
		List<Transform> targetsInView = fovController.GetTargetsInView();
		bool playerInView = false;
		foreach(Transform target in targetsInView){
			if(target.tag == "Player"){
				playerInView = true;
			}
		}
		if(playerInView){
			SetCurrentState(C_STATE);
			ResetSearchTime();
		}else if(!playerInView && chasing){
			target = playersLastKnownPos;
			//stoppingDistance = 2.0f;
			searchingForPlayerNearGuard = false;
			SetCurrentState(S_STATE);
			ResetSearchTime();
		}
		if(!playerInView && searching && currentSearchTime > maxSearchTime){
			StopCoroutine(RotateGuard());
			SetCurrentState(P_STATE);
		}
	}

	void UpdateTimeSpentSearching(){
		currentSearchTime += Time.deltaTime;
	}

	void ResetSearchTime(){
		currentSearchTime = 0;
	}

	public void SetCurrentState(string state){
		if(state.Equals("patrolling")){
			patrolling = true;
			chasing = false;
			searching = false;
		}else if(state.Equals("chasing")){
			patrolling = false;
			chasing = true;
			searching = false;
		}else if(state.Equals("searching")){
			patrolling = false;
			chasing = false;
			searching = true;
		}
	}

	public string GetCurrentState(){
		if(patrolling){
			return P_STATE;
		}
		else if(chasing){
			return C_STATE;
		}
		else{	//In Patrolling State
			return S_STATE;
		}
	}

	public void SearchForPlayerNearCamera(Vector3 target){
		playersLastKnownPos = target;
		StopCoroutine(RotateGuard());
		SetCurrentState(S_STATE);
	}

	void PlayWalkingSound(){
		int n = Random.Range(1, footsteps.Length);
		GetComponent<AudioSource>().PlayOneShot(footsteps[n]);
		AudioClip temp = footsteps[n];
		footsteps[n] = footsteps[0];
		footsteps[0] = temp;
	}

}