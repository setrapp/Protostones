using UnityEngine;
using System.Collections;

public class GameObjectToggle : MonoBehaviour {
	public GameObject target;
	public LayerMask layerOn;
	public LayerMask layerOff;

	void OnTriggerEnter(Collider collider) {
		int layerValue = (int)Mathf.Pow(2, collider.gameObject.layer);
		if ((layerValue & layerOn.value) == layerValue) {
			target.SetActive(true);
		} else if ((layerValue & layerOff.value) == layerValue) {
			target.SetActive(false);
		}
	}
}
