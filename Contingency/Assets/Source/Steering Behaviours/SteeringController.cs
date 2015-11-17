using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Rigidbody))]
public class SteeringController : MonoBehaviour
{
	public GameObject Agent
	{
		get { return gameObject; }
	}

	public Rigidbody Rigidbody
	{
		get { return m_rigidBody; }
	}

	public float MaxSpeed
	{
		get { return m_maxSpeed; }
	}

	public float MaxForce
	{
		get { return m_maxForce; }
	}

	public Vector3 TargetPosition
	{
		get { return m_targetPosition; }
		set { m_targetPosition = value; }
	}

	public float FleeActivationDistance
	{
		get { return m_fleeActivationDistance; }
		set { m_fleeActivationDistance = value; }
	}

	public enum Deceleration
	{
		Slow = 3,
		Normal = 2,
		Fast = 1,
	}

	[SerializeField] private float m_maxSpeed;
	[SerializeField] private float m_maxForce;

	private Vector3 m_steeringForce;
	private Rigidbody m_rigidBody;

	private Vector3 m_targetPosition;
	private float m_fleeActivationDistance;

	// Steering behaviours
	private Seek m_seek;
	private Flee m_flee;
	private Arrive m_arrive;

	[Flags] public enum BehaviourType
	{
		None = 0x00000,
		Seek = 0x00002,
		Flee = 0x00004,
		Arrive = 0x00008,
	}

	private BehaviourType m_activeBehaviours;

	void Awake()
	{
		m_rigidBody = gameObject.GetComponent<Rigidbody>();
		m_targetPosition = Vector3.zero;
		m_fleeActivationDistance = 0.0f;

		m_seek = new Seek(this);
		m_flee = new Flee(this);
		m_arrive = new Arrive(this);
	}

	void Start()
	{
	}
	
	void Update()
	{
		m_rigidBody.velocity += CalculateSteeringForce() * Time.deltaTime;
	}

	public Vector3 CalculateSteeringForce()
	{
		m_steeringForce = Vector3.zero;
		Vector3 newForce = Vector3.zero;

		if (IsBehaviourOn(BehaviourType.Seek))
		{
			newForce = m_seek.GetSteeringVector(m_targetPosition);
			if (!AccumulateForce(ref m_steeringForce, newForce))
			{
				return m_steeringForce;
			}
		}

		if (IsBehaviourOn(BehaviourType.Flee))
		{
			newForce = m_flee.GetSteeringVector(m_targetPosition, m_fleeActivationDistance);
			if (!AccumulateForce(ref m_steeringForce, newForce))
			{
				return m_steeringForce;
			}
		}

		if (IsBehaviourOn(BehaviourType.Arrive))
		{
			newForce = m_arrive.GetSteeringVector(m_targetPosition, Deceleration.Fast);
			if (!AccumulateForce(ref m_steeringForce, newForce))
			{
				return m_steeringForce;
			}
		}

		return m_steeringForce;
	}

	public void TurnOnBehaviour(BehaviourType behaviour)
	{
		m_activeBehaviours |= behaviour;
	}

	public void TurnOffBehaviour(BehaviourType behaviour)
	{
		if (IsBehaviourOn(behaviour))
		{
			m_activeBehaviours ^= behaviour;
		}
	}

	private bool AccumulateForce(ref Vector3 currentForce, Vector3 forceToAdd)
	{
		float remainingMagnitude = m_maxForce - currentForce.magnitude;
		if (remainingMagnitude <= 0.0f)
		{
			return false;
		}

		// If there's enough remaining magnitude add full force, otherwise add as much as possible
		if (forceToAdd.magnitude < remainingMagnitude)
		{
			currentForce += forceToAdd;
		}
		else
		{
			currentForce += (Vector3.Normalize(forceToAdd) * remainingMagnitude);
		}

		return true;
	}

	private bool IsBehaviourOn(BehaviourType behaviour)
	{
		return (m_activeBehaviours & behaviour) == behaviour;
	}
}
