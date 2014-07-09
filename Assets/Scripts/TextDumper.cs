using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextDumper : MonoBehaviour {
	public List<TextDump> dumps;
	private List<TextDump> potentialDumps;
	public List<DialogChoice> choices; 
	private System.Random random;
	public int seed;
	private int guaranteedDumps;
	public GUIText dumpDisplay;

	void Start() {
		if (seed <= 0) {
			seed = (int)System.DateTime.Now.Ticks;
		}
		random = new System.Random(seed);

		potentialDumps = new List<TextDump>();
		ResetPotentialDumps();

		for (int i = 0; i < choices.Count; i++) {
			choices[i].ChoiceIndex = i.ToString()[0];
		}

		guaranteedDumps = 1;
		UpdateAllChoices();
	}

	private void UpdateChoice(int choiceIndex, int dumpIndex) {
		choices[choiceIndex].Dump = potentialDumps[dumpIndex];
		potentialDumps.RemoveAt(dumpIndex);
	}

	private void UpdateAllChoices() {
		for (int i = 0; i < guaranteedDumps; i++) {
			if (potentialDumps.Count > 0 && i < choices.Count) {
				UpdateChoice(i, 0);
			} else {
				choices[i].Dump = null;
			}
		}

		for (int i = guaranteedDumps; i < choices.Count; i++) {
			if (potentialDumps.Count > 0) {
				int dumpIndex;
				dumpIndex = random.Next(0, potentialDumps.Count);
				UpdateChoice(i, dumpIndex);
			} else {
				choices[i].Dump = null;
			}
		}
	}

	public bool DumpText(TextDump dump) {
		if (!dumps.Contains(dump)) {
			return false;
		}

		dumpDisplay.enabled = true;
		dumpDisplay.text = dump.text;

		ResetPotentialDumps();

		// TODO might not need to happen here
		UpdateAllChoices();

		return true;
	}

	private void ResetPotentialDumps() {
		potentialDumps.Clear();
		for (int i = 0; i < dumps.Count; i++) {
			if (dumps[i].isCommon) {
				potentialDumps.Add(dumps[i]);
			}
		}
	}
}

[System.Serializable]
public class TextDump {
	public string text;
	public bool isCommon;
}