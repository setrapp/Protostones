using UnityEngine;
using System.Collections;

public class HelpCall : MonoBehaviour {
	public GameObject escort;
	private SimpleMotor motor;

	void Start() {
		motor = GetComponent<SimpleMotor>();
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Space)) {
			audio.Play();
			escort.SendMessage("HelpEscortee", SendMessageOptions.DontRequireReceiver);
			motor.Hop();
		}
	}
}