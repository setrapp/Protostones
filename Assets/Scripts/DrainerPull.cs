using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrainerPull : MonoBehaviour {
	public CharacterController target;
	public List<GameObject> drainers;
	public float pullSpeed;
	public HealthDrain deactivatorDrainer;

	void Update() {
		if (deactivatorDrainer == null || !deactivatorDrainer.drainActive) {
			float minSqrDist = 0;
			int closestDrainer = -1;
			for (int i = 0; i < drainers.Count; i++) {
				float sqrDist = (drainers[i].transform.position - target.transform.position).sqrMagnitude;
				if (closestDrainer < 0 || sqrDist < minSqrDist) {
					minSqrDist = sqrDist;
					closestDrainer = i;
				}
			}

			Vector3 pull = drainers[closestDrainer].transform.position - target.transform.position;
			pull = pull.normalized * pullSpeed * Time.deltaTime;
			target.Move (pull);
		}
	}
}