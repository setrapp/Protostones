using UnityEngine;
using System.Collections;

public class SimpleMotor : MonoBehaviour {
	public float moveSpeed;
	public float hopSpeed;
	public float gravity;
	public Vector3 movement;
	private CharacterController controller;
	private SimpleCamera cameraController;

	void Start() {
		controller = GetComponent<CharacterController>();
		cameraController = GetComponent<SimpleCamera>();
	}

	public void UpdateMotor() {
		if (cameraController) {
			AlignWithCameraSnap();
		}
		ProcessMotion();
	}

	private void AlignWithCameraSnap() {
		if (movement.sqrMagnitude > 0) 
		{
			Vector3 rotation = cameraController.targetCamera.transform.rotation.eulerAngles;
			rotation.x = transform.rotation.eulerAngles.x;
			rotation.z = transform.rotation.eulerAngles.z;
			transform.rotation = Quaternion.Euler(rotation);
		}
	}

	private void ProcessMotion() {
		Vector3 worldMovement = Vector3.zero;
		if (movement.sqrMagnitude > 0) {
			worldMovement = transform.TransformDirection(movement);
		}
		if (worldMovement.sqrMagnitude > 1) {
			worldMovement.Normalize();
		}
		worldMovement *= moveSpeed * Time.deltaTime;
		worldMovement.y = 0;

		worldMovement.y -= gravity * Time.deltaTime;

		controller.Move(worldMovement);
	}

	public void Hop() {
		Vector3 hop = Vector3.up * hopSpeed * Time.deltaTime;
		controller.Move(hop);
	}
}
