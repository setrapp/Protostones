using UnityEngine;
using System.Collections;

public class GameObjectToggle : MonoBehaviour {
	public GameObject target;
	public LayerMask layerOn;
	public LayerMask layerOff;
	private int layerOnIndex = -1;
	private int layerOffIndex= -1;

	void Start() {
		// Convert layermask value to power of 2 index. 
		// Is there not an automatic way of doing this???
		int value = 1;
		int pow = 0;
		do {
			if (value == layerOn.value) {
				layerOnIndex = pow;
			}
			if (value == layerOff.value) {
				layerOffIndex = pow;
			}
			value *= 2;
			pow++;
		} while (pow < 32 && (layerOnIndex < 0 || layerOffIndex < 0));
	}

	void OnTriggerEnter(Collider collider) {
		if (collider.gameObject.layer == layerOnIndex) {
			target.SetActive(true);
		} else if (collider.gameObject.layer == layerOffIndex) {
			target.SetActive(false);
		}
	}
}
