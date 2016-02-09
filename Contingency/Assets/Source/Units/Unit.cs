using UnityEngine;
using System.Collections;
using System;

public class Unit : MonoBehaviour, IDamageable, IAttacker
{
	private int m_id;
	[SerializeField] private Player m_owner;
	private Renderer m_renderer;
	private SteeringController m_steeringController;

	private float m_health;
	[SerializeField] private float m_sightRadius;
	[SerializeField] private float m_attackRange;
	private IDamageable m_currentTarget;

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
		if (m_currentTarget != null)
		{
			Vector3 direction = m_currentTarget.transform.position - transform.position;
			Debug.DrawRay(transform.position, direction, Color.cyan);
		}
	}

	public void Stop()
	{
		m_steeringController.Stop();
		StopAllCoroutines();
		m_currentTarget = null;
	}

	//public virtual void Attack(GameObject target)
	//{
	//	Debug.Log("Pew pew pew");

	//	if (target != m_currentTarget)
	//	{
	//		Stop();
	//		m_currentTarget = target;
	//		StartCoroutine(AttackActions());
	//	}
	//}

	public virtual void TakeDamage(float damage)
	{
		Debug.LogError("Unit.TakeDamage() is not implemented");
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
		// TODO: Unit attack action... laser line renderer?
		yield return null;
	}
}
