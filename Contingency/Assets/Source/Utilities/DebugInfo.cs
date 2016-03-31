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

	[Header("Line of sight")]
	public bool SightRendering;

	[Header("AI")]
	public bool AutoSpawnAI;

	private Unit m_lastSelectedUnit;

	public Unit LastSelectedUnit
	{
		get { return m_lastSelectedUnit; }
		set { m_lastSelectedUnit = value; }
	}

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

		if (m_lastSelectedUnit != null)
		{
			if (Input.GetKeyDown(KeyCode.J))
			{
				m_lastSelectedUnit.Stance = Unit.CombatStance.Aggressive;
			}
			else if (Input.GetKeyDown(KeyCode.K))
			{
				m_lastSelectedUnit.Stance = Unit.CombatStance.Defensive;
			}
			else if (Input.GetKeyDown(KeyCode.L))
			{
				m_lastSelectedUnit.Stance = Unit.CombatStance.Static;
			}
		}

		if (Input.GetKeyDown(KeyCode.V))
		{
			SightRendering = !SightRendering;
		}
	}
}
