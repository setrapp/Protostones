using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EscortController : MonoBehaviour {
	private SimpleMotor motor;
	private HealthTracker health;
	public List<GameObject> criticalPath;
	public float criticalRange;
	private int criticalIndex;
	public List<GameObject> drainers;
	public int drainerIndex;
	public List<GameObject> escorteeRefuges;
	private int escorteeRefugeIndex;
	public GameObject escortee;
	private HealthTracker escorteeHealth;
	public float choiceDelay;
	private float lastChoice = -1;
	private System.Random random;
	public int seed = 0;
	private Vector3 lastMove;
	public float decisiveness = 1;
	public float stubornness = 1;

	// Macro weights.
	public float criticalWeight;
	public float drainWeight;
	public float protectWeight;
	public float retreatWeight;

	// Micro weights.
	public float criticalBaseWeight;
	public float criticalTimeWeight;
	public float drainBaseWeight;
	public float drainEscorteeWeight;
	public float drainEscortWeight;
	public float protectBaseWeight;
	public float protectEscorteeWeight;
	public float protectEscortWeight;
	public float protectDistanceWeight;
	public float retreatBaseWeight;
	public float retreatEscorteeWeight;
	public float retreatDistanceWeight;
	public float retreatEscortWeight;

	// Weight helpers.
	public float elapsedTime;
	public float maxTime;
	public float minPrioritizeDistance;
	public float maxPrioritizeDistance;

	void Start() {
		if (seed <= 0) {
			seed = (int)System.DateTime.Now.Ticks;
		}
		random = new System.Random(seed);

		criticalIndex = 0;
		drainerIndex = -1;
		escorteeRefugeIndex = -1;
		motor = GetComponent<SimpleMotor>();
		health = GetComponent<HealthTracker>();
		escorteeHealth = escortee.GetComponent<HealthTracker>();
		elapsedTime = 0;

		//TODO Make separate list for refuges.
		escorteeRefuges = criticalPath;
	}

	void Update() {
		elapsedTime += Time.deltaTime;
		motor.movement = lastMove * stubornness;

		if (lastChoice < 0 || Time.time - lastChoice >= choiceDelay) {
			lastChoice = Time.time;
			CalculateWeights();
			float criticalChance = criticalWeight;
			float drainChance = criticalChance + drainWeight;
			float protectChance = drainChance + protectWeight;
			float retreatChance = protectChance + retreatWeight;
			float choice = (float)random.NextDouble();
			if (choice < criticalChance) {
				criticalWeight *= decisiveness;
			} else if (choice < drainChance) {
				drainWeight *= decisiveness;
			} else if (choice < protectChance) {
				protectWeight *= decisiveness;
			} else if (choice < retreatChance) {
				retreatWeight *= decisiveness;
			}
			NormalizeWeights();
		}

		Vector3 criticalMove = CalculateCriticalDirection() * criticalWeight;
		Vector3 drainMove = CalculateDrainDirection() * drainWeight;
		Vector3 protectMove = CalculateProtectDirection() * protectWeight;
		Vector3 retreatMove = CalculateRetreatDirection() * retreatWeight;

		motor.movement += criticalMove + drainMove + protectMove + retreatMove;
		lastMove = motor.movement.normalized;
		motor.UpdateMotor();
	}

	private void CalculateWeights() {
		float distProporation = ((transform.position - escortee.transform.position).magnitude - minPrioritizeDistance) / (maxPrioritizeDistance - minPrioritizeDistance);
		distProporation = Mathf.Clamp(distProporation, 0, 1);
		float escorteeHealthFromOptimal = Mathf.Abs(escorteeHealth.Health - 0.5f) * 2;
		float healthAboveOptimal = Mathf.Max(health.Health - 0.5f, 0) * 2;

		// Calculate importance of progressing along critical path.
		criticalWeight = criticalBaseWeight + (((elapsedTime / maxTime) * criticalTimeWeight));
		/*TODO are these calculations wrong, escort constanct healing is breaking everything.*/
		// Calculate importance of finding a drainer to restore condition.
		drainWeight = drainBaseWeight * ((healthAboveOptimal * drainEscortWeight) + (escorteeHealthFromOptimal * drainEscorteeWeight)); 

		// Calculate importance of protecting escortee.
		protectWeight = protectBaseWeight * ((escorteeHealthFromOptimal * protectEscorteeWeight) - (healthAboveOptimal * protectEscortWeight) + (distProporation * protectDistanceWeight));

		// Calculate importance of retreating to safety for escortee.
		/*TODO might not need retreat*/
		retreatWeight = retreatBaseWeight * (((1 - escorteeHealth.Health) * retreatEscorteeWeight) - (distProporation * retreatDistanceWeight) - (health.Health * retreatEscortWeight));

		NormalizeWeights();
	}

	private void NormalizeWeights() {
		criticalWeight = Mathf.Clamp(criticalWeight, 0, 1);
		drainWeight = Mathf.Clamp(drainWeight, 0, 1);
		protectWeight = Mathf.Clamp(protectWeight, 0, 1);
		retreatWeight = Mathf.Clamp(retreatWeight, 0, 1);

		float weightSum = criticalWeight + drainWeight + protectWeight + retreatWeight;
		if (weightSum > 1) {
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

	private Vector3 CalculateRetreatDirection() {
		if (retreatWeight > 0 && escorteeRefugeIndex < 0) {
			float minSqrDist = 0;
			for (int i = 0; i < escorteeRefuges.Count; i++) {
				float sqrDist = (escorteeRefuges[i].transform.position - transform.position).sqrMagnitude;
				if (escorteeRefugeIndex < 0 || sqrDist < minSqrDist) {
					minSqrDist = sqrDist;
					escorteeRefugeIndex = i;
				}
			}
		}

		Vector3 retreatDirection = Vector3.zero;
		if (escorteeRefugeIndex >= 0) {
			retreatDirection = escorteeRefuges[escorteeRefugeIndex].transform.position - transform.position;
			retreatDirection.Normalize();
		}
		
		return retreatDirection;
	}
}