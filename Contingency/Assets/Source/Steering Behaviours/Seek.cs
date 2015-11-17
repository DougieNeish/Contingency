using UnityEngine;
using System.Collections;

//[RequireComponent (typeof (SteeringController))]
public class Seek
{	public Seek(SteeringController steeringController)
	{
		m_steeringController = steeringController;
	}

	private SteeringController m_steeringController;

	//void Awake()
	//{
	//	m_steeringController = GetComponent<SteeringController>();
	//}

	//void Start()
	//{
	//}
	
	//void Update()
	//{
	//}

	public Vector3 GetSteeringVector(Vector3 targetPosition)
	{
		Vector3 desiredVelocity = Vector3.Normalize(targetPosition - m_steeringController.Agent.transform.position) * m_steeringController.MaxSpeed;
		return (desiredVelocity - m_steeringController.Rigidbody.velocity);
	}
}
