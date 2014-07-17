using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EscortController : MonoBehaviour {
	private SimpleMotor motor;
	public List<GameObject> criticalPath;
	public float criticalRange;
	private int criticalIndex;
	public List<GameObject> drainers;
	public int drainerIndex;
	public GameObject escortee;

	public float criticalWeight;
	public float drainWeight;
	public float protectWeight;

	void Start() {
		criticalIndex = 0;
		drainerIndex = -1;
		motor = GetComponent<SimpleMotor>();
	}

	void Update() {
		motor.movement = Vector3.zero;
		CalculateWeights();

		Vector3 criticalMove = CalculateCriticalDirection() * criticalWeight;
		Vector3 drainMove = CalculateDrainDirection() * drainWeight;
		Vector3 protectMove = CalculateProtectDirection() * protectWeight;

		motor.movement = criticalMove + drainMove + protectMove;
		motor.UpdateMotor();
	}

	private void CalculateWeights() {
		// Calculate importance of progressing along critical path.

		// Calculate importance of finding a drainer to restore condition.

		// Calculate importance of protecting escortee.

		NormalizeWeights();
	}

	private void NormalizeWeights() {
		float weightSumSqr = (criticalWeight * criticalWeight) + (drainWeight * drainWeight) + (protectWeight * protectWeight);
		if (weightSumSqr > 1) {
			float weightSum = Mathf.Sqrt(weightSumSqr);
			criticalWeight /= weightSum;
			drainWeight /= weightSum;
			protectWeight /= weightSum;
		}
	}

	private Vector3 CalculateCriticalDirection() {
		/*TODO Could probably do simple path following, or just check if object has moved beyond the half plane perpedicular to vector from last checkpoint to this one.*/
		if (criticalIndex < criticalPath.Count && (criticalPath[criticalIndex].transform.position - transform.position).sqrMagnitude < criticalRange * criticalRange) {
			criticalIndex++;
		}

		Vector3 criticalDirection = Vector3.zero;
		if (criticalIndex < criticalPath.Count) {
			criticalDirection = criticalPath[criticalIndex].transform.position - transform.position;
			criticalDirection.Normalize();
		} else {
			criticalWeight = 0;
		}

		return criticalDirection;
	}

	private Vector3 CalculateDrainDirection() {
		if (drainWeight > 0 && drainerIndex < 0) {
			float minSqrDist = 0;
			for (int i = 0; i < drainers.Count; i++) {
				float sqrDist = (drainers[i].transform.position - transform.position).sqrMagnitude;
				if (drainerIndex < 0 || sqrDist < minSqrDist) {
					minSqrDist = sqrDist;
					drainerIndex = i;
				}
			}
		}

		Vector3 drainDirection = Vector3.zero;
		if (drainerIndex >= 0) {
			drainDirection = drainers[drainerIndex].transform.position - transform.position;
			drainDirection.Normalize();
		}

		return drainDirection;
	}

	private Vector3 CalculateProtectDirection() {
		Vector3 protectDirection = escortee.transform.position - transform.position;
		protectDirection.Normalize();
		return protectDirection;
	}
}
