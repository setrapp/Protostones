using UnityEngine;
using System.Collections;

public class SpreadBead : MonoBehaviour {
	public Spreader spreader;
	public/*private*/ SpreadBead[] neighbors;
	private int neighborCount = 0;
	private bool neighborLocked = false;
	public bool NeighborLocked {
		get { return neighborLocked; }
	}


	void Start() {
		/*TODO connect adjacent beads as neighbors? (Might do within spreader)*/
		if (neighbors == null || neighbors.Length < 1) {
			InitNeighbors();
		}
	}

	private void InitNeighbors() {
		neighbors = new SpreadBead[8];
		for (int i = 0; i < neighbors.Length; i++) {
			neighbors[i] = null;
		}
	}

	public SpreadBead GetNeighbor(int index) {
		if (neighbors == null || neighbors.Length < 1) {
			InitNeighbors();
		}
		return neighbors[index];
	}

	public void SetNeighbor(int index, SpreadBead neighbor) {
		if (neighbors == null || neighbors.Length < 1) {
			InitNeighbors();
		}
		neighbors[index] = neighbor;
		if (neighbor != null) {
			neighborCount++;
			if (neighborCount >= neighbors.Length) {
				neighborLocked = true;
			}
		} else {
			neighborCount--;
			neighborLocked = false;
		}
	}


}
