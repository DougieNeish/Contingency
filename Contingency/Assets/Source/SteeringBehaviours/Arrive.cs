using UnityEngine;

public class Arrive
{
	protected readonly SteeringController m_steeringController;
	private Vector3 m_targetPosition;

	// The radius from the target that means we are close enough and have arrived
	private float m_arriveRadius = 0.5f;

	// The radius from the target where we start to slow down 
	private float m_slowRadius = 1f;

	/* The time in which we want to achieve the targetSpeed */
	private float m_timeToTarget = 0.1f;

	public Vector3 TargetPosition
	{
		get { return TargetPosition; }
		set { m_targetPosition = value; }
	}

	public float ArriveRadius
	{
		get { return m_arriveRadius; }
		set { m_arriveRadius = value; }
	}

	public float SlowRadius
	{
		get { return m_slowRadius; }
		set { m_slowRadius = value; }
	}

	public float TimeToTarget
	{
		get { return m_timeToTarget; }
		set { m_timeToTarget = value; }
	}

	public Arrive(SteeringController steeringController)
	{
		m_steeringController = steeringController;
	}

	public virtual Vector3 GetSteeringVector(Vector3 targetPosition)
	{
		Vector3 velocityToTarget = targetPosition - m_steeringController.transform.position;

		velocityToTarget.y = 0;

		float distance = velocityToTarget.magnitude;

		// If we are within the stopping radius then stop
		if (distance < m_arriveRadius)
		{
			m_steeringController.Rigidbody.velocity = Vector3.zero;
			return Vector3.zero;
		}

		// Calculate the target speed, full speed at slowRadius distance and 0 speed at 0 distance
		float targetSpeed;
		if (distance > m_slowRadius)
		{
			targetSpeed = m_steeringController.MaxVelocity;
		}
		else
		{
			targetSpeed = m_steeringController.MaxVelocity * (distance / m_slowRadius);
		}

		// Give targetVelocity the correct speed
		velocityToTarget.Normalize();
		velocityToTarget *= targetSpeed;

		// Calculate the linear acceleration
		Vector3 acceleration = velocityToTarget - new Vector3(m_steeringController.Rigidbody.velocity.x, 0f, m_steeringController.Rigidbody.velocity.z);

		// Rather than accelerate the character to the correct speed in 1 second, 
		// accelerate so we reach the desired speed in timeToTarget seconds 
		// (if we were to actually accelerate for the full timeToTarget seconds).
		acceleration *= 1 / m_timeToTarget;

		// Make sure we are accelerating at max acceleration
		if (acceleration.magnitude > m_steeringController.MaxAcceleration)
		{
			acceleration.Normalize();
			acceleration *= m_steeringController.MaxAcceleration;
		}

		return acceleration;
	}

	public virtual Vector3 GetSteeringVector()
	{
		return GetSteeringVector(m_targetPosition);
	}
}
