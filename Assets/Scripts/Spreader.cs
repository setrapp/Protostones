using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spreader : MonoBehaviour {
	public GameObject beadPrefab;
	private List<SpreadBead> beads;
	public int seed;
	private System.Random random;
	private Vector3[] neighborDirections;
	public float neighborDistance;
	private float neighborEpsilon;
	public bool frozenOnStart = false;
	private List<SpreaderFakePrey> onStartSpreadPrey;
	public int onStartWanderCount = 0;
	public float spreadDelay;
	public float lastSpreadTime = - 1;
	public float damageDelay;
	public float lastDamageTime = - 1;
	public int potentialSpreadCount = 1;
	public int spreadCount = 1;
	public GameObject prey;
	public float preyingDistance;
	public float damageDistance;
	public int damage;
	public GameObject spatialAudioContainer;
	public AudioSource stepAudioPlayer;
	public AudioSource drainAudioPlayer;
	//public float preySpeedMultiplier;

	void Start() {
		if (seed <= 0) {
			seed = (int)(System.DateTime.Now.Ticks % int.MaxValue);
		}
		random = new System.Random(seed);

		neighborEpsilon = neighborDistance / 1000.0f;

		neighborDirections = new Vector3[8];
		neighborDirections[0] = Vector3.up;
		neighborDirections[1] = (Vector3.up + Vector3.right).normalized;
		neighborDirections[2] = Vector3.right;
		neighborDirections[3] = (-Vector3.up + Vector3.right).normalized;
		neighborDirections[4] = -Vector3.up;
		neighborDirections[5] = (-Vector3.up - Vector3.right).normalized;
		neighborDirections[6] = -Vector3.right;
		neighborDirections[7] = (Vector3.up - Vector3.right).normalized;

		beads = new List<SpreadBead>();
		onStartSpreadPrey = new List<SpreaderFakePrey>();
		for (int i = 0; i < transform.childCount; i++) {
			SpreadBead childBead = transform.GetChild(i).GetComponent<SpreadBead>();
			SpreaderFakePrey childPrey = transform.GetChild(i).GetComponent<SpreaderFakePrey>();
			if (childBead != null) {
				beads.Add(childBead);
			} else if (childPrey) {
				onStartSpreadPrey.Add(childPrey);
			}
		}

		for (int i = 0; i < beads.Count; i++) {
			beads[i].spreader = this;
			FindNeighbors(beads[i]);
		}

		for (int i = 0; i < onStartWanderCount; i++) {
			Wander();
		}

		GameObject actualPrey = prey;
		int actualPotentialSpreadCount = potentialSpreadCount;
		int actualSpreadCount = spreadCount;
		for(int i = 0; i < onStartSpreadPrey.Count; i++) {
			prey = onStartSpreadPrey[i].gameObject;
			potentialSpreadCount = onStartSpreadPrey[i].potentialSpreadCount;
			if (potentialSpreadCount <= 0) {
				potentialSpreadCount = actualPotentialSpreadCount;
			}
			spreadCount = onStartSpreadPrey[i].potentialSpreadCount;
			if (spreadCount <= 0) {
				spreadCount = actualSpreadCount;
			}
			SpreadBead preyingBead;
			do {
				SeekPrey(onStartSpreadPrey[i].freezeBeadsTo, true);
				preyingBead = FindBeadNearPrey(false, true);
			} while ((prey.transform.position - preyingBead.transform.position).sqrMagnitude > damageDistance * damageDistance);
		}
		prey = actualPrey;
		potentialSpreadCount = actualPotentialSpreadCount;
		spreadCount = actualSpreadCount;


		if (frozenOnStart) {
			for (int i = 0; i < beads.Count; i++) {
				beads[i].Frozen = true;
			}
		}
	}

	void Update() {
		bool preying = false;
		bool damaging = false;

		// Determine nearest proximity to player.
		SpreadBead preyingBead = FindBeadNearPrey(true);
		if (preyingBead != null) {
			Vector3 toPreyDir = prey.transform.position - preyingBead.transform.position;
			float toPreyDist = toPreyDir.magnitude;
			if (toPreyDist > 0) {
				toPreyDir /= toPreyDist;
			}

			spatialAudioContainer.transform.position = preyingBead.transform.position;

			if (toPreyDist < preyingDistance) {
				preying = true;
			}

			// Detect if touching player.
			if (toPreyDist < damageDistance) {
				damaging = true;
			}

			// Damage the prey or spread on timer.
			if (damaging && (damageDelay < 0 || Time.time - lastDamageTime >= damageDelay)) {
				prey.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
				//drainAudioPlayer.Play();
				lastDamageTime = Time.time;
			} else if (!frozenOnStart && (spreadDelay < 0 || Time.time - lastSpreadTime >= spreadDelay)) {
				//if (!damaging) {
					stepAudioPlayer.Play();
				//}
				Spread(preying);
				lastSpreadTime = Time.time;
			}
		}
	}

	private void Spread(bool preying) {
		if (beads.Count < 1) {
			return;
		}

		if (preying) {
			SeekPrey();

		} else {
			//Wander();
		}
	}

	private void SeekPrey(bool spawnFrozen = false, bool searchFrozen = false) {
		/*TODO Distribute processing time over spread delay. Possible with moving character?*/

		List<SpreadBead> potentialBeads = FindBeadsNearPrey(potentialSpreadCount, false, searchFrozen);
		List<SpreadBead> spreadingBeads = new List<SpreadBead>();

		if (spreadCount >= potentialBeads.Count || spreadCount >= potentialSpreadCount) {
			spreadingBeads = potentialBeads;
		} else {
			while (spreadingBeads.Count < spreadCount) {
				int randomIndex = random.Next(0, potentialBeads.Count);
				spreadingBeads.Add(potentialBeads[randomIndex]);
				potentialBeads.RemoveAt(randomIndex);
			}
		}

		for (int i = 0; i < spreadingBeads.Count; i++) {
			int bestNeighborIndex = -1;
			Vector3 bestNeighborPosition = Vector3.zero;
			float minPreySqrDist = (prey.transform.position - spreadingBeads[i].transform.position).sqrMagnitude;
			for (int j = 0; j < neighborDirections.Length; j++) {
				if (spreadingBeads[i].GetNeighbor(j) == null) {
					Vector3 neighborPosition = spreadingBeads[i].transform.position + (neighborDirections[j] * neighborDistance);
					float preySqrDist = (prey.transform.position - neighborPosition).sqrMagnitude;
					if (preySqrDist < minPreySqrDist) {
						minPreySqrDist = preySqrDist;
						bestNeighborIndex = j;
						bestNeighborPosition = neighborPosition;
					}
				}
			}

			if (bestNeighborIndex >= 0) {
				CreateBead(bestNeighborPosition, spawnFrozen, spreadingBeads[i], bestNeighborIndex, false);
			}
		}
	}

	private void Wander(bool spawnFrozen = false) {
		/*TODO Maybe use a more sophisticated wander than just random*/
		/*TODO Likely want to give higher precedence to outer limbs to avoid clumping*/
		/*TODO Try looping through before to add to weight outcome*/
		/*TODO What about just seeking random 'prey' for a number of steps?*/
		for (int i = 0; i < spreadCount; i++) {
			SpreadBead spreadingBead = beads[random.Next(0, beads.Count)];
			int spreadingDir = random.Next(0, neighborDirections.Length);

			// If the chose bead has an empty neighbor in the chose direction, than create a bead.
			// Actually creating the max number (or any) beads is not guaranteed, problem?
			if (spreadingBead.GetNeighbor(spreadingDir) == null) {
				Vector3 neighborPosition = spreadingBead.transform.position + (neighborDirections[spreadingDir] * neighborDistance);
				CreateBead(neighborPosition, spawnFrozen, spreadingBead, spreadingDir, false);
			}
		}
	}

	private SpreadBead FindBeadNearPrey(bool includeNeighborLocked = false, bool includeFrozen = false) {
		SpreadBead nearestBead = null;
		List<SpreadBead> nearBeads = FindBeadsNearPrey(1, includeNeighborLocked, includeFrozen);
		if (nearBeads.Count > 0) {
			nearestBead = nearBeads[0];
		}
		return nearestBead;
	}

	private List<SpreadBead> FindBeadsNearPrey(int count, bool includeNeighborLocked = false, bool includeFrozen = false) {
		int beadCount = beads.Count;
		List<SpreadBead> nearBeads = new List<SpreadBead>();
		List<float> nearSqrDists = new List<float>();
		for (int i = 0; i < beadCount; i++) {
			if ((includeNeighborLocked || !beads[i].NeighborLocked) && (includeFrozen || !beads[i].Frozen)) {
				float preySqrDist = (prey.transform.position - beads[i].transform.position).sqrMagnitude;
				if (nearBeads.Count < 1) {
					nearBeads.Add(beads[i]);
					nearSqrDists.Add(preySqrDist);
				} else if (preySqrDist < nearSqrDists[nearSqrDists.Count - 1]) {
					for (int j = nearBeads.Count - 1; j >= 0; j--) {
						if (preySqrDist < nearSqrDists[j]) {
							nearBeads.Insert(j, beads[i]);
							nearSqrDists.Insert(j, preySqrDist);
							if (nearBeads.Count > count) {
								nearBeads.RemoveAt(nearBeads.Count - 1);
								nearSqrDists.RemoveAt(nearSqrDists.Count - 1);
							}
						}
					}
				} else if (nearBeads.Count < count) {
					nearBeads.Add(beads[i]);
					nearSqrDists.Add(preySqrDist);
				}
			}
		}
		return nearBeads;
	}

	private void FindNeighbors(SpreadBead bead) {
		if (!bead || bead.spreader != this) {
			return;
		}

		if (!bead.NeighborLocked) {
			float closeEnoughSqr = (neighborDistance + neighborEpsilon) * (neighborDistance + neighborEpsilon);

			for (int i = 0; i < beads.Count; i++) {
				if (beads[i] != bead && !beads[i].NeighborLocked &&
				    (beads[i].transform.position - bead.transform.position).sqrMagnitude <= closeEnoughSqr)
				{
					Vector3 toNeighbor = beads[i].transform.position - bead.transform.position;
					float bestToDotDir = Vector3.Dot(toNeighbor, neighborDirections[0]);
					int bestDirIndex = 0;
					for (int j = 1; j < neighborDirections.Length; j++) {
						float toDotDir = Vector3.Dot(toNeighbor, neighborDirections[j]);
						if (toDotDir > bestToDotDir) {
							bestToDotDir = toDotDir;
							bestDirIndex = j;
						}
					}

					ConnectNeighbor(bead, beads[i], bestDirIndex, false);
				}
			}
		}
	}

	private bool ConnectNeighbor(SpreadBead bead, SpreadBead neighbor, int neighborIndex, bool overwriteNeighbors) {
		int halfNeighborCount = neighborDirections.Length / 2;
		int inverseNeighborIndex = neighborIndex + halfNeighborCount;
		if (neighborIndex >= halfNeighborCount) {
			inverseNeighborIndex = neighborIndex - halfNeighborCount;
		}

		if (!overwriteNeighbors && (bead.GetNeighbor(neighborIndex) != null || neighbor.GetNeighbor(inverseNeighborIndex) != null)) {
			return false;
		}

		bead.SetNeighbor(neighborIndex, neighbor);
		neighbor.SetNeighbor(inverseNeighborIndex, bead);
		return true;
	}

	private void CreateBead(Vector3 newPosition, bool spawnFrozen = false, SpreadBead spreadNeighor = null, int fromDirectionIndex = 0, bool overwriteNeighbors = false) {
		SpreadBead newBead = ((GameObject)GameObject.Instantiate(beadPrefab, newPosition, Quaternion.identity)).GetComponent<SpreadBead>();
		beads.Add(newBead);
		newBead.spreader = this;
		newBead.transform.parent = transform;
		if (spreadNeighor != null) {
			ConnectNeighbor(spreadNeighor, newBead, fromDirectionIndex, overwriteNeighbors);
		}
		FindNeighbors(newBead);
		if(spawnFrozen) {
			newBead.Frozen = true;
		}
	}
}
