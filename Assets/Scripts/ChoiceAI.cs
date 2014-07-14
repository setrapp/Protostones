﻿using UnityEngine;
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
	public float eyeContactBurnout;
	private bool dislikedBurnout;
	public float likingContactFactor;
	public float likeEyeContactRate;
	public float likeEyeContactGrowth;
	public float boredDislikeRate;

	void Start() {
		if (seed <= 0) {
			seed = (int)System.DateTime.Now.Ticks;
		}
		random = new System.Random(seed);

		// Attach affinities to partner's dumps.
		List<TextDump> partnerDumps = dumper.partner.dumps;
		List<TextDump> partnerResponses = dumper.partner.responses;
		for (int i = 0; i < dumpAffinities.Count; i++) {
			bool dumpFound = false;
			for (int j = 0; j < partnerDumps.Count && !dumpFound; j++) {
				if (dumpAffinities[i].dumpName == partnerDumps[j].text) {
					dumpAffinities[i].dump = partnerDumps[j];
					dumpFound = true;
				}

			}
			for (int j = 0; j < partnerResponses.Count && !dumpFound; j++) {
				if (dumpAffinities[i].dumpName == partnerResponses[j].text) {
					dumpAffinities[i].dump = partnerResponses[j];
					dumpFound = true;
				}
			}
		}

		likingText.enabled = showLiking;
		partnerTimer.timerText.enabled = showTimer;

		lastEmote = liking;
		emoted = false;
		eyeContact.maxScaling =  Mathf.Pow(liking, likingContactFactor);
		dislikedBurnout = false;
	}

	void Update() {
		// TODO this should not be player controlled
		if (Input.GetKeyDown(KeyCode.Home)) {
			RandomDump();
		}

		if (liking > 1) {
			liking = 1;
		} else if (liking < 0) {
			liking = 0;
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
				dislikedBurnout = false;
			} else if (eyeContact.Exhausted && !dislikedBurnout) {
				liking -= eyeContactBurnout;
				dislikedBurnout = true;
			} else if (liking > 0) {
				liking -= boredDislikeRate * Time.deltaTime;
			}
		}

		likingText.text = liking.ToString();
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
				bool canLike = (liking >= dumpAffinities[i].minToLike && liking <= dumpAffinities[i].maxToLike);
				bool canDislike = (liking >= dumpAffinities[i].minToDislike && liking <= dumpAffinities[i].maxToDislike);
				bool flipReaction = (random.NextDouble() < dumpAffinities[i].chanceToFlip);
				if ((canLike && !flipReaction) || (canDislike && flipReaction)) {
					liking += dumpAffinities[i].likeChange * likingFactor;
					deltaLiking = dumpAffinities[i].likeChange * likingFactor;
				} else if ((canDislike && !flipReaction) || (canLike && flipReaction)) {
					liking -= dumpAffinities[i].dislikeChange * likingFactor;
					deltaLiking = -dumpAffinities[i].dislikeChange * likingFactor;
				}

				// Determine what emotion to show.
				if (!emoted) {
					emoted = true;
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
					} else {
						if (deltaLiking > 0) {
							// ^_^
							dumper.Emote(1);
						} else if (deltaLiking < 0) {
							// -_-
							dumper.Emote(3);
						}
					}
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
	public string dumpName;
	[HideInInspector]
	public TextDump dump;
	public float minToLike;
	public float maxToLike;
	public float likeChange;
	public float minToDislike;
	public float maxToDislike;
	public float dislikeChange;
	public float chanceToFlip;
	public bool specialResponse;
}