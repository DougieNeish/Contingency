using UnityEngine;
using System.Collections;

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
	[SerializeField] private Weapon m_weapon;

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

	public Weapon Weapon
	{
		get { return m_weapon; }
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
	#endregion

	void Awake()
	{
		m_id = UnitController.NextUnitID;
		m_steeringController = GetComponent<SteeringController>();
		m_renderer = GetComponent<Renderer>();

		m_health = 100f;
		m_currentTarget = null;

		m_weapon = Instantiate(m_weapon, transform.position, Quaternion.identity) as Weapon;
		m_weapon.transform.SetParent(gameObject.transform);
	}

	void Start()
	{
		gameObject.GetComponentInChildWithTag<SphereCollider>("SightRadius").radius = m_sightRadius;
	}

	public void Stop()
	{
		m_steeringController.Stop();
		StopAllCoroutines();
		m_weapon.StopAllCoroutines();
		m_currentTarget = null;
	}

	public virtual void ReceiveDamage(float damage)
	{
		m_health -= damage;
		Debug.Log(m_health);

		if (m_health <= 0f)
		{
			StartCoroutine(DeathActions());
		}
	}

	public void Attack(IDamageable target)
	{
		if (target != m_currentTarget)
		{
			Stop();
			m_currentTarget = target;
			transform.LookAt(target.transform);

			StartCoroutine(m_weapon.Fire(m_currentTarget));
		}
	}

	private IEnumerator DeathActions()
	{
		Destroy(gameObject);

		if (OnUnitKilled != null)
		{
			OnUnitKilled(gameObject);
		}

		yield return null;
	}
}
