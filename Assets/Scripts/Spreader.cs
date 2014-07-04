using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spreader : MonoBehaviour {
	public GameObject beadPrefab;
	public List<SpreadBead> beads;
	private Vector3[] neighborDirections;
	public float neighborDistance;
	public float spreadDelay;
	public float lastSpreadTime = - 1;
	public GameObject prey;
	public SpreadBead lastSpreadBead;

	void Start() {
		neighborDirections = new Vector3[8];
		neighborDirections[0] = Vector3.up;
		neighborDirections[1] = (Vector3.up + Vector3.right).normalized;
		neighborDirections[2] = Vector3.right;
		neighborDirections[3] = (-Vector3.up + Vector3.right).normalized;
		neighborDirections[4] = -Vector3.up;
		neighborDirections[5] = (-Vector3.up - Vector3.right).normalized;
		neighborDirections[6] = -Vector3.right;
		neighborDirections[7] = (Vector3.up - Vector3.right).normalized;
	}

	void Update() {
		if (spreadDelay < 0 || Time.time - lastSpreadTime >= spreadDelay) {
			Spread();
		}
	}

	private void Spread() {
		if (lastSpreadBead == null) {
			if (beads.Count < 1) {
				return;
			}
			lastSpreadBead = beads[0];
		}

		SeekPrey();
	}

	private void SeekPrey() {
		int beadCount = beads.Count;

		float minPreySqrDist = (prey.transform.position - lastSpreadBead.transform.position).sqrMagnitude;
		SpreadBead nearestBead = lastSpreadBead;
		/*TODO Check based on neighbors from guessed nearest*/
		for (int i = 0; i < beadCount; i++) {
			float preySqrDist = (prey.transform.position - beads[i].transform.position).sqrMagnitude;
			if (preySqrDist < minPreySqrDist) {
				minPreySqrDist = preySqrDist;
				nearestBead = beads[i];
			}
		}

		int bestNeighborIndex = -1;
		Vector3 bestNeighborPosition;
		for (int i = 0; i < neighborDirections.Length; i++) {
			if (nearestBead.neighbors[i] == null) {
				Vector3 neighborPosition = nearestBead.transform.position + (neighborDirections[i] * neighborDistance);
				float preySqrDist = (prey.transform.position - neighborPosition).sqrMagnitude;
				if (preySqrDist < minPreySqrDist) {
					minPreySqrDist = preySqrDist;
					bestNeighborIndex = i;
					bestNeighborPosition = neighborPosition;
				}
			}
		}

		if (bestNeighborIndex >= 0) {
			SpreadBead newBead = ((GameObject)GameObject.Instantiate(beadPrefab, bestNeighborPosition, Quaternion.identity)).GetComponent<SpreadBead>();
			beads.Add(newBead);
			nearestBead.neighbors[bestNeighborIndex] = newBead;
			newBead.neighbors[(int)(Mathf.Abs((neighborDirections.Length / 2) - bestNeighborIndex))] = nearestBead;
			newBead.transform.parent = nearestBead.transform.parent;
			lastSpreadBead = newBead;
			lastSpreadTime = Time.time;
		} else {
			/*TODO Handle case where found bead is closest to the prey?*/
		}
	}
}
