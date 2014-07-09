using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextDumper : MonoBehaviour {
	public List<TextDump> dumps;
	private List<TextDump> potentialDumps;
	public List<DialogChoice> choices; 
	private System.Random random;
	public int seed;

	void Start() {
		if (seed <= 0) {
			seed = (int)System.DateTime.Now.Ticks;
		}
		random = new System.Random(seed);

		// TODO This should be handled conditionally elsewhere, potentialDumps should be a subset
		potentialDumps = new List<TextDump>();
		for (int i = 1; i < dumps.Count; i++) {
			potentialDumps.Add(dumps[i]);
		}

		for (int i = 0; i < choices.Count; i++) {
			choices[i].ChoiceIndex = i.ToString()[0];
		}

		UpdateAllChoices();
	}

	void Update() {
		for (int i = 0; i < choices.Count; i++) {
			if (choices[i].dumpDisplay.enabled && Input.GetKeyDown(choices[i].ChoiceIndex.ToString())) {
				Debug.Log(choices[i].ChoiceIndex);
			}
		}
	}

	private void UpdateChoice(int choiceIndex) {
		if (potentialDumps.Count < 1) {
			choices[choiceIndex].Dump = null;
		} else {
			int dumpIndex;
			dumpIndex = random.Next(0, potentialDumps.Count);
			choices[choiceIndex].Dump = potentialDumps[dumpIndex];
			potentialDumps.RemoveAt(dumpIndex);
		}
	}

	private void UpdateAllChoices() {
		// Zeroth choice is default.
		choices[0].Dump = dumps[0];

		for (int i = 1; i < choices.Count; i++) {
			UpdateChoice(i);
		}
	}
}

[System.Serializable]
public class TextDump {
	public string text;
}