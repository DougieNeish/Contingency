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
		Vector3 offsetWorldPosition = m_leader.transform.TransformPoint(m_offset);
		Vector3 vectorToOffset = offsetWorldPosition - m_steeringController.transform.position;

		float lookAheadTime = vectorToOffset.magnitude / m_steeringController.MaxVelocity + m_leader.velocity.magnitude;
		Vector3 targetPosition = vectorToOffset + m_leader.velocity * lookAheadTime;

		return base.GetSteeringVector(targetPosition);
	}
}
