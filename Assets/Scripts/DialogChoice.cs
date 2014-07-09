using UnityEngine;
using System.Collections;

public class DialogChoice : MonoBehaviour {
	public GUIText keyDisplay;
	private char choiceKey;
	public char ChoiceIndex {
		get { return choiceKey; }
		set {
			choiceKey = value;
			keyDisplay.text = choiceKey.ToString();
		}
	}
	public GUIText dumpDisplay;
	private TextDump dump;
	public TextDump Dump {
		get { return dump; }
		set {
			dump = value; 
			if (dump == null || dump.text == null) {
				keyDisplay.enabled = false;
				dumpDisplay.enabled = false;
			} else {
				keyDisplay.enabled = true;
				dumpDisplay.enabled = true;
				dumpDisplay.text = dump.text;
			}
		}
	}
}
