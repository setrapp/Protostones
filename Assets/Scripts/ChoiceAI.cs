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
	public PartnerTimer partnerTimer;
	public bool showTimer;
	public float emoteThreshold;
	public float lastEmote;
	private bool emoted;
	public EyeContact eyeContact;
	public float eyeContactBoost;
	public float likingContactFactor;
	public float likeEyeContactRate;
	public float likeEyeContactGrowth;
	public float boredDislikeRate;

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
		partnerTimer.timerText.enabled = showTimer;

		lastEmote = liking;
		emoted = false;
		eyeContact.maxScaling =  Mathf.Pow(liking, likingContactFactor);
	}

	void Update() {
		// TODO this should not be player controlled
		if (Input.GetKeyDown(KeyCode.Home)) {
			RandomDump();
		}

		if (liking > 0.5f) {
			int desiredSeconds = (int)(Mathf.Sqrt(((liking - 0.5f) * 2) + 1) * partnerTimer.maxSecondsLeft);
			int projectedSeconds = partnerTimer.secondsElapsed + partnerTimer.secondsLeft;
			if (desiredSeconds > projectedSeconds) {
				partnerTimer.AddTime();
			}
		}

		if (partnerTimer.isActive) {
			if (eyeContact.Contacting) {
				liking += Mathf.Pow (liking, likeEyeContactGrowth) * likeEyeContactRate * Time.deltaTime;
			} else {
				liking -= boredDislikeRate * Time.deltaTime;
			}
		}

		likingText.text = liking.ToString();

		if (!emoted) {
			emoted = true;
			float likingChange = liking - lastEmote;
			if (likingChange >= emoteThreshold) {
				dumper.Emote(1);
			} else if (likingChange <= -emoteThreshold) {
				dumper.Emote(3);
			}
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

	void RespondToDump(TextDump dump) {
		bool dumpFound = false;
		emoted = false;
		for (int i = 0; i < dumpAffinities.Count && !dumpFound; i++) {
			if (dumpAffinities[i].dump == dump) {
				// Update liking.
				dumpFound = true;
				float deltaLiking = 0;
				float likingFactor = 1;
				if (eyeContact.Contacting && dump.eyeContactBoosted && !dump.eyeContactOnly) {
					likingFactor = eyeContactBoost;
				}
				if (liking >= dumpAffinities[i].minToLike && liking <= dumpAffinities[i].maxToLike) {
					liking += dumpAffinities[i].likeChange * likingFactor;
					deltaLiking = dumpAffinities[i].likeChange * likingFactor;
				} else if (liking >= dumpAffinities[i].minToDislike && liking <= dumpAffinities[i].maxToDislike) {
					liking -= dumpAffinities[i].dislikeChange * likingFactor;
					deltaLiking = -dumpAffinities[i].dislikeChange * likingFactor;
				}

				// Determine if special emote should be given as feedback.
				if (dumpAffinities[i].specialResponse) {
					if (deltaLiking > 0) {
						// ^o^
						dumper.Emote(2);
					} else if (deltaLiking < 0) {
						// >_<
						dumper.Emote(4);
					} else {
						// -_^
						dumper.Emote(5);
					}

					emoted = true;
				}

				// Update eye contact stats.
				eyeContact.maxScaling = Mathf.Pow(liking, likingContactFactor);
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
	public bool specialResponse;
}