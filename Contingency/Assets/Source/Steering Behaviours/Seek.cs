using UnityEngine;
using System.Collections;

//[RequireComponent (typeof (SteeringController))]
public class Seek
{
	private SteeringController m_steeringController;

	public Seek(SteeringController steeringController)
	{
		m_steeringController = steeringController;
	}

	public Vector3 GetSteeringVector(Vector3 targetPosition)
	{
		Vector3 desiredVelocity = Vector3.Normalize(targetPosition - m_steeringController.Agent.transform.position) * m_steeringController.MaxSpeed;
		return (desiredVelocity - m_steeringController.Rigidbody.velocity);
	}
}
