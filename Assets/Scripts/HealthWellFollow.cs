using UnityEngine;
using System.Collections;

public class HealthWellFollow : MonoBehaviour {
	public HealthDrain drain;
	public GameObject leader;
	public Vector3 seperation;
	public bool drainLeader;
	private bool wasDrainLeader;

	void Awake() {
		SetupTargets();
		wasDrainLeader = drainLeader;
		if (drain.emitter != null) {
			drain.emitter.simulationSpace = ParticleSystemSimulationSpace.Local;
		}
	}

	void Update() {
		/*TODO collider triggers to stop wells*/
		if (wasDrainLeader != drainLeader) {
			wasDrainLeader = drainLeader;
		}

		transform.position = leader.transform.position +
			(leader.transform.right * seperation.x) +
			(leader.transform.up * seperation.y) +
			(leader.transform.forward * seperation.z);
	}

	private void SetupTargets() {
		if (drainLeader) {
			drain.drainee = leader;
			drain.drainer = gameObject;
		} else {
			drain.drainee = gameObject;
			drain.drainer = leader;
		}
	}
}
