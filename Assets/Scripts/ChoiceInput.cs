using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChoiceInput : MonoBehaviour {
	public TextDumper dumper;
	public PartnerTimer partnerTimer;
	public PartnerTimer meTimer;

	void Update () {
		List<DialogChoice> choices = dumper.choices;
		for (int i = 0; i < choices.Count; i++) {
			if (choices[i].dumpDisplay.enabled && Input.GetKeyDown(choices[i].ChoiceIndex.ToString())) {
				if (!partnerTimer.isActive) {
					StartTimers();
				}
				dumper.DumpText(choices[i].Dump);
				break;
			}
		}

		if (Input.GetKeyDown(KeyCode.Space)) {
			if (!partnerTimer.isActive) {
				StartTimers();
			} else {
				partnerTimer.AddTime();
			}
		}
	}

	private void StartTimers() {
		partnerTimer.isActive = true;
		meTimer.isActive = true;
	}
}
