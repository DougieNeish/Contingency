using UnityEngine;
using System.Collections;

public class Wander : Seek
{
	private float m_wanderRadius = 2.0f;
	private float m_wanderDistance = 0.5f;
	private float m_wanderJitter = 40.0f;
	private Vector3 m_wanderTarget;

	public float WanderRadius
	{
		get { return m_wanderRadius; }
		set { m_wanderRadius = value; }
	}

	public float WanderDistance
	{
		get { return m_wanderDistance; }
		set { m_wanderDistance = value; }
	}

	public float WanderJitter
	{
		get { return m_wanderJitter; }
		set { m_wanderJitter = value; }
	}

	public Wander(SteeringController steeringController) : base(steeringController)
	{
		float theta = Random.value * 2 * Mathf.PI;

		// Create a vector to a target position on the wander circle
		m_wanderTarget = new Vector3(m_wanderRadius * Mathf.Cos(theta), 0f, m_wanderRadius * Mathf.Sin(theta));
	}

	public override Vector3 GetSteeringVector()
	{
		float jitter = m_wanderJitter * Time.deltaTime;

		// Add a small displacement to the wander target
		m_wanderTarget += new Vector3(Random.Range(-1f, 1f) * jitter, 0f, Random.Range(-1f, 1f) * jitter);

		// Make the wanderTarget fit on the wander circle again
		m_wanderTarget.Normalize();
		m_wanderTarget *= m_wanderRadius;

		// Move the target in front of the agent
		Vector3 targetPosition = m_steeringController.transform.position + m_steeringController.transform.right * m_wanderDistance + m_wanderTarget;

		Debug.DrawLine(m_steeringController.transform.position, targetPosition, Color.red);

		return base.GetSteeringVector(targetPosition);
	}
}
