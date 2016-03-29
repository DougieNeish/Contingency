using UnityEngine;
using System.Collections.Generic;

public class ObstacleAvoidance
{
	private readonly SteeringController m_steeringController;
	private HashSet<GameObject> m_obstacles;

	public HashSet<GameObject> Obstacles
	{
		get { return m_obstacles; }
		set { m_obstacles = value; }
	}

	private class ClosestObstacle
	{
		public GameObject gameObject = null;
		public float radius = 0f;
		public float timeToCollision = float.MaxValue;
		public Vector3 separation = Vector3.zero;
        public float distanceFromAgent = 0f;
		public Vector3 relativePosition = Vector3.zero;
		public Vector3 relativeVelocity = Vector3.zero;
	}

	public ObstacleAvoidance(SteeringController steeringController)
	{
		m_steeringController = steeringController;
		m_obstacles = new HashSet<GameObject>();
    }

	public Vector3 GetSteeringVector()
	{
		ClosestObstacle closestObstacle = new ClosestObstacle();
		float agentRadius = m_steeringController.Radius;

		foreach (GameObject obstacle in m_obstacles)
		{
			if (obstacle == null || obstacle == m_steeringController.gameObject)
			{
				continue;
			}

			// Calculate the time to collision
			Vector3 relativePosition = m_steeringController.transform.position - obstacle.transform.position;

			Rigidbody obstacleRigidbody = obstacle.GetComponent<Rigidbody>();
			Rigidbody agentRigidbody = m_steeringController.Rigidbody;
			Vector3 relativeVelocity = obstacleRigidbody ? agentRigidbody.velocity - obstacleRigidbody.velocity : agentRigidbody.velocity;

			float distanceFromAgent = relativePosition.magnitude;
			float relativeSpeed = relativeVelocity.magnitude;

			if (relativeSpeed == 0)
			{
				continue;
			}

			float timeToCollision = -1 * Vector3.Dot(relativePosition, relativeVelocity) / (relativeSpeed * relativeSpeed);

			// Check if agent and obstacle are going to collide
			Vector3 separation = relativePosition + relativeVelocity * timeToCollision;

			float obstacleRadius = SteeringUtils.GetBoundingRadius(obstacle.transform);
			float combinedRadius = agentRadius + obstacleRadius;
			float sqrCombinedRadius = combinedRadius * combinedRadius;

			if (separation.sqrMagnitude > sqrCombinedRadius)
			{
				continue;
			}

			// Check if this is the closest obstacle
			if (timeToCollision > 0 && timeToCollision < closestObstacle.timeToCollision)
			{
				closestObstacle.gameObject = obstacle;
				closestObstacle.radius = obstacleRadius;
				closestObstacle.timeToCollision = timeToCollision;
				closestObstacle.separation = separation;
				closestObstacle.distanceFromAgent = distanceFromAgent;
				closestObstacle.relativePosition = relativePosition;
				closestObstacle.relativeVelocity = relativeVelocity;
			}
		}

		// Calculate steering to closestObstacle
		Vector3 steeringVector = Vector3.zero;

		if (closestObstacle.gameObject == null)
		{
			return steeringVector;
		}

		// If agent will collide with no separation or it is already colliding, steer based on current position
		if (closestObstacle.separation.sqrMagnitude <= 0 || closestObstacle.distanceFromAgent < agentRadius + closestObstacle.radius)
		{
			steeringVector = m_steeringController.transform.position - closestObstacle.gameObject.transform.position;
		}
		else
		{
			// Otherwise steer based on future relative position
			steeringVector = closestObstacle.relativePosition + closestObstacle.relativeVelocity * closestObstacle.timeToCollision;
		}

		steeringVector.Normalize();
		steeringVector *= m_steeringController.MaxAcceleration;

		return steeringVector;
	}
}
