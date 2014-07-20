using UnityEngine;
using System.Collections;

public class HealthTracker : MonoBehaviour {
	public float startHealth = 0.5f;
	private float health;
	public float Health {
		get { return health; }
	}
	public float alterMod = 1;
	public Color fullColor = Color.white;
	public Color halfColor = Color.gray;
	public Color emptyColor = Color.black;
	public GUIText textDisplay;

	void Start() {
		health = Mathf.Clamp(startHealth, 0, 1);
		if (textDisplay && textDisplay.enabled) {
			textDisplay.text = health.ToString();
		}
	}

	void Update() {
		if (textDisplay && textDisplay.enabled) {
			textDisplay.text = health.ToString();
		}

		float halfHealth = 0.5f;
		float healthFromHalf = (health - halfHealth) * 2;

		if (healthFromHalf >= 0) {
			renderer.material.color = (fullColor * healthFromHalf) + (halfColor * (1 - healthFromHalf));
		} else {
			renderer.material.color = (emptyColor * -healthFromHalf) + (halfColor * (1 + healthFromHalf));
		}
	}

	public void AlterHealth(float alteration) {
		if ((alteration > 0 && health >= 1) || (alteration < 0 && health <= 0)) {
			return;
		}

		bool wasExtreme = false;
		if (health >= 1 || health <= 0) {
			wasExtreme = true;
		}

		alteration *= alterMod;
		health += alteration;
		if (health >= 1) {
			health = 1;
			SendMessage("FullHealth", SendMessageOptions.DontRequireReceiver);
		} else if (health <= 0) {
			health = 0;
			SendMessage("EmptyHealth", SendMessageOptions.DontRequireReceiver);
		} else if (wasExtreme) {
			SendMessage("NormalHealth", SendMessageOptions.DontRequireReceiver);
		}
	}
}
