using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChoiceAI : MonoBehaviour {
	public TextDumper dumper;
	private System.Random random;
	public int seed;

	void Start() {
		if (seed <= 0) {
			seed = (int)System.DateTime.Now.Ticks;
		}
		random = new System.Random(seed);
	}

	void Update() {
		/* TODO this should not be player controlled*/
		if (Input.GetKeyDown(KeyCode.Space)) {
			RandomDump();
		}
	}

	private void RandomDump() {
		List<DialogChoice> validChoices = new List<DialogChoice>();
		for (int i = 0; i < dumper.choices.Count; i++) {
			if (dumper.choices[i].Dump != null) {
				validChoices.Add(dumper.choices[i]);
			}
		}
		int dumpIndex = random.Next(0, validChoices.Count);
		dumper.DumpText(validChoices[dumpIndex].Dump);
	}
}
