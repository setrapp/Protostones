using UnityEngine;
using System.Collections;

public class SimpleCamera : MonoBehaviour {
	public Camera targetCamera;
	public Transform lookAt;
	public float deadZone;
	public Vector2 mousePosition;
	public Vector2 startMousePosition;
	public Vector2 minLimits;
	public Vector2 maxLimits;
	public bool clampX;
	public bool clampY;
	public bool requireRMB;
	public Vector3 sensitivity;
	public float cameraDistance;
	private Vector3 desiredCameraPosition;


	void Start() {
		mousePosition.x = Mathf.Clamp(mousePosition.x, minLimits.x, maxLimits.x);
		mousePosition.y = Mathf.Clamp(mousePosition.y, minLimits.y, maxLimits.y);
		startMousePosition = mousePosition;
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
		if (!requireRMB || Input.GetMouseButton(1)) {
			if (Mathf.Abs(Input.GetAxis("Mouse X")) > deadZone) {
				mousePosition.x += Input.GetAxis("Mouse X") * sensitivity.x;
				if (clampX) {
					mousePosition.x = Helper.ClampAngle(mousePosition.x, minLimits.x, maxLimits.x);
				}
			}
			if (Mathf.Abs(Input.GetAxis("Mouse Y")) > deadZone) {
				mousePosition.y -= Input.GetAxis("Mouse Y") * sensitivity.y;
				if (clampY) {
					mousePosition.y = Helper.ClampAngle(mousePosition.y, minLimits.y, maxLimits.y);
				}
			}
		}
		targetCamera.transform.rotation = Quaternion.Euler(new Vector3(mousePosition.y, mousePosition.x, 0));
	}

	private void FindDesiredPosition() {
		desiredCameraPosition = lookAt.transform.position - targetCamera.transform.forward * cameraDistance;
	}

	private Vector3 CalculatePostion(float rotationX, float rotationY, float distance) {
		return Vector3.zero;
	}

	private void UpdatePostion() {
		targetCamera.transform.position = desiredCameraPosition;
	}

	public void Reset() {
		mousePosition.x = startMousePosition.x;
		mousePosition.y = startMousePosition.y;
	}
}