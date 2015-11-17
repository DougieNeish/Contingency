using UnityEngine;
using System.Collections;

public class Arrive
{
	private SteeringController m_steeringController;

	public Arrive(SteeringController steeringController)
	{
		m_steeringController = steeringController;
	}

	// TODO: Change enum SteeringController.Deceleration and decelerationTweaker
	public Vector3 GetSteeringVector(Vector3 targetPosition, SteeringController.Deceleration deceleration)
	{
		Vector3 toTarget = targetPosition - m_steeringController.Agent.transform.position;
		float distanceToTarget = toTarget.magnitude;

		if (distanceToTarget > 0.0f)
		{
			const float decelerationTweaker = 0.3f;

			// Calculate the speed required to reach targetPosition given the desired deceleration
			float speed = distanceToTarget / (float)deceleration * decelerationTweaker;

			// Ensure speed does not exceed MaxSpeed
			speed = Mathf.Min(speed, m_steeringController.MaxSpeed);

			Vector3 desiredVelocity = toTarget * m_steeringController.MaxSpeed / distanceToTarget;
			return (desiredVelocity - m_steeringController.Rigidbody.velocity);
		}

		return Vector3.zero;
	}
}
