using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography.X509Certificates;

public class CameraController : MonoBehaviour {

	private CameraFieldOfView fovController;
	private Collider[] targetsInRadius;
	private List<Transform> targetsInView;

	private Transform swivel;
	[Range(0,180)]
	public float rotationAmount = 40;
	public float rotationSpeed = 1;

	private bool scanning;
	private readonly string S_STATE = "scanning";
	private bool alerting;
	private readonly string A_STATE = "alerting";

	private Vector3 targetPos;

	// Use this for initialization
	void Start () {
		fovController = GetComponentInChildren<CameraFieldOfView>();
		targetsInRadius = new Collider[0];
		swivel = transform.GetChild(0);

		SetCameraState(S_STATE);
	}

	// Update is called once per frame
	void Update () {
		CheckForPlayerInView();
		if(scanning){
			//print("Scanning");
			RotateCamera();
		}else if(alerting){
			//print("Alerting");
			AlertNearbyEnemies();
			float targetPosX = targetPos.x;
			float targetPosZ = targetPos.z;
			targetPos = new Vector3(targetPosX, swivel.position.y, targetPosZ);
			swivel.LookAt(targetPos);
		}
	}

	void RotateCamera(){
		//swivel.localRotation = Quaternion.Euler(0, ((Mathf.Sin(Time.time) * rotationAmount)) + (swivel.eulerAngles.y/360), 0) * rotationSpeed;
		swivel.localRotation = Quaternion.Euler(0, (Mathf.Sin(Time.time * rotationSpeed) * rotationAmount) + (swivel.eulerAngles.y/360), 0);
	}

	void CheckForPlayerInView(){
		targetsInView = fovController.GetTargetsInView();
		foreach(Transform target in targetsInView){
			if(target.tag == "Player"){
				SetCameraState(A_STATE);
				return;
			}
		}
		SetCameraState(S_STATE);
	}

	/*void AlertNearbyEnemies(){
		enemies = fovController.GetTargetsInRadius();
		foreach(Collider collider in enemies){
			if(collider.tag == "Player"){
				targetPos = collider.transform.position;
				foreach(Collider enemy in enemies){
					if(enemy.tag == "Enemy"){
						enemy.GetComponent<GuardController>().SearchForPlayerNearCamera(targetPos);
					}
				}
				break;
			}
		}
	}*/
	void AlertNearbyEnemies(){
		targetsInRadius = fovController.GetTargetsInRadius();
		foreach(Collider player in targetsInRadius){
			if(player.tag == "Player"){
				targetPos = player.transform.position;
				break;
			}
		}
		foreach(Collider enemy in targetsInRadius){
			if(enemy.tag == "Enemy"){
				enemy.GetComponent<GuardController>().SearchForPlayerNearCamera(targetPos);
			}
		}
	}

	void SetCameraState(string state){
		if(state.Equals(S_STATE)){
			scanning = true;
			alerting = false;
		}else if(state.Equals(A_STATE)){
			scanning = false;
			alerting = true;
		}
	}
}
