﻿using UnityEngine;
using System.Collections.Generic;

public class Path
{
	private List<Vector3> m_waypoints;
	private int m_currentWaypoint;
	private bool m_loop;
	private bool m_finalWaypointSelected;

	public List<Vector3> Waypoints
	{
		get { return m_waypoints; }
		set { m_waypoints = value; }
	}

	public Vector3 CurrentWaypoint
	{
		get { return m_waypoints[m_currentWaypoint]; }
	}

	public bool Loop
	{
		get { return m_loop; }
		set { m_loop = value; }
	}

	public bool FinalWaypointSelected
	{
		get { return m_finalWaypointSelected; }
		set { m_finalWaypointSelected = value; }
	}

	public Path()
	{
		m_waypoints = new List<Vector3>();
		m_loop = false;
		m_finalWaypointSelected = false;
	}

	public void AddWaypoint(Vector3 newPoint)
	{
		m_waypoints.Add(newPoint);
	}

	public void SetWaypoints(List<Vector3> newPath)
	{
		m_waypoints = newPath;
		m_currentWaypoint = 0;
	}
	public void SetWaypoints(Path path)
	{
		m_waypoints = path.Waypoints;
		m_currentWaypoint = 0;
	}

	public void ClearWaypoints()
	{
		m_waypoints.Clear();
		m_currentWaypoint = 0;
		m_finalWaypointSelected = false;
	}

	public void SetNextWaypoint()
	{
		if (m_waypoints.Count > 0)
		{
			if (++m_currentWaypoint >= m_waypoints.Count - 1)
			{
				if (m_loop)
				{
					m_currentWaypoint = 0;
				}
				else
				{
					m_currentWaypoint = m_waypoints.Count - 1;
					m_finalWaypointSelected = true;
				}
			}
		}
	}

	public bool Finished()
	{
		return (m_waypoints[m_currentWaypoint] == m_waypoints[m_waypoints.Count - 1]);
	}
};