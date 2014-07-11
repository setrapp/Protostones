using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChoiceInput : MonoBehaviour {
	public TextDumper dumper;
	public PartnerTimer timerFrom;
	public PartnerTimer timerTo;

	void Update () {
		List<DialogChoice> choices = dumper.choices;
		for (int i = 0; i < choices.Count; i++) {
			if (choices[i].dumpDisplay.enabled && Input.GetKeyDown(choices[i].ChoiceIndex.ToString())) {
				if (!timerTo.isActive) {
					StartTimers();
				}
				dumper.DumpText(choices[i].Dump);
				break;
			}
		}

		if (Input.GetKeyDown(KeyCode.Space)) {
			if (!timerTo.isActive) {
				StartTimers();
			} else {
				timerFrom.AddTime();
			}
		}

		// TODO this should be in ChoiceAI controlled by a formula like secondsLeft + secondElapsed < (liking * likingFactor) -> add time*/
		if (Input.GetKeyDown(KeyCode.Home)) {
			timerTo.AddTime();
		}
	}

	private void StartTimers() {
		timerTo.isActive = true;
		timerFrom.isActive = true;
	}
}
