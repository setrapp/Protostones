using UnityEngine;
using System.Collections;

public class GameObjectToggle : MonoBehaviour {
	public GameObject target;
	public LayerMask layerOn;
	public LayerMask layerOff;
	private GameObject collidingObject = null;
	private bool toggleOn;

	void OnTriggerEnter(Collider collider) {
		int layerValue = (int)Mathf.Pow(2, collider.gameObject.layer);
		if ((layerValue & layerOn.value) == layerValue && !target.activeSelf) {
			collidingObject = collider.gameObject;
			toggleOn = true;
		} else if ((layerValue & layerOff.value) == layerValue && target.activeSelf) {
			collidingObject = collider.gameObject;
			toggleOn = false;
		}
	}

	void OnTriggerExit(Collider collider) {
		if (collider.gameObject == collidingObject) {
			target.SetActive(toggleOn);
		}
	}
}
