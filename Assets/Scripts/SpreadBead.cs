using UnityEngine;
using System.Collections;

public class SpreadBead : MonoBehaviour {
	public Spreader spreader;
	private SpreadBead[] neighbors;
	private int neighborCount = 0;
	private bool neighborLocked = false;
	public bool NeighborLocked {
		get { return neighborLocked; }
	}
	public bool touchingPrey;
	public bool frozenOnStart;
	private bool frozen = false;
	public bool Frozen {
		get { return frozen; }
		set {
			frozen = value; 
			collider.enabled = frozen;
			renderer.material = frozenMaterial;
		}
	}
	public Material normalMaterial;
	public Material frozenMaterial;

	void Start() {
		/*TODO connect adjacent beads as neighbors? (Might do within spreader)*/
		if (neighbors == null || neighbors.Length < 1) {
			InitNeighbors();
		}
		touchingPrey = false;
		if (normalMaterial != null) {
			normalMaterial = renderer.material;
		} else { 
			renderer.material = normalMaterial;
		}
		if (frozenOnStart) {
			Frozen = true;
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

	/*void OnCollisionEnter(Collision collision) {
		if (spreader != null && spreader.prey.collider == collision.collider) {
			touchingPrey = true;
		}
	}

	void OnCollisionExit(Collision collision) {
		if (spreader != null && spreader.prey.collider == collision.collider) {
			touchingPrey = false;
		}
	}*/
}
