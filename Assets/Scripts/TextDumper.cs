using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextDumper : MonoBehaviour {
	public List<TextDump> dumps;
	public List<TextDump> responses;
	private List<TextDump> potentialDumps;
	public List<DialogChoice> choices; 
	private System.Random random;
	public int seed;
	private int guaranteedDumps;
	public GUIText dumpDisplay;
	public TextDumper partner;
	public bool leading;

	void Start() {
		if (seed <= 0) {
			seed = (int)System.DateTime.Now.Ticks;
		}
		random = new System.Random(seed);

		for (int i = 0; i < choices.Count; i++) {
			choices[i].ChoiceIndex = i.ToString()[0];
		}

		potentialDumps = new List<TextDump>();
		ResetPotentialDumps();
		if (leading) {
			dumpDisplay.text = potentialDumps[0].text;
		} else {
			dumpDisplay.text = potentialDumps[1].text;
		}
		UpdateAllChoices();
	}

	public void UpdateChoice(int choiceIndex, int dumpIndex) {
		choices[choiceIndex].Dump = potentialDumps[dumpIndex];
		potentialDumps.RemoveAt(dumpIndex);
	}

	public void UpdateAllChoices() {
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

		partner.transform.parent.SendMessage("RespondToDump", dump, SendMessageOptions.DontRequireReceiver);

		if (dump.changesLead) {
			if (leading) {
				YieldLead();
			} else {
				TakeLead();
			}
		}

		ResetPotentialDumps();
		UpdateAllChoices();

		return true;
	}

	public void ResetPotentialDumps() {
		potentialDumps.Clear();
		int guarantees = 0;

		if (!leading) {
			for (int i = 0; i < responses.Count; i++) {
				if (responses[i].isCommon) {
					potentialDumps.Add(responses[i]);
					if (responses[i].isGuaranteed) {
						guarantees++;
					}
				}
			}

		} else {
			for (int i = 0; i < dumps.Count; i++) {
				if (dumps[i].isCommon) {
					potentialDumps.Add(dumps[i]);
					if (dumps[i].isGuaranteed) {
						guarantees++;
					}
				}
			}
		}

		guaranteedDumps = guarantees;
	}

	public void YieldLead() {
		leading = false;
		partner.leading = true;
		partner.ResetPotentialDumps();
		partner.UpdateAllChoices();
	}

	public void TakeLead() {
		leading = true;
		partner.leading = false;
		partner.ResetPotentialDumps();
		partner.UpdateAllChoices();
	}

	public void Emote(int emoteIndex) {
		DumpText(responses[emoteIndex]);
	}
}

[System.Serializable]
public class TextDump {
	public string text;
	public bool changesLead;
	public bool isCommon;
	public bool isGuaranteed;
}