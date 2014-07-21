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
	public GameObject escort;
	private Vector3 startPosition;
	private Quaternion startRotation;

	void Start() {
		controller = GetComponent<CharacterController>();
		tracker = GetComponent<HealthTracker>();
		motor = GetComponent<SimpleMotor>();
		normalMaterial = renderer.material;
		startPosition = transform.position;
		startRotation = transform.rotation;
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

		if (fading && cameraFade.renderer.material.color.a >= 1.3) {
			cameraFade.renderer.material.color -= new Color(0, 0, 0, cameraFade.renderer.material.color.a - 1);
			Reset();
		}
	}

	void FullHealth() {
		fullHealth = true;
		fading = true;
		cameraFade.renderer.material.color = new Color(1, 1, 1, Mathf.Max(cameraFade.renderer.material.color.a, 0));
	}

	void EmptyHealth() {
		emptyHealth = true;
		fading = true;
		cameraFade.renderer.material.color = new Color(0, 0, 0, Mathf.Max(cameraFade.renderer.material.color.a, 0));
	}

	void NormalHealth() {
		fullHealth = false;
		emptyHealth = false;
	}

	private void Reset() {
		escort.SendMessage("Reset", SendMessageOptions.DontRequireReceiver);
		transform.position = startPosition;
		transform.rotation = startRotation;
		tracker.ResetHealth();
		fullHealth = false;
		emptyHealth = false;
	}
}
