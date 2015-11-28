using UnityEngine;
using System.Collections;

public class Pursuit : Seek
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

	public Pursuit(SteeringController steeringController) : base(steeringController) { }

	public override Vector3 GetSteeringVector()
	{
		Vector3 velocityToTarget = m_target.position - m_steeringController.transform.position;
		float distance = velocityToTarget.magnitude;

		float agentSpeed = m_steeringController.Rigidbody.velocity.magnitude;

		// Calculate the prediction time
		float prediction;
		if (agentSpeed <= distance / m_maxPrediction)
		{
			prediction = m_maxPrediction;
		}
		else
		{
			prediction = distance / agentSpeed;
		}

		// Put the target together based on where we think the target will be
		Vector3 predictedTarget = m_target.position + m_target.velocity * prediction;
		return base.GetSteeringVector(predictedTarget);
    }
}
