using UnityEngine;
using System.Collections;

public class GameObjectToggle : MonoBehaviour {
	public GameObject target;
	public GameObject watcher;
	public string watcherMessageOn;
	public string watcherMessageOff;
	public LayerMask layerOn;
	public LayerMask layerOff;
	private GameObject collidingObjectOn = null;
	private GameObject collidingObjectOff = null;
	private bool toggleOn;
	public bool favorOn;

	void OnTriggerStay(Collider collider) {
		int layerValue = (int)Mathf.Pow(2, collider.gameObject.layer);
		if ((layerValue & layerOn.value) == layerValue) {
			collidingObjectOn = collider.gameObject;

		} else if ((layerValue & layerOff.value) == layerValue) {
			collidingObjectOff = collider.gameObject;
		}
	}

	void Update() {
		bool notify = false;
		if (collidingObjectOn != null && collidingObjectOff != null) {
			collidingObjectOn = null;
			collidingObjectOff = null;
			target.SetActive(favorOn);
			toggleOn = favorOn;
			notify = true;
		}
		else if (collidingObjectOn != null && !target.activeSelf) {
			collidingObjectOn = null;
			target.SetActive(true);
			toggleOn = true;
			notify = true;
		} else if (collidingObjectOff != null && target.activeSelf) {
			collidingObjectOff = null;
			target.SetActive(false);
			toggleOn = false;
			notify = true;
		}
		
		if (watcher != null && notify) {
			string watcherMessage = watcherMessageOn;
			if (!target.activeSelf) {
				watcherMessage = watcherMessageOff;
			}
			watcher.SendMessage("ObjectToggled", watcherMessage, SendMessageOptions.DontRequireReceiver);
		}
	}

	void OnTriggerExit(Collider collider) {
		/*if (collider.gameObject == collidingObject) {
			target.SetActive(toggleOn);
		}*/

		/*if (watcher != null) {
			string watcherMessage = watcherMessageOn;
			if (!target.activeSelf) {
				watcherMessage = watcherMessageOff;
			}
			watcher.SendMessage("ObjectToggled", watcherMessage, SendMessageOptions.DontRequireReceiver);
		}*/
	}
}
