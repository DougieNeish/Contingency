using UnityEngine;

public class Building : MonoBehaviour, IDamageable
{
	public delegate void BuildingDestroyedEventHandler(Building building);
	public event BuildingDestroyedEventHandler OnBuildingDestroyed;

	private Player m_owner;
	private float m_health;

	public float Health
	{
		get { return m_health; }
		set { m_health = value; }
	}

	public Player Owner
	{
		get { return m_owner; }
		set { m_owner = value; }
	}

	void Awake()
	{
		m_health = 100f;
	}

	public void ReceiveDamage(float damage, IAttacker attacker)
	{
		m_health -= damage;

		if (m_health <= 0f)
		{
			DeathActions();
		}
	}

	private void DeathActions()
	{
		Destroy(gameObject);
	}
}
