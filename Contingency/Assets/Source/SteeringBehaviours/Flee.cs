using UnityEngine;
using System.Collections;

public class Flee
{
	protected readonly SteeringController m_steeringController;
	private Vector3 m_targetPosition;
	private float m_panicDistance = float.MaxValue;
	private float m_timeToTargetSpeed = 0.1f;
	private bool m_decelerateToStop = true;

	public Vector3 TargetPosition
	{
		get { return m_targetPosition; }
		set { m_targetPosition = value; }
	}

	public float PanicDistance
	{
		get { return m_panicDistance; }
		set { m_panicDistance = value; }
	}

	public float TimeToTarget
	{
		get { return m_timeToTargetSpeed; }
		set { m_timeToTargetSpeed = value; }
	}

	// Set to false to allow combining with other steering behaviours
	public bool DecelerateToStop
	{
		get { return m_decelerateToStop; }
        set { m_decelerateToStop = value; }
	}

	public Flee(SteeringController steeringController)
	{
		m_steeringController = steeringController;
	}

	public virtual Vector3 GetSteeringVector(Vector3 targetPosition)
	{
		Vector3 velocityToTarget = m_steeringController.transform.position - targetPosition;

		// If target is not within panic distance, stop
		float sqrPanicDistance = m_panicDistance * m_panicDistance;
		if (velocityToTarget.sqrMagnitude > sqrPanicDistance)
		{
			if (m_decelerateToStop && m_steeringController.Rigidbody.velocity.magnitude > 0.001f)
			{
				// Decelerate to zero velocity in time to target amount of time
				velocityToTarget = -m_steeringController.Rigidbody.velocity / m_timeToTargetSpeed;

				if (velocityToTarget.magnitude > m_steeringController.MaxAcceleration)
				{
					velocityToTarget = giveMaxAccel(velocityToTarget);
				}

				return velocityToTarget;
			}
			else
			{
				//m_steeringController.m_rb.velocity = Vector3.zero;
				return Vector3.zero;
			}
		}

		return giveMaxAccel(velocityToTarget);
	}

	public virtual Vector3 GetSteeringVector()
	{
		return GetSteeringVector(m_targetPosition);
	}

	private Vector3 giveMaxAccel(Vector3 v)
	{
		//Remove the y coordinate
		v.y = 0;

		v.Normalize();

		//Accelerate to the target
		v *= m_steeringController.MaxAcceleration;

		return v;
	}
}
