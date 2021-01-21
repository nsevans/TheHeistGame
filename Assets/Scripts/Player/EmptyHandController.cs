using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyHandController : MonoBehaviour {

	[HideInInspector]
	public bool isCarrying = false;
	private Transform carriedObject;											//How far the Player can reach to pick up a Guard
	public float reach = 0.4f;

	public LevelController levelController;

	public void PickUpGuard(){
		RaycastHit hit;
		if(!isCarrying){
			if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, reach)){
				Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward), Color.red);
				if(hit.transform.tag == "Enemy KOd"){
					//Pick up guard
					hit.transform.position = new Vector3(transform.position.x, transform.position.y + transform.localScale.y, transform.position.z);
					hit.transform.parent = gameObject.transform;
					carriedObject = hit.transform;
					isCarrying = true;
				}else if(hit.transform.tag == "Gold"){
					//print("Grabbed gold");
					hit.transform.gameObject.SetActive(false);
				}
			}
		}else{
			//Drop Guard
			//print("Dropping Guard");
			carriedObject.position = new Vector3(transform.position.x + (transform.forward.x* 0.2f), transform.position.y, transform.position.z + (transform.forward.z * 0.2f));
			carriedObject.parent = null;
			isCarrying = false;

		}
	}
}
