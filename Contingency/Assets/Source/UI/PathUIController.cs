using UnityEngine;
using System.Collections.Generic;

public class PathUIController : MonoBehaviour
{
	private Unit m_unit;
	private SteeringController m_steeringController;
	private Path m_path;
	private LineRenderer m_line;

	[SerializeField] private float m_lineWidth;

	void Awake()
	{
		m_unit = GetComponentInParent<Unit>();
		m_steeringController = GetComponentInParent<SteeringController>();
		m_path = m_steeringController.PathFollowing.Path;
		m_line = GetComponent<LineRenderer>();
	}

	void OnEnable()
	{
		m_path.OnWaypointAdded += CalculatePathMarker;
		m_path.OnCurrentWaypointUpdated += CalculatePathMarker;
	}

	void OnDisable()
	{
		m_path.OnWaypointAdded -= CalculatePathMarker;
		m_path.OnCurrentWaypointUpdated -= CalculatePathMarker;
	}

	void Update()
	{
		m_line.SetPosition(0, transform.position);
	}

	private void CalculatePathMarker(List<Vector3> waypoints, int currentWaypoint)
	{
		if (!DebugInfo.Instance.DrawUnitPaths)
		{
			m_line.enabled = false;
			return;
		}

		// Don't draw path if there are no waypoints, or if the player is an AI and AI path drawing is disable
		if (waypoints.Count < 1 || (!DebugInfo.Instance.DrawAIPaths && m_unit.Owner.Type == Player.PlayerType.AI))
		{
			m_line.enabled = false;
			return;
		}

		int remainingWaypoints = waypoints.Count - currentWaypoint;
		m_line.SetVertexCount(remainingWaypoints + 1);
		m_line.SetWidth(m_lineWidth, m_lineWidth);
		m_line.useWorldSpace = true;

		// Add line from unit position to first waypoint
		int vertexIndex = 0;
		m_line.SetPosition(vertexIndex++, transform.position);

		for (int i = currentWaypoint; i < waypoints.Count; i++)
		{
			m_line.SetPosition(vertexIndex++, waypoints[i]);
		}

		m_line.enabled = true;
	}
}
