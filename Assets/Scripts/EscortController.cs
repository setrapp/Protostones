using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EscortController : MonoBehaviour {
	private SimpleMotor motor;
	private HealthTracker health;
	public List<GameObject> criticalPath;
	private int criticalIndex;
	public List<GameObject> drainers;
	public int drainerIndex;
	public List<GameObject> escorteeRefuges;
	private int escorteeRefugeIndex;
	public GameObject escortee;
	private HealthTracker escorteeHealth;
	public string escorteeInPositive;
	public string escorteeInNegative;
	private bool escorteeHealing = true;
	public float helpIncrement;
	public float helpDecay;
	public float annoyDecay;
	private float helpWeight;
	private float annoyWeight;
	public float annoyThreshold;
	public float choiceDelay;
	private float lastChoice = -1;
	private System.Random random;
	public int seed = 0;
	private Vector3 lastMove;
	public float decisiveness = 1;
	public float stubornness = 1;
	public float seekProximity;
	private Vector3 startPosition;
	private Quaternion startRotation;
	public GameObject babyDrainerPrefab;

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
	public float timeSinceCheckpoint;
	public float timePerCheckpoint;
	public float minPrioritizeDistance;
	public float maxPrioritizeDistance;
	public float healthIgnoreThreshold;

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
		timeSinceCheckpoint = timePerCheckpoint;
		helpWeight = 0;
		startPosition = transform.position;
		startRotation = transform.rotation;

		//TODO Make separate list for refuges.
		escorteeRefuges = criticalPath;
	}

	void Update() {
		timeSinceCheckpoint += Time.deltaTime;
		motor.movement = lastMove * stubornness;

		if (lastChoice < 0 || Time.time - lastChoice >= choiceDelay) {
			lastChoice = Time.time;
			CalculateWeights();
			float[] weights = new float[] {criticalWeight, drainWeight, protectWeight, retreatWeight};
			float maxWeight = Mathf.Max(weights);
			if (criticalWeight == maxWeight) {
				criticalWeight *= decisiveness;
			}else if (drainWeight == maxWeight) {
				drainWeight *= decisiveness;
			}else if (protectWeight == maxWeight) {
				protectWeight *= decisiveness;
			}else if (retreatWeight == maxWeight) {
				retreatWeight *= decisiveness;
			}
			/*float criticalChance = criticalWeight;
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
			}*/
			NormalizeWeights();
		}

		Vector3 criticalMove = CalculateCriticalDirection() * criticalWeight;
		Vector3 drainMove = CalculateDrainDirection() * drainWeight;
		Vector3 protectMove = CalculateProtectDirection() * protectWeight;
		Vector3 retreatMove = CalculateRetreatDirection() * retreatWeight;

		Vector3 newMove = criticalMove + drainMove + protectMove + retreatMove;
		if (newMove.sqrMagnitude <= 0) {
			motor.movement = Vector3.zero;
		}
		motor.movement += newMove;
		lastMove = motor.movement.normalized;
		motor.UpdateMotor();

		if (helpWeight > 0) {
			helpWeight -= helpDecay * Time.deltaTime;
		} else if (helpWeight < 0) {
			helpWeight = 0;
		}

		if (annoyWeight > 0) {
			annoyWeight -= annoyDecay * Time.deltaTime;
		} else if (annoyWeight < 0) {
			annoyWeight = 0;
		}
	}

	private void CalculateWeights() {
		float distProporation = ((transform.position - escortee.transform.position).magnitude - minPrioritizeDistance) / (maxPrioritizeDistance - minPrioritizeDistance);
		distProporation = Mathf.Clamp(distProporation, 0, 1);
		float escorteeHealthFromOptimal = Mathf.Max(escorteeHealth.Health - 0.5f, 0) * 2;
		if (!escorteeHealing) {
			escorteeHealthFromOptimal = Mathf.Max(0.5f - escorteeHealth.Health, 0) * 2;
		}
		if (escorteeHealthFromOptimal < healthIgnoreThreshold) {
			escorteeHealthFromOptimal = 0;
		}
		float healthAboveOptimal = Mathf.Max(health.Health - 0.5f, 0) * 2;
		if (healthAboveOptimal < healthIgnoreThreshold) {
			healthAboveOptimal = 0;
		}
		float helpModulation = helpWeight;
		if (annoyWeight >= annoyThreshold) {
			helpModulation *= -1;
		}

		// Calculate importance of progressing along critical path.
		criticalWeight = criticalBaseWeight * ((Mathf.Min(timeSinceCheckpoint / timePerCheckpoint, 1) * criticalTimeWeight));

		// Calculate importance of finding a drainer to restore condition.
		drainWeight = drainBaseWeight * ((healthAboveOptimal * drainEscortWeight) - (escorteeHealthFromOptimal * drainEscorteeWeight)); 

		// Calculate importance of protecting escortee.
		protectWeight = protectBaseWeight * (helpModulation + Mathf.Max((escorteeHealthFromOptimal * protectEscorteeWeight) - (healthAboveOptimal * protectEscortWeight), (distProporation * protectDistanceWeight)));

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

	private void ObjectToggled(string message) {
		if (message == escorteeInPositive) {
			escorteeHealing = true;
		} else if (message == escorteeInNegative) {
			escorteeHealing = false;
		}
	}

	private void HelpEscortee() {
		helpWeight += helpIncrement;
		annoyWeight += helpIncrement;
	}

	private void EmptyHealth() {
		enabled = false;
	}

	private void Reset() {
		GameObject baby = (GameObject)GameObject.Instantiate(babyDrainerPrefab, transform.position, transform.rotation);
		DrainerSetup babySetup = baby.GetComponent<DrainerSetup>();
		babySetup.playerDrain.drainee = escortee;
		babySetup.escortDrain.drainee = gameObject;
		transform.position = startPosition;
		transform.rotation = startRotation;
		health.ResetHealth();
	}

	private Vector3 CalculateCriticalDirection() {
		/*TODO Could probably do simple path following, or just check if object has moved beyond the half plane perpedicular to vector from last checkpoint to this one.*/
		if (criticalIndex < criticalPath.Count && (criticalPath[criticalIndex].transform.position - transform.position).sqrMagnitude < seekProximity * seekProximity) {
			criticalIndex++;
			timeSinceCheckpoint = 0;
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
		if (drainWeight > 0) {
			float minSqrDist = 0;
			drainerIndex = -1;
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
			Vector3 drainerPos = drainers[drainerIndex].transform.position;
			drainerPos.y = transform.position.y;
			drainDirection = drainerPos - transform.position;
			drainDirection.Normalize();
			Vector3 nearDrainerPos = drainerPos - (drainDirection * seekProximity);
			if (Vector3.Dot(drainDirection, nearDrainerPos - transform.position) <= 0) {
				drainDirection = Vector3.zero;
			}
		}

		return drainDirection;
	}

	private Vector3 CalculateProtectDirection() {
		Vector3 protectDirection = Vector3.zero;
		Vector3 escorteePos = escortee.transform.position;
		escorteePos.y = transform.position.y;
		protectDirection = escorteePos - transform.position;
		protectDirection.Normalize();
		Vector3 nearEscorteePos = escorteePos - (protectDirection * seekProximity);
		if (Vector3.Dot(protectDirection, nearEscorteePos - transform.position) <= 0) {
			protectDirection = Vector3.zero;
		}
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