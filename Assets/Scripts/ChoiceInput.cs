using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChoiceInput : MonoBehaviour {
	public TextDumper dumper;

	void Update () {
		List<DialogChoice> choices = dumper.choices;
		for (int i = 0; i < choices.Count; i++) {
			if (choices[i].dumpDisplay.enabled && Input.GetKeyDown(choices[i].ChoiceIndex.ToString())) {
				dumper.DumpText(choices[i].Dump);
				break;
			}
		}
	}
}
