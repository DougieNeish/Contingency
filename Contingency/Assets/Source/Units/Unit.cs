using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour, IDamageable, IAttacker
{
	public delegate void UnitKilledEventHandler(GameObject unit);
	public event UnitKilledEventHandler OnUnitKilled;

	public delegate void DamageReceivedEventHandler(float remainingHealth, IAttacker attacker);
	public event DamageReceivedEventHandler OnDamageReceived;

	public enum CombatStance
	{
		Aggressive,
		Defensive,
		Static,
	}

	private int m_id;
	private Player m_owner;
	private Renderer m_renderer;
	private UnitController m_unitController;
	private SteeringController m_steeringController;
	private LineOfSightController m_lineOfSightController;

	private float m_health;
	private IDamageable m_currentTarget;
	private CombatStance m_stance;
	[SerializeField] private float m_sightRadius;
	[SerializeField] private Weapon m_weapon;

	private StateMachine<Unit> m_stateMachine;
	private Vector3 m_lastStationaryPosition;

	public const float kDefensiveRange = 12f;
	public const float kMaxVelocity = 6f;
	public const float kFleeVelocity = 9f;

	#region Unit Properties
	public int ID
	{
		get { return m_id; }
	}

	public Player Owner
	{
		get { return m_owner; }
		set { m_owner = value; }
	}

	public UnitController UnitController
	{
		get { return m_unitController; }
		set { m_unitController = value; }
	}

	public SteeringController SteeringController
	{
		get { return m_steeringController; }
	}

	public LineOfSightController LineOfSightController
	{
		get { return m_lineOfSightController; }
	}

	public CombatStance Stance
	{
		get { return m_stance; }
		set { m_stance = value; }
	}

	public float SightRadius
	{
		get { return m_sightRadius; }
	}

	public Weapon Weapon
	{
		get { return m_weapon; }
	}

	public StateMachine<Unit> StateMachine
	{
		get { return m_stateMachine; }
	}

	public Vector3 LastStationaryPosition
	{
		get { return m_lastStationaryPosition; }
		set { m_lastStationaryPosition = value; }
	}
	#endregion

	#region IDamageable Properties
	public float Health
	{
		get { return m_health; }
		set { m_health = value; }
	}

	Transform IDamageable.transform
	{
		get { return this == null ? null : transform; }
	}
	#endregion

	#region IAttacker Properties
	public IDamageable CurrentTarget
	{
		get { return m_currentTarget; }
	}

	GameObject IAttacker.gameObject
	{
		get { return gameObject; }
	}
	#endregion

	void Awake()
	{
		m_id = UnitController.NextUnitID;
		m_steeringController = GetComponent<SteeringController>();
		m_renderer = GetComponent<Renderer>();
		m_lineOfSightController = gameObject.GetComponentInChildWithTag<LineOfSightController>("SightRadius");

		m_health = 100f;
		m_currentTarget = null;
		m_stance = CombatStance.Aggressive;
		m_steeringController.MaxVelocity = kMaxVelocity;

		m_weapon = Instantiate(m_weapon, transform.position, Quaternion.identity) as Weapon;
		m_weapon.transform.SetParent(gameObject.transform);

		m_stateMachine = new StateMachine<Unit>(this);
		m_stateMachine.SetGlobalState(new Global());
		m_lastStationaryPosition = Vector3.zero;
	}

	void Start()
	{
		gameObject.GetComponentInChildWithTag<SphereCollider>("SightRadius").radius = m_sightRadius;

		m_stateMachine.ChangeState(new Idle());
	}

	void Update()
	{
		m_stateMachine.Update();
	}

	void OnDrawGizmos()
	{
		if (m_steeringController.PathFollowing.Path.Waypoints.Count > 1)
		{
			Gizmos.DrawWireSphere(m_steeringController.PathFollowing.Path.Waypoints[0], SteeringUtils.kWaypointLoopActivationDistance);
		}
	}

	public void Stop()
	{
		m_steeringController.Stop();
		StopAllCoroutines();
		m_weapon.StopAllCoroutines();
		m_currentTarget = null;
	}

	public void ReceiveDamage(float damage, IAttacker attacker)
	{
		m_health -= damage;
		//Debug.Log(m_health);

		if (OnDamageReceived != null)
		{
			OnDamageReceived(m_health, attacker);
		}

		if (m_health <= 0f)
		{
			Death();
		}
	}

	public void Attack(IDamageable target)
	{
		if (target != m_currentTarget)
		{
			m_stateMachine.ChangeState(new Attacking());

			Stop();
			m_currentTarget = target;
			transform.LookAt(target.transform);

			StartCoroutine(m_weapon.Fire(m_currentTarget));
		}
	}

	private void Death()
	{
		Destroy(gameObject);

		if (OnUnitKilled != null)
		{
			OnUnitKilled(gameObject);
		}
	}
}
