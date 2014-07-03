using UnityEngine;
using System.Collections;

public class ObjectFollow : MonoBehaviour {
	public GameObject leader;
	public float moveSpeed;
	public float followRadius;
	public float followTriggerDistance;
	public float followLeash;
	public enum FollowState {
		Waiting = 0,
		Following
	};
	public FollowState followState;

	void Update() {
		float toLeaderSqrDist = (transform.position - leader.transform.position).sqrMagnitude;
		if ((followState == FollowState.Waiting && toLeaderSqrDist <= followTriggerDistance * followTriggerDistance) ||
		    (followState == FollowState.Following && toLeaderSqrDist >= followRadius * followRadius))
		{
			if (followState == FollowState.Waiting) {
				followState = FollowState.Following;
			}
			MoveToObject();
		}
	}

	private void MoveToObject() {
		Vector3 targetPos = leader.transform.position;
		targetPos.z = transform.position.z;
		
		Vector3 toTarget = (targetPos - transform.position);
		float toTargetMag = toTarget.magnitude;
		if (toTargetMag > 0) {
			toTarget /= toTargetMag;
			if (toTargetMag > followLeash) {
				transform.position = leader.transform.position - (toTarget * followLeash);
			} else {
				float moveDist = moveSpeed * Time.deltaTime;
				if (moveDist > toTargetMag) {
					moveDist = toTargetMag;
				}
				toTarget = toTarget * moveDist;
				transform.position += toTarget;
			}
		}
	}
}
