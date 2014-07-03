using UnityEngine;
using System.Collections;

public class DragFollow : MonoBehaviour {
	public bool clickToFollow = false;
	private bool followingMouse = false;
	public float moveSpeed;

	void Update() {
		//--- Follow when mouse is down anywhere ---//
		if (!clickToFollow) {
			if (Input.GetMouseButtonDown(0)) {
				followingMouse = true;
			} else if (Input.GetMouseButtonUp(0)) {
				followingMouse = false;
			}
		}

		if (followingMouse) {
			MoveToMouse();
		}
	}

	//--- Follow when clicking object ---//
	void OnMouseDown() {
		if (clickToFollow) {
			if (!followingMouse) {
				followingMouse = true;
			}
		}
	}
	void OnMouseUp() {
		followingMouse = false;
	}

	private void MoveToMouse() {
		Vector3 mousePos = Input.mousePosition;
		mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z;
		mousePos = Camera.main.ScreenToWorldPoint(mousePos);

		Vector3 toMouse = (mousePos - transform.position);
		float toMouseMag = toMouse.magnitude;
		float moveDist = moveSpeed * Time.deltaTime;
		if (moveDist > toMouseMag) {
			moveDist = toMouseMag;
		} 
		toMouse = toMouse * moveDist;
		transform.position += toMouse;
	}
}
