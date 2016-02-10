using UnityEngine;
using System.Collections;

public class DebugInfo : Singleton<DebugInfo>
{
	public bool DrawUnitPaths;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			DrawUnitPaths = !DrawUnitPaths;
		}
	}
}
