using UnityEngine;
using System.Collections;

public class SimpleMoveInput : MonoBehaviour {
	public float deadSpace;
	private SimpleMotor motor;

	void Start() {
		motor = GetComponent<SimpleMotor>();
	}

	void Update() {
		motor.movement = Vector3.zero;
		if (Mathf.Abs(Input.GetAxis("Vertical")) > deadSpace) {
			motor.movement += Vector3.forward * Input.GetAxis("Vertical");
		}
		if (Mathf.Abs(Input.GetAxis("Horizontal")) > deadSpace) {
			motor.movement += Vector3.right * Input.GetAxis("Horizontal");
		}

		motor.UpdateMotor();
	}
}
