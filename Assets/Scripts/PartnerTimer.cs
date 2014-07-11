using UnityEngine;
using System.Collections;

public class PartnerTimer : MonoBehaviour {
	public GUIText timerText;
	[HideInInspector]
	public int secondsLeft;
	public int maxSecondsLeft;
	public int secondsElapsed;
	private float prevSecond = -1;
	public bool isActive;
	public int secondAddIncrement;

	void Start() {
		isActive = false;
		secondsLeft = maxSecondsLeft;
		DisplayTime();
	}

	void Update() {
		if (isActive) {
			if (secondsLeft <= 0) {
				/*TODO figure out end results*/
			} else if (prevSecond < 0 || Time.time - prevSecond >= 1) {
				prevSecond = Time.time;
				secondsLeft--;
				secondsElapsed++;
			}
		}

		DisplayTime();
	}

	private void DisplayTime() {
		timerText.text = "" + (secondsLeft / 60) + ":" + (secondsLeft % 60);
	}

	public void AddTime() {
		secondsLeft += secondAddIncrement;
		if (secondsLeft > maxSecondsLeft) {
			secondsLeft = maxSecondsLeft;
		}
	}
}
