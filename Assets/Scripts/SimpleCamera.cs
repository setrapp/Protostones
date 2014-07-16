using UnityEngine;
using System.Collections;

public class SimpleCamera : MonoBehaviour {
	public Camera targetCamera;
	public Transform lookAt;
	public float deadZone;
	public Vector3 mousePosition;
	public Vector3 startMousePosition;
	public Vector3 minLimits;
	public Vector3 maxLimits;
	public bool clampX;
	public bool clampY;
	public bool requireRMB;
	public Vector3 sensitivity;
	public float distanceSmooth;

	private float desiredDistance;
	private float distanceVelocity;


	void Start() {
		mousePosition.z = Mathf.Clamp (mousePosition.z, minLimits.z, maxLimits.z);
		startMousePosition.z = mousePosition.z;
		Reset();
	}

	void LateUpdate() {
		if (lookAt == null) {
			return;
		}

		HandlePlayerInput();
		FindDesiredPosition();
		UpdatePostion();
	}

	private void HandlePlayerInput() {
		// Camera Look.
		if (!requireRMB || Input.GetMouseButtonDown(1)) {
			if (Mathf.Abs(Input.GetAxis("Mouse X")) > deadZone) {
				mousePosition += Input.GetAxis("Mouse X") * sensitivity.x;
				if (clampX) {
					mousePosition.x = Helper.ClampAngle(mousePosition.x, minLimits.x, maxLimits.x);
				}
			}
			if (Mathf.Abs(Input.GetAxis("Mouse Y")) > deadZone) {
				mousePosition -= Input.GetAxis("Mouse Y") * sensitivity.x;
				if (clampX) {
					mousePosition.y = Helper.ClampAngle(mousePosition.y, minLimits.y, maxLimits.y);
				}
			}
		}

		// Camera Zoom.

	}

	private void FindDesiredPosition() {

	}

	private Vector3 CalculatePostion(float rotationX, float rotationY, float distance) {

	}

	private void UpdatePostion() {

	}

	public void Reset() {
		mousePosition.x = startMousePosition.x;
		mousePosition.y = startMousePosition.y;
		mousePosition.z = startMousePosition.z;
		desiredDistance = mousePosition.z;
	}

	private void 
}