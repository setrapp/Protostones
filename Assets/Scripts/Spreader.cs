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
	public bool spreading = true;
	public int initialSpreadCount = 0;
	public float spreadDelay;
	public float lastSpreadTime = - 1;
	public int potentialSpreadCount = 1;
	public int spreadCount = 1;
	public GameObject prey;

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
		for (int i = 0; i < transform.childCount; i++) {
			SpreadBead childBead = transform.GetChild(i).GetComponent<SpreadBead>();
			if (childBead != null) {
				beads.Add(childBead);
			}
		}

		for (int i = 0; i < beads.Count; i++) {
			beads[i].spreader = this;
			FindNeighbors(beads[i]);
		}

		for (int i = 0; i < initialSpreadCount; i++) {
			Wander();
		}
	}

	void Update() {
		if (spreading && (spreadDelay < 0 || Time.time - lastSpreadTime >= spreadDelay)) {
			Spread();
		}
	}

	private void Spread() {
		if (beads.Count < 1) {
			return;
		}

		/*TODO Only seek prey withing range, probably put nearest bead calculation here.*/
		SeekPrey();
		//Wander();
	}

	private void SeekPrey() {
		/*TODO Distribute processing time over spread delay. Possible with moving character?*/

		int beadCount = beads.Count;
		List<SpreadBead> potentialBeads = new List<SpreadBead>();
		List<float> potentialSqrDists = new List<float>();
		List<SpreadBead> spreadingBeads = new List<SpreadBead>();

		potentialBeads.Add(beads[0]);
		potentialSqrDists.Add((prey.transform.position - beads[0].transform.position).sqrMagnitude);
		for (int i = 1; i < beadCount; i++) {
			if (!beads[i].NeighborLocked) {
				float preySqrDist = (prey.transform.position - beads[i].transform.position).sqrMagnitude;
				if (preySqrDist < potentialSqrDists[potentialSqrDists.Count - 1]) {
					for (int j = potentialBeads.Count - 1; j >= 0; j--) {
						if (preySqrDist < potentialSqrDists[j]) {
							potentialBeads.Insert(j, beads[i]);
							potentialSqrDists.Insert(j, preySqrDist);
							if (potentialBeads.Count > potentialSpreadCount) {
								potentialBeads.RemoveAt(potentialBeads.Count - 1);
								potentialSqrDists.RemoveAt(potentialSqrDists.Count - 1);
							}
						}
					}
				} else if (potentialBeads.Count < potentialSpreadCount) {
					potentialBeads.Add(beads[i]);
					potentialSqrDists.Add(preySqrDist);
				}
			}
		}

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
				CreateBead(bestNeighborPosition, spreadingBeads[i], bestNeighborIndex, false);
			}
		}
	}

	private void Wander() {
		/*TODO Maybe use a more sophisticated wander than just random*/
		/*TODO Likely want to give higher precedence to outer limbs to avoid clumping*/
		for (int i = 0; i < spreadCount; i++) {
			SpreadBead spreadingBead = beads[random.Next(0, beads.Count)];
			int spreadingDir = random.Next(0, neighborDirections.Length);

			// If the chose bead has an empty neighbor in the chose direction, than create a bead.
			// Actually creating the max number (or any) beads is not guaranteed, problem?
			if (spreadingBead.neighbors[spreadingDir] == null) {
				Vector3 neighborPosition = spreadingBead.transform.position + (neighborDirections[spreadingDir] * neighborDistance);
				CreateBead(neighborPosition, spreadingBead, spreadingDir, false);
			}
		}
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

	private void CreateBead(Vector3 newPosition, SpreadBead spreadNeighor = null, int fromDirectionIndex = 0, bool overwriteNeighbors = false) {
		SpreadBead newBead = ((GameObject)GameObject.Instantiate(beadPrefab, newPosition, Quaternion.identity)).GetComponent<SpreadBead>();
		beads.Add(newBead);
		newBead.spreader = this;
		newBead.transform.parent = transform;
		if (spreadNeighor != null) {
			ConnectNeighbor(spreadNeighor, newBead, fromDirectionIndex, overwriteNeighbors);
		}
		FindNeighbors(newBead);
		lastSpreadTime = Time.time;
	}
}
