using UnityEngine;
using System.Collections;

public class Flee
{
	private SteeringController m_steeringController;

	public Flee(SteeringController steeringController)
	{
		m_steeringController = steeringController;
	}

	public Vector3 GetSteeringVector(Vector3 targetPosition)
	{
		return GetSteeringVector(targetPosition, 0.0f);
	}

	public Vector3 GetSteeringVector(Vector3 targetPosition, float activationDistance)
	{
		float distanceFromTarget = Vector3.Distance(m_steeringController.Agent.transform.position, targetPosition);
		if (activationDistance > 0.0f && distanceFromTarget > activationDistance)
		{
			return Vector3.zero;
		}

		Vector3 desiredVelocity = Vector3.Normalize(m_steeringController.Agent.transform.position - targetPosition) * m_steeringController.MaxSpeed;
		return (desiredVelocity - m_steeringController.Rigidbody.velocity);
	}
}
