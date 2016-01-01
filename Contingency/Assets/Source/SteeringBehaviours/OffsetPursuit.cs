using UnityEngine;

public class OffsetPursuit : Arrive
{
	private Rigidbody m_leader;
	private Vector3 m_offset;

	public Rigidbody Leader
	{
		get { return m_leader; }
		set { m_leader = value; }
	}

	public Vector3 Offset
	{
		get { return m_offset; }
		set { m_offset = value; }
	}

	public OffsetPursuit(SteeringController steeringController) : base(steeringController)
	{
	}

	public override Vector3 GetSteeringVector()
	{
		//m_offset.y = m_leader.transform.position.y;
		//Vector3 offsetWorldPosition = m_leader.transform.TransformPoint(m_offset);
		//Vector3 vectorToOffset = offsetWorldPosition - m_steeringController.transform.position;

		//float lookAheadTime = vectorToOffset.magnitude / m_steeringController.MaxVelocity + m_leader.velocity.magnitude;
		//Vector3 targetPosition = offsetWorldPosition + m_leader.velocity * lookAheadTime;

		//Debug.DrawLine(m_steeringController.transform.position, targetPosition, Color.red);

		//return base.GetSteeringVector(targetPosition);

		

		Vector3 worldOffsetPos = m_leader.position + m_leader.transform.TransformDirection(m_offset);

		//Debug.DrawLine(transform.position, worldOffsetPos);

		/* Calculate the distance to the offset point */
		Vector3 displacement = worldOffsetPos - m_steeringController.transform.position;
		float distance = displacement.magnitude;

		/* Get the character's speed */
		float speed = m_steeringController.Rigidbody.velocity.magnitude;

		/* Calculate the prediction time */
		float maxPrediction = 1f;
		float prediction;
		if (speed <= distance / maxPrediction)
		{
			prediction = maxPrediction;
		}
		else
		{
			prediction = distance / speed;
		}

		/* Put the target together based on where we think the target will be */
		Vector3 targetPos = worldOffsetPos + m_leader.velocity * prediction;

		return base.GetSteeringVector(targetPos);
	}
}
