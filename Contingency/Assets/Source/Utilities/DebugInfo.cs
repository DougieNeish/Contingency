using UnityEngine;
using System.Collections;

public class DebugInfo : Singleton<DebugInfo>
{
	[Header("Pathfinding")]
	public bool DrawUnitPaths;
	public bool DrawAIPaths;

	[Header("Navigation")]
	public bool DrawNodes;
	public bool DrawEdges;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			DrawUnitPaths = !DrawUnitPaths;
		}

		if (Input.GetKeyDown(KeyCode.LeftBracket))
		{
			DrawAIPaths = !DrawAIPaths;
		}
	}
}
