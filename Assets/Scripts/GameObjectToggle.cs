using UnityEngine;
using System.Collections;

public class GameObjectToggle : MonoBehaviour {
	public GameObject target;
	public GameObject watcher;
	public string watcherMessageOn;
	public string watcherMessageOff;
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

		if (watcher != null) {
			string watcherMessage = watcherMessageOn;
			if (!target.activeSelf) {
				watcherMessage = watcherMessageOff;
			}
			watcher.SendMessage("ObjectToggled", watcherMessage, SendMessageOptions.DontRequireReceiver);
		}
	}
}
