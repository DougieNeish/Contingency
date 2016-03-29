using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class SteeringController : MonoBehaviour
{
	[Flags] public enum BehaviourType
	{
		None				= 0,
		Seek				= 1 << 0,
		Flee				= 1 << 1,
		Arrive				= 1 << 2,
		Pursuit				= 1 << 3,
		Evade				= 1 << 4,
		Wander				= 1 << 5,
		Separation			= 1 << 6,
		Alignment			= 1 << 7,
		Cohesion			= 1 << 8,
		ObstacleAvoidance	= 1 << 9,
		PathFollowing		= 1 << 10,
		OffsetPursuit		= 1 << 11,
		Flocking			= Separation | Alignment | Cohesion,
	}

	private BehaviourType m_activeBehaviours;
	private Seek m_seek;
	private Flee m_flee;
	private Arrive m_arrive;
	private Pursuit m_pursuit;
	private Evade m_evade;
	private Wander m_wander;
	private Separation m_separation;
	private Alignment m_alignment;
	private Cohesion m_cohesion;
	private ObstacleAvoidance m_obstacleAvoidance;
	private PathFollowing m_pathFollowing;
	private OffsetPursuit m_offsetPursuit;

	[SerializeField] private float m_maxVelocity = 10f;
	[SerializeField] private float m_maxAcceleration = 10f;

	private Rigidbody m_rigidbody;
	private NearSensor m_nearSensor;

#region Properties
	public Seek Seek
	{
		get
		{
			if (m_seek == null)
			{
				m_seek = new Seek(this);
			}
			return m_seek;
		}
	}

	public Flee Flee
	{
		get
		{
			if (m_flee == null)
			{
				m_flee = new Flee(this);
			}
			return m_flee;
		}
	}

	public Arrive Arrive
	{
		get
		{
			if (m_arrive == null)
			{
				m_arrive = new Arrive(this);
			}
			return m_arrive;
        }
	}

	public Pursuit Pursuit
	{
		get
		{
			if (m_pursuit == null)
			{
				m_pursuit = new Pursuit(this);
			}
			return m_pursuit;
		}
	}

	public Evade Evade
	{
		get
		{
			if (m_evade == null)
			{
				m_evade = new Evade(this);
			}
			return m_evade;
		}
	}

	public Wander Wander
	{
		get
		{
			if (m_wander == null)
			{
				m_wander = new Wander(this);
			}
			return m_wander;
		}
	}

	public Separation Separation
	{
		get
		{
			if (m_separation == null)
			{
				m_separation = new Separation(this);
			}
			return m_separation;
		}
	}

	public Alignment Alignment
	{
		get
		{
			if (m_alignment == null)
			{
				m_alignment = new Alignment(this);
			}
			return m_alignment;
		}
	}

	public Cohesion Cohesion
	{
		get
		{
			if (m_cohesion == null)
			{
				m_cohesion = new Cohesion(this);
			}
			return m_cohesion;
		}
	}

	public ObstacleAvoidance ObstacleAvoidance
	{
		get
		{
			if (m_obstacleAvoidance == null)
			{
				m_obstacleAvoidance = new ObstacleAvoidance(this);
			}
			return m_obstacleAvoidance;
		}
	}

	public PathFollowing PathFollowing
	{
		get
		{
			if (m_pathFollowing == null)
			{
				m_pathFollowing = new PathFollowing(this);
			}
			return m_pathFollowing;
		}
	}

	public OffsetPursuit OffsetPursuit
	{
		get
		{
			if (m_offsetPursuit == null)
			{
				m_offsetPursuit = new OffsetPursuit(this);
			}
			return m_offsetPursuit;
		}
	}

	public float MaxVelocity
	{
		get { return m_maxVelocity; }
		set { m_maxVelocity = value; }
	}

	public float MaxAcceleration
	{
		get { return m_maxAcceleration; }
		set { m_maxAcceleration = value; }
	}

	public Rigidbody Rigidbody
	{
		get { return m_rigidbody; }
		set { m_rigidbody = value; }
	}

	public float Radius
	{
		get
		{
			if (GetComponent<SphereCollider>() == null)
			{
				Debug.LogWarning("Trying to get the radius of an agent but no SphereCollider was found.");
			}

			return GetComponent<SphereCollider>().radius;
		}
	}
#endregion

	void Awake()
	{
		m_rigidbody = GetComponent<Rigidbody>();
		m_nearSensor = GetComponentInChildren<NearSensor>();
	}

	void Update()
	{
		if ((IsBehaviourOn(BehaviourType.ObstacleAvoidance) || 
			IsBehaviourOn(BehaviourType.Alignment) ||
			IsBehaviourOn(BehaviourType.Cohesion) ||
			IsBehaviourOn(BehaviourType.Separation)) &&
			m_nearSensor == null)
		{
			Debug.LogWarning("Trying to use a steering behaviour that requires a NearSensor but one has not been found.");
		}

		if (IsBehaviourOn(BehaviourType.ObstacleAvoidance))
		{
			ObstacleAvoidance.Obstacles = m_nearSensor.NearbyObstacles;
		}

		if (IsBehaviourOn(BehaviourType.Alignment))
		{
			Alignment.Neighbours = m_nearSensor.NearbyUnits;
		}

		if (IsBehaviourOn(BehaviourType.Cohesion))
		{
			Cohesion.Neighbours = m_nearSensor.NearbyUnits;
		}

		if (IsBehaviourOn(BehaviourType.Separation))
		{
			Separation.Neighbours = m_nearSensor.NearbyUnits;
		}
	}

	void FixedUpdate()
	{
		Steer(CalculateSteeringForce());
	}

	void OnDrawGizmos()
	{
		//foreach (Vector3 point in PathFollowing.Path.Waypoints)
		//for (int i = 0; i < PathFollowing.Path.Waypoints.Count; i++)
		//{
		//	Gizmos.DrawIcon(PathFollowing.Path.Waypoints[i], "Waypoint " + i);
		//	Gizmos.DrawLine(PathFollowing.Path.Waypoints[i], PathFollowing.Path.Waypoints[i++]);
		//}

		//if (PathFollowing.Path.Waypoints.Count > 0)
		//{
		//	Gizmos.DrawWireSphere(PathFollowing.Path.Waypoints[0], SteeringUtils.kWaypointLoopActivationDistance);
        //}
	}

	public void Steer(Vector3 steeringForce)
	{
		m_rigidbody.velocity += steeringForce * Time.deltaTime;

		if (m_rigidbody.velocity.sqrMagnitude > m_maxVelocity * m_maxVelocity)
		{
			m_rigidbody.velocity = m_rigidbody.velocity.normalized * m_maxVelocity;
		}

		// Rotate to direction
		if (m_rigidbody.velocity != Vector3.zero)
		{
			transform.rotation = Quaternion.LookRotation(m_rigidbody.velocity);
		}
	}

	public Vector3 CalculateSteeringForce()
	{
		Vector3 accumulatedForce = Vector3.zero;
		Vector3 newForce = Vector3.zero;

		if (IsBehaviourOn(BehaviourType.ObstacleAvoidance))
		{
			newForce = ObstacleAvoidance.GetSteeringVector();
			if (!AccumulateForce(ref accumulatedForce, newForce))
			{
				return accumulatedForce;
			}
		}

		if (IsBehaviourOn(BehaviourType.Seek))
		{
			newForce = Seek.GetSteeringVector();
			if (!AccumulateForce(ref accumulatedForce, newForce))
			{
				return accumulatedForce;
			}
		}

		if (IsBehaviourOn(BehaviourType.Flee))
		{
			newForce = Flee.GetSteeringVector();
			if (!AccumulateForce(ref accumulatedForce, newForce))
			{
				return accumulatedForce;
			}
		}

		if (IsBehaviourOn(BehaviourType.Arrive))
		{
			newForce = Arrive.GetSteeringVector();
			if (!AccumulateForce(ref accumulatedForce, newForce))
			{
				return accumulatedForce;
			}
		}

		if (IsBehaviourOn(BehaviourType.Pursuit))
		{
			newForce = Pursuit.GetSteeringVector();
			if (!AccumulateForce(ref accumulatedForce, newForce))
			{
				return accumulatedForce;
			}
		}

		if (IsBehaviourOn(BehaviourType.Evade))
		{
			newForce = Evade.GetSteeringVector();
			if (!AccumulateForce(ref accumulatedForce, newForce))
			{
				return accumulatedForce;
			}
		}

		if (IsBehaviourOn(BehaviourType.Separation))
		{
			newForce = Separation.GetSteeringVector();
			if (!AccumulateForce(ref accumulatedForce, newForce))
			{
				return accumulatedForce;
			}
		}

		if (IsBehaviourOn(BehaviourType.Alignment))
		{
			newForce = Alignment.GetSteeringVector();
			if (!AccumulateForce(ref accumulatedForce, newForce))
			{
				return accumulatedForce;
			}
		}

		if (IsBehaviourOn(BehaviourType.Cohesion))
		{
			newForce = Cohesion.GetSteeringVector();
			if (!AccumulateForce(ref accumulatedForce, newForce))
			{
				return accumulatedForce;
			}
		}

		if (IsBehaviourOn(BehaviourType.Wander))
		{
			newForce = m_wander.GetSteeringVector();
			if (!AccumulateForce(ref accumulatedForce, newForce))
			{
				return accumulatedForce;
			}
		}

		if (IsBehaviourOn(BehaviourType.PathFollowing))
		{
			newForce = m_pathFollowing.GetSteeringVector();
			if (!AccumulateForce(ref accumulatedForce, newForce))
			{
				return accumulatedForce;
			}
		}

		if (IsBehaviourOn(BehaviourType.OffsetPursuit))
		{
			newForce = m_offsetPursuit.GetSteeringVector();
			if (!AccumulateForce(ref accumulatedForce, newForce))
			{
				return accumulatedForce;
			}
		}

		return accumulatedForce;
	}

	public void TurnOnBehaviour(BehaviourType behaviour)
	{
		m_activeBehaviours |= behaviour;

		switch (behaviour)
		{
			case BehaviourType.Seek:
				{
					if (m_seek == null)
					{
						m_seek = new Seek(this);
					}
					break;
				}
			case BehaviourType.Flee:
				{
					if (m_flee == null)
					{
						m_flee = new Flee(this);
					}
					break;
				}
			case BehaviourType.Arrive:
				{
					if (m_arrive == null)
					{
						m_arrive = new Arrive(this);
					}
					break;
				}
			case BehaviourType.Pursuit:
				{
					if (m_pursuit == null)
					{
						m_pursuit = new Pursuit(this);
					}
					break;
				}
			case BehaviourType.Evade:
				{
					if (m_evade == null)
					{
                        m_evade = new Evade(this);
					}
					break;
				}
			case BehaviourType.Wander:
				{
					if (m_wander == null)
					{
						m_wander = new Wander(this);
					}
					break;
				}
			case BehaviourType.Separation:
				{
					if (m_separation == null)
					{
						m_separation = new Separation(this);
					}
					break;
				}
			case BehaviourType.Alignment:
				{
					if (m_alignment == null)
					{
						m_alignment = new Alignment(this);
					}
					break;
				}
			case BehaviourType.ObstacleAvoidance:
				{
					if (m_obstacleAvoidance == null)
					{
						m_obstacleAvoidance = new ObstacleAvoidance(this);
					}
					break;
				}
			case BehaviourType.PathFollowing:
				{
					if (m_pathFollowing == null)
					{
						m_pathFollowing = new PathFollowing(this);
					}
					break;
				}
			case BehaviourType.OffsetPursuit:
				{
					if (m_offsetPursuit == null)
					{
						m_offsetPursuit = new OffsetPursuit(this);
					}
					break;
				}
		}
	}

	public void TurnOffBehaviour(BehaviourType behaviour)
	{
		if (IsBehaviourOn(behaviour))
		{
			m_activeBehaviours ^= behaviour;
		}
	}

	public bool IsBehaviourOn(BehaviourType behaviour)
	{
		return (m_activeBehaviours & behaviour) == behaviour;
	}

	private bool AccumulateForce(ref Vector3 currentForce, Vector3 forceToAdd)
	{
		float remainingMagnitude = m_maxVelocity - currentForce.magnitude;
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
}
