using UnityEngine;
using System.Collections;

public class SimpleMotor : MonoBehaviour {
	public float moveSpeed;
	public Vector3 movement;
	private CharacterController controller;

	void Start() {
		controller = GetComponent<CharacterController>();
	}

	public void UpdateMotor() {
		//AlignWithCameraSnap();
		ProcessMotion();
	}

	private void AlignWithCameraSnap() {
		if (movement.sqrMagnitude > 0) 
		{
			Vector3 rotation = transform.rotation.eulerAngles;
			rotation.y = Camera.main.transform.rotation.y;
			transform.rotation = Quaternion.Euler(rotation);
		}
	}

	private void ProcessMotion() {
		Vector3 worldMovement = transform.TransformDirection(movement);
		if (worldMovement.sqrMagnitude > 1) {
			worldMovement.Normalize();
		}
		worldMovement *= moveSpeed * Time.deltaTime;
		controller.Move (worldMovement);
	}
}
