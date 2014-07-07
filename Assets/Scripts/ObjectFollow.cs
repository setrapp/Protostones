using UnityEngine;
using System.Collections;

public class ObjectFollow : MonoBehaviour {
	public SimpleMover leader;
	public float followRadius;
	public float followTriggerDistance;
	public float followLeash;
	public float distanceWhenFound;
	private DragFollow dragFollow;
	public enum FollowState {
		Waiting = 0,
		Busy,
		OffLeash,
		Following
	};
	public FollowState followState;
	public SimpleMover mover;

	void Start() {
		dragFollow = GetComponent<DragFollow>();
		mover = GetComponent<SimpleMover>();
		mover.moving = false;
	}

	void Update() {
		mover.moving = false;

		// Only attempt to follow leader if not busy following something else.
		if (followState != FollowState.Busy && dragFollow && dragFollow.followingMouse) {
			followState = FollowState.Busy;
		} else if (!dragFollow || !dragFollow.followingMouse) {
			float toLeaderSqrDist = (transform.position - leader.transform.position).sqrMagnitude;

			// If object is no longer busy, or is waiting and is within range of leader, start following without leash.
			if (followState == FollowState.Busy ||
			    (followState == FollowState.Waiting && toLeaderSqrDist <= followTriggerDistance * followTriggerDistance))
			{
				followState = FollowState.OffLeash;
				distanceWhenFound = Mathf.Sqrt(toLeaderSqrDist);
			}

			// If object is off leash, follow and stay within distance originally between object and leader.
			// Otherwise, start following closely.
			if (followState == FollowState.OffLeash) {
				if (toLeaderSqrDist >= followRadius * followRadius) {
					MoveToObject(distanceWhenFound);
				} else {
					followState = FollowState.Following;
				}
			}

			// If following closely, stay on prescribed leash.
			if (followState == FollowState.Following)
			{
				MoveToObject(followLeash);
			}
		}
	}

	private void MoveToObject(float leashLength) {
		// Target leader position to follow.
		Vector3 targetPos = leader.transform.position;
		targetPos.z = transform.position.z;
		Vector3 toTarget = (targetPos - transform.position);
		float toTargetMag = toTarget.magnitude;
		if (toTargetMag > 0) {
			toTarget /= toTargetMag;
			// Actually follow slightly behind the leader.
			targetPos = targetPos - (toTarget * followRadius);
			toTargetMag = (targetPos - transform.position).magnitude;

			float moveDist = mover.maxSpeed;
			if (toTargetMag > leashLength) {
				moveDist = leader.currentSpeed;
			}
			if (moveDist > toTargetMag) {
				moveDist = toTargetMag;
			}
			mover.Move(toTarget, moveDist, false);
		}
		mover.moving = true;
	}
}
