using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChoiceAI : MonoBehaviour {
	public TextDumper dumper;
	private System.Random random;
	public int seed;
	public List<DumpAffinity> dumpAffinities;
	public float liking;
	public GUIText likingText;
	public bool showLiking;
	public GUIText timerText;
	public bool showTimer;

	void Start() {
		if (seed <= 0) {
			seed = (int)System.DateTime.Now.Ticks;
		}
		random = new System.Random(seed);

		// Attach affinities to partners dumps.
		List<TextDump> partnerDumps = dumper.partner.dumps;
		List<TextDump> partnerResponses = dumper.partner.responses;
		for (int i = 0; i < partnerDumps.Count; i++) {
			dumpAffinities[i].dump = partnerDumps[i];
		}
		int startingIndex = partnerDumps.Count;
		for (int i = startingIndex; i < partnerResponses.Count + startingIndex; i++) {
			dumpAffinities[i].dump = partnerResponses[i - startingIndex];
		}


		likingText.enabled = showLiking;
		timerText.enabled = showTimer;
	}

	void Update() {
		// TODO this should not be player controlled
		if (Input.GetKeyDown(KeyCode.End)) {
			RandomDump();
		}

		likingText.text = liking.ToString ();
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

	void RespondToDump(TextDump dump) {
		bool dumpFound = false;
		for (int i = 0; i < dumpAffinities.Count && !dumpFound; i++) {
			if (dumpAffinities[i].dump == dump) {
				dumpFound = true;
				if (liking >= dumpAffinities[i].minToLike && liking <= dumpAffinities[i].maxToLike) {
					liking += dumpAffinities[i].likeChange;
				} else if (liking >= dumpAffinities[i].minToDislike && liking <= dumpAffinities[i].maxToDislike) {
					liking -= dumpAffinities[i].dislikeChange;
				}
			}
		}

		/*TODO End if liking is hits 0*/
	}
}

[System.Serializable]
public class DumpAffinity {
	public TextDump dump;
	public string dumpName; // Just need to see name in editor
	public float minToLike;
	public float maxToLike;
	public float likeChange;
	public float minToDislike;
	public float maxToDislike;
	public float dislikeChange;
}