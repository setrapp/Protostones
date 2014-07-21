using UnityEngine;
using System.Collections;

public class TreePull : MonoBehaviour {
	public GameObject player;
	public GameObject escort;
	public GameObject cameraFade;
	public float pullSpeed;
	public float pullDistance;
	private bool pulling;
	public float fadeSpeed;

	void Update() {
		Vector3 playerPos = player.transform.position;
		playerPos.y = transform.position.y;
		if (!pulling && (playerPos - transform.position).sqrMagnitude < pullDistance * pullDistance) {
			pulling = true;
			MonoBehaviour[] playerComponents = player.GetComponents<MonoBehaviour>();
			for (int i = 0; i < playerComponents.Length; i++) {
				playerComponents[i].enabled = false;
			}
			player.GetComponent<MeshRenderer>().enabled = true;
			player.GetComponent<HealthTracker>().enabled = true;
			player.GetComponent<SimpleCamera>().enabled = true;
			escort.GetComponent<EscortController>().enabled = false;
			cameraFade.renderer.material.color = new Color(1, 1, 1, Mathf.Max(cameraFade.renderer.material.color.a, 0));
		} else if (pulling) {
			if ((transform.position - player.transform.position).sqrMagnitude > Mathf.Pow(pullSpeed * Time.deltaTime, 2)) {
				player.transform.position += (transform.position - player.transform.position).normalized * pullSpeed * Time.deltaTime;
			}
			cameraFade.renderer.material.color += new Color(0, 0, 0, fadeSpeed * Time.deltaTime);
		}
	}
}
