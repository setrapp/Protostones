using UnityEngine;
using System.Collections;

public class SpreadBead : MonoBehaviour {
	public SpreadBead[] neighbors;

	void Start() {
		/*TODO connect adjacent beads as neighbors? (Might do within spreader)*/
		/*neighbors = new SpreadBead[8];
		for (int i = 0; i < neighbors.Length; i++) {
			neighbors[i] = null;
		}*/
	}
}
