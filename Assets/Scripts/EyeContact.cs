using UnityEngine;
using System.Collections;

public class EyeContact : MonoBehaviour {
	public GameObject eyeGauge;
	private Vector3 gaugeScale;
	private float scaling;
	public float maxScaling;
	public float MaxScaling {
		get { return MaxScaling; }
		set {
			maxScaling = value;
			if (scaling > maxScaling) {
				scaling = maxScaling;
			}
		}
	}
	public float minScaling;
	public float downScaleRate;
	public float upScaleRate;
	public float upScaleDelay;
	private float lastDownScaleTime = -1;
	public bool Exhausted {
		get { return (scaling <= minScaling); }
	}
	private bool contacting;
	public bool Contacting {
		get { return contacting; }
		set 
		{
			contacting = value;
			if (contacting) {
				renderer.material = contactingMaterial;
				eyeGauge.renderer.material = gaugeContactingMaterial;
			} else {
				renderer.material = restingMaterial;
				eyeGauge.renderer.material = gaugeRestingMaterial;
			}
		}
	}
	public Material restingMaterial;
	public Material gaugeRestingMaterial;
	public Material contactingMaterial;
	public Material gaugeContactingMaterial;

	void Start() {
		gaugeScale = eyeGauge.transform.localScale;
		scaling = maxScaling;
		contacting = false;
		renderer.material = restingMaterial;
		eyeGauge.renderer.material = gaugeRestingMaterial;
	}

	void Update() {

		if (contacting) {
			ScaleDown();
		} else if (scaling < maxScaling && (lastDownScaleTime < 0 || Time.time - lastDownScaleTime > upScaleDelay)) {
			ScaleUp();
		}

		if (scaling > maxScaling) {
			scaling = maxScaling;
		} else if (scaling < minScaling) {
			scaling = minScaling;
			Contacting = false;
		}

		eyeGauge.transform.localScale = gaugeScale * scaling;
	}

	private void ScaleDown() {
		if (scaling > minScaling) {
			scaling -= downScaleRate * Time.deltaTime;
			lastDownScaleTime = Time.time;
		}
	}

	private void ScaleUp() {
		if (scaling < maxScaling) {
			scaling += upScaleRate * Time.deltaTime;
		}
	}
}
