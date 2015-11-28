using UnityEngine;
using System.Collections;

public class Seek
{
	protected readonly SteeringController m_steeringController;
	private Vector3 m_targetPosition;

	public Vector3 TargetPosition
	{
		get { return m_targetPosition; }
		set { m_targetPosition = value; }
	}

	public Seek(SteeringController steeringController)
	{
		m_steeringController = steeringController;
	}

	public virtual Vector3 GetSteeringVector(Vector3 targetPosition)
	{
		Vector3 velocityToTarget = targetPosition - m_steeringController.transform.position;

		velocityToTarget.y = 0;

		// Accelerate to the target
		velocityToTarget.Normalize();
		velocityToTarget *= m_steeringController.MaxAcceleration;

		return velocityToTarget;
	}

	public virtual Vector3 GetSteeringVector()
	{
		return GetSteeringVector(m_targetPosition);
	}
}
