using UnityEngine;
using System.Collections;

public class HealthReaction : MonoBehaviour {
	private CharacterController controller;
	private HealthTracker tracker;
	private SimpleMotor motor;
	private bool fullHealth;
	private bool emptyHealth;
	public float ascendSpeed;
	public float fadeSpeed;
	public Material fadeMaterial;
	private Material normalMaterial;
	private bool fading;
	public GameObject cameraFade;

	void Start() {
		controller = GetComponent<CharacterController>();
		tracker = GetComponent<HealthTracker>();
		motor = GetComponent<SimpleMotor>();
		normalMaterial = renderer.material;
	}

	void Update() {
		if (fullHealth) {
			controller.Move(Vector3.up * (ascendSpeed + motor.gravity) * Time.deltaTime);
			cameraFade.renderer.material.color += new Color(0, 0, 0, fadeSpeed * Time.deltaTime);
		} else if (emptyHealth) {
			cameraFade.renderer.material.color += new Color(0, 0, 0, fadeSpeed * Time.deltaTime);
		} else if (fading) {
			if (renderer.material.color.a <= 0) {
				Color color = cameraFade.renderer.material.color;
				color.a = 0;
				cameraFade.renderer.material.color = color;
				fading = false;
			} else {
				cameraFade.renderer.material.color -= new Color(0, 0, 0, fadeSpeed * Time.deltaTime);
			}
		}
	}

	void FullHealth() {
		fullHealth = true;
		fading = true;
		cameraFade.renderer.material.color = new Color(1, 1, 1, 0);
	}

	void EmptyHealth() {
		emptyHealth = true;
		fading = true;
		cameraFade.renderer.material.color = new Color(0, 0, 0, 0);
	}

	void NormalHealth() {
		// Unfade material
		fullHealth = false;
		emptyHealth = false;

	}
}
