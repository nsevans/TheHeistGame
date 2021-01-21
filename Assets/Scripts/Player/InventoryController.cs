using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour {

	private GameObject[] inventory;										//A list of strings representing all items in the Player's inventory
	public int currentItem;											//The item the Player currently has in hand

	public GameObject flashlightItem;
	private FlashlightController flashlightController;

	public GameObject powerItem;
	private TimeChangeController powerController;

	public GameObject batonItem;
	private BatonController batonController;

	public GameObject emptyItem;
	private EmptyHandController emptyController;

	public Text equppedItemUI;

	// Use this for initialization
	void Start () {
		inventory = new GameObject[]{ emptyItem, flashlightItem, batonItem, powerItem };
		currentItem = 0;

		flashlightController = flashlightItem.GetComponent<FlashlightController>();
		flashlightController.SetFlashlight(false);

		powerController = powerItem.GetComponent<TimeChangeController>();

		batonController = batonItem.GetComponent<BatonController>();

		emptyController = emptyItem.GetComponent<EmptyHandController>();

		SetCurrentItem(inventory[currentItem]);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void UseInventoryItem(){
		GameObject equippedItem = inventory[currentItem];
		if(emptyItem == equippedItem){
			//print("Pick Up Guard");
			emptyController.PickUpGuard();
		}
		else if(flashlightItem == equippedItem){
			//print("Toggle flashlight");
			flashlightController.ToggleFlashlight();
		}
		else if(batonItem == equippedItem){
			//print("Swing baton");
			batonController.SwingBaton();
		}
		else if(powerItem == equippedItem){
			//print("Change time");
			powerController.ToggleTimeChange();
		}
	}

	public void SetCurrentItem(GameObject currentItem){
		foreach(GameObject item in inventory){
			if(item == currentItem){
				item.SetActive(true);
				equppedItemUI.text = item.name;
				if(currentItem == flashlightItem){
					flashlightItem.transform.GetChild(0).GetComponent<Light>().enabled = false;
				}
			}else{
				item.SetActive(false);
			}
		}
	}

	public int GetCurrentItemIndex(){
		return currentItem;
	}

	public void SetCurrentItemIndex(int index){
		currentItem = index;
	}

	public GameObject[] GetInventory(){
		return inventory;
	}
}
