using UnityEngine;
using System.Collections;

public class HealthDrain : MonoBehaviour {
	public bool drainActive;
	public GameObject drainee;
	private HealthTracker draineeHealth;
	public GameObject drainer;
	private HealthTracker drainerHealth;
	public ParticleSystem emitter;
	private float emitterBaseSpeed;
	public HealthDrain preferredDrain;
	public float rate;
	public float maxDistance;
	public HealthDampening dampenRelativeTo;
	public float minDampenedRate;

	public enum HealthDampening {
		NONE = 0,
		DRAINEE_HEALTH,
		DRAINER_HEALTH
	};

	void Start() {
		draineeHealth = drainee.GetComponent<HealthTracker>();
		drainerHealth = drainer.GetComponent<HealthTracker>();
		if (emitter != null) {
			emitterBaseSpeed = emitter.startSpeed;
		}
	}

	void Update() {
		Vector3 drainerPos = drainer.transform.position;
		Vector3 draineePos = drainee.transform.position;
		drainerPos.y = drainee.transform.position.y;
		if ((!preferredDrain || !preferredDrain.drainActive) && (drainerPos - draineePos).sqrMagnitude < maxDistance * maxDistance) {
			drainActive = true;
			if (renderer != null) {
				renderer.enabled = true;
			}

			// Position at drainee and point at drainer.
			transform.position = drainee.transform.position;
			transform.LookAt(drainer.transform.position);
			if (emitter != null) {
				emitter.enableEmission = true;
				emitter.startSpeed = rate * Mathf.Pow(emitterBaseSpeed, 2);
				float distance = (drainer.transform.position - drainee.transform.position).magnitude;
				if (emitter.startSpeed <= 0) {
					emitter.startLifetime = 0;
				} else {
					emitter.startLifetime = distance / emitter.startSpeed;
				}
			}

			// Calculate actual drain rate.
			float drainRate = rate;
			if (dampenRelativeTo == HealthDampening.DRAINEE_HEALTH && draineeHealth != null) {
				drainRate *= draineeHealth.Health;
			} else if (dampenRelativeTo == HealthDampening.DRAINER_HEALTH && drainerHealth != null) {
				drainRate *= 1 - drainerHealth.Health;
			}
			drainRate = Mathf.Clamp(drainRate, minDampenedRate, rate);

			// Drain health.
			if (draineeHealth != null) {
				draineeHealth.AlterHealth(-drainRate * Time.deltaTime);
			}
			if (drainerHealth != null) {
				drainerHealth.AlterHealth(drainRate * Time.deltaTime);
			}
		} else {
			drainActive = false;
		}

		// Toggle visiblity based on drain activity.
		if (renderer != null) {
			renderer.enabled = drainActive;
		}
		if (emitter) {
			emitter.enableEmission = drainActive;
		}
	}
}
