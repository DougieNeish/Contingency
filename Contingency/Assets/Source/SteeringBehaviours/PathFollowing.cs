using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathFollowing
{
	private SteeringController m_steeringController;
	private Path m_path;
	private float m_arriveRadius = 0.005f;

	public Path Path
	{
		get { return m_path; }
	}

	public float ArriveRadius
	{
		get { return m_arriveRadius; }
		set { m_arriveRadius = value; }
	}

	public PathFollowing(SteeringController steeringController)
	{
		m_steeringController = steeringController;
		m_path = new Path();
	}

	public Vector3 GetSteeringVector()
	{
		float sqrArriveRadius = m_arriveRadius * m_arriveRadius;

		if ((m_steeringController.transform.position - m_path.CurrentWaypoint).magnitude < sqrArriveRadius)
		{
			m_path.SetNextWaypoint();
		}

		if (!m_path.Finished())
		{
			return m_steeringController.Seek.GetSteeringVector(m_path.CurrentWaypoint);
		}
		else
		{
			return m_steeringController.Arrive.GetSteeringVector(m_path.CurrentWaypoint);
		}
	}
}
