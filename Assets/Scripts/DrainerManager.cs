using UnityEngine;
using System.Collections;

public class DrainerManager : MonoBehaviour {
	public GameObject player;
	public GameObject escort;
	public HealthDrain preferredDrain;

	void Awake() {
		EscortController escortController = escort.GetComponent<EscortController>();
		escortController.drainers.Clear();

		for (int i = 0; i < transform.childCount; i++) {
			DrainerSetup childDrainer = transform.GetChild(i).GetComponent<DrainerSetup>();
			if (childDrainer != null) {
				childDrainer.playerDrain.drainee = player;
				childDrainer.playerDrain.drainer = childDrainer.gameObject;
				childDrainer.playerDrain.preferredDrain = preferredDrain;
				childDrainer.escortDrain.drainee = escort;
				childDrainer.escortDrain.drainer = childDrainer.gameObject;
				escortController.drainers.Add(childDrainer.gameObject);
			}
		}
	}
}
