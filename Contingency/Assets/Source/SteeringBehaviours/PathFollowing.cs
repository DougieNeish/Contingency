using UnityEngine;

public class PathFollowing
{
	public delegate void PathCompletedEventHandler();
	public event PathCompletedEventHandler OnPathCompleted;

	private SteeringController m_steeringController;
	private Path m_path;
	private float m_arriveRadius = 1.3f; // Ensure this is > Arrive.ArriveRadius
	private float m_sqrArriveRadius;

	public Path Path
	{
		get { return m_path; }
	}

	public float ArriveRadius
	{
		get { return m_arriveRadius; }
		set { m_arriveRadius = value; }
	}

	public float SqrArriveRadius
	{
		get { return m_arriveRadius * m_arriveRadius; }
	}

	public PathFollowing(SteeringController steeringController)
	{
		m_steeringController = steeringController;
		m_path = new Path();
	}

	public Vector3 GetSteeringVector()
	{
		Vector3 vectorToWaypoint = m_steeringController.transform.position - m_path.CurrentWaypointPosition;
		float sqrDistance = vectorToWaypoint.sqrMagnitude;

		if (sqrDistance < SqrArriveRadius)
		{
			if (m_path.FinalWaypointSelected)
			{
				m_path.ClearWaypoints();
				m_steeringController.TurnOffBehaviour(SteeringController.BehaviourType.PathFollowing);
				m_steeringController.Rigidbody.velocity = Vector3.zero;

				if (OnPathCompleted != null)
				{
					OnPathCompleted();
				}

				return Vector3.zero;
			}
			else
			{
				m_path.SetNextWaypoint();
			}
		}

		return m_steeringController.Arrive.GetSteeringVector(m_path.CurrentWaypointPosition);
	}
}
