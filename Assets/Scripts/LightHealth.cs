using UnityEngine;
using System.Collections;

public class LightHealth : MonoBehaviour {
	public float maxHealth;
	public float health;
	public Light controlledLight;
	private float maxIntensity;
	private float maxSpotAngle;
	public bool controlIntensity;
	public bool controlSpotAngle;
	public SimpleMover mover;
	public float recoverDelay;
	public float postDamageDelay;
	public float lastRecoverTime;
	public float lastDamageTime;
	public float mobileDamage;
	public float stillRecover;
	public float stillThreshold;


	void Start() {
		health = maxHealth;
		maxIntensity = controlledLight.intensity;
		maxSpotAngle = controlledLight.spotAngle;
	}

	void Update() {
		if ((lastRecoverTime < 0 || Time.time - lastRecoverTime >= recoverDelay) &&
		    (lastDamageTime < 0 || Time.time - lastDamageTime >= postDamageDelay))
		{
			if (mover.currentSpeed / mover.maxSpeed <= stillThreshold) {
				RecoverDamage(stillRecover);
			} else {
				/*TODO If damage should be taken from moving for too long, this should be separated from healing*/
				TakeDamage(mobileDamage);
			}
		}
	}

	void RecoverDamage(float recovery) {
		if (health < maxHealth) {
			health += recovery;
			lastRecoverTime = Time.time;
			UpdateLight();
		}
		if (health > maxHealth){
			health = maxHealth;
		}
	}

	void TakeDamage(float damage) {
		if (health > 0) {
			health -= damage;
			lastDamageTime = Time.time;
			UpdateLight();
		}
		if (health < 0){
			health = 0;
		}
	}

	private void UpdateLight() {
		if (controlIntensity) {
			controlledLight.intensity = maxIntensity * (health / maxHealth);
		}
		if (controlSpotAngle) {
			controlledLight.spotAngle = maxSpotAngle * (health / maxHealth);
		}
	}
}
