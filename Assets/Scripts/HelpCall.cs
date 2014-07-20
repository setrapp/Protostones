using UnityEngine;
using System.Collections;

public class HelpCall : MonoBehaviour {
	public GameObject escort;

	void Update() {
		if (Input.GetKeyDown(KeyCode.Space)) {
			audio.Play();
			escort.SendMessage("HelpEscortee", SendMessageOptions.DontRequireReceiver);
		}
	}
}