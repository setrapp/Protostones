using UnityEngine;
using System.Collections;

public class EnableOnStart : MonoBehaviour {
	public GameObject target;

	void Start() {
		target.SetActive(true);
	}
}
