using UnityEngine;
using System.Collections;

public class Evade : Flee
{
	private Rigidbody m_target;
	private float m_maxPrediction = 1.0f;

	public Rigidbody Target
	{
		get { return m_target; }
		set { m_target = value; }
	}

	public float MaxPrediction
	{
		get { return m_maxPrediction; }
		set { m_maxPrediction = value; }
	}

	public Evade(SteeringController steeringController) : base(steeringController) { }

	public override Vector3 GetSteeringVector()
	{
		Vector3 velocityToTarget = m_target.position - m_steeringController.transform.position;
		float distance = velocityToTarget.magnitude;

		float targetSpeed = m_target.velocity.magnitude;

		// Calculate the prediction time 
		float prediction;
		if (targetSpeed <= distance / m_maxPrediction)
		{
			prediction = m_maxPrediction;
		}
		else
		{
			prediction = distance / targetSpeed;
			// Move the predicted position a little before the target reaches the agent
			prediction *= 0.9f;
		}

		// Put the target together based on where we think the target will be
		Vector3 predictedTarget = m_target.position + m_target.velocity * prediction;

		return base.GetSteeringVector(predictedTarget);
	}
}
