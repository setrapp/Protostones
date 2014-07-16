using UnityEngine;
using System.Collections;

public class Helper {
	public static float ClampAngle(float angle, float min, float max) {
		if (Mathf.Abs(angle) > 360) {
			int truncated = (int)angle;
			float truncatee = angle - truncated;
			truncated %= 360;
			angle = truncated + truncatee;
		}
		return Mathf.Clamp(angle, min, max);
	}
}