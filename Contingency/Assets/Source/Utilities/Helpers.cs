using UnityEngine;
using System.Collections;

public static class Helpers
{
	public static T GetComponentInChildWithTag<T>(this GameObject parent, string tag) where T : Component
	{
		Transform parentTransform = parent.transform;
		foreach (Transform child in parentTransform)
		{
			if (child.tag == tag)
			{
				return child.GetComponent<T>();
			}
		}

		return null;
	}
}
