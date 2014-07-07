using UnityEngine;
using System.Collections;

public class DragFollow : MonoBehaviour {
	public bool clickToFollow = false;
	public bool followingMouse = false;
	public float speedRampRadius;
	public SimpleMover mover;
	public float damageSlowDown; /*TODO Makes more sense connected to spreader*/

	void Start() {
		mover = GetComponent<SimpleMover>();
		mover.moving = false;
	}

	void Update() {
		mover.moving = false;

		//Follow when mouse is down anywhere
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
		mover.externalMultiplier = 1;
	}

	void TakeDamage() {
		mover.externalMultiplier = damageSlowDown;
	}

	//Follow when clicking object
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
		if (toMouseMag > 0) {
			toMouse /= toMouseMag;
			float speedRamp = 1;
			if (speedRampRadius > 0) {
				speedRamp = toMouseMag / speedRampRadius;
			}
			float moveDist = mover.maxSpeed * speedRamp;
			if (moveDist > toMouseMag) {
				moveDist = toMouseMag;
			} 
			mover.Move(toMouse, moveDist, true);
		}

		mover.moving = true;
	}
}
