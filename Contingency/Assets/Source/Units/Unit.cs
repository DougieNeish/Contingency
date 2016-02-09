using UnityEngine;
using System.Collections;
using System;

public class Unit : MonoBehaviour, IDamageable, IAttacker
{
	public delegate void UnitKilledEventHandler(GameObject unit);
	public event UnitKilledEventHandler OnUnitKilled;

	private int m_id;
	private Player m_owner;
	private Renderer m_renderer;
	private SteeringController m_steeringController;

	private float m_health;
	private IDamageable m_currentTarget;
	[SerializeField] private float m_sightRadius;
	[SerializeField] private float m_attackRange;
	[SerializeField] private float m_attackDamage;
	[SerializeField] private float m_attackSpeed;

	public int ID
	{
		get { return m_id; }
	}

	public Player Owner
	{
		get { return m_owner; }
		set { m_owner = value; }
	}

	public SteeringController SteeringController
	{
		get { return m_steeringController; }
	}

	public float SightRadius
	{
		get { return m_sightRadius; }
	}

	#region IDamageable Properties
	public float Health
	{
		get { return m_health; }
		set { m_health = value; }
	}

	Transform IDamageable.transform
	{
		get { return transform; }
	}
	#endregion

	#region IAttacker Properties
	public IDamageable CurrentTarget
	{
		get { return m_currentTarget; }
	}

	public float AttackRange
	{
		get { return m_attackRange; }
	}

	public float AttackDamage
	{
		get { return m_attackDamage; }
	}

	public float AttackSpeed
	{
		get { return m_attackSpeed; }
	}
	#endregion

	void Awake()
	{
		m_id = UnitController.NextUnitID;
		m_steeringController = GetComponent<SteeringController>();
		m_renderer = GetComponent<Renderer>();

		m_health = 100f;
		m_currentTarget = null;
	}

	void Start()
	{
		gameObject.GetComponentInChildWithTag<SphereCollider>("SightRadius").radius = m_sightRadius;
	}

	void Update()
	{
		if (m_health <= 0f)
		{
			StartCoroutine(DeathActions());
		}
	}

	public void Stop()
	{
		m_steeringController.Stop();
		StopAllCoroutines();
		m_currentTarget = null;
	}

	public virtual void TakeDamage(float damage)
	{
		m_health -= damage;
	}

	public virtual void Attack(IDamageable target)
	{
		Debug.Log("Pew pew pew");

		if (target != m_currentTarget)
		{
			Stop();
			m_currentTarget = target;
			transform.LookAt(target.transform);

			StartCoroutine(AttackActions());
		}
	}

	protected virtual IEnumerator AttackActions()
	{
		while (m_currentTarget.Health > 0)
		{
			m_currentTarget.TakeDamage(m_attackDamage);
			yield return new WaitForSeconds(m_attackSpeed);
		}
	}

	protected virtual IEnumerator DeathActions()
	{
		Destroy(gameObject);

		if (OnUnitKilled != null)
		{
			OnUnitKilled(gameObject);
		}

		yield return null;
	}
}
