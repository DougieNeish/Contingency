using UnityEngine;
using System.Collections;

public abstract class Weapon : MonoBehaviour
{
	[SerializeField] protected float m_damage;
	[SerializeField] protected float m_range;
	[SerializeField] protected float m_fireRate;
	protected Unit m_owner;

	public float Damage
	{
		get { return m_damage; }
	}

	public float Range
	{
		get { return m_range; }
	}

	public float FireRate
	{
		get { return m_fireRate; }
	}

	public abstract IEnumerator Fire(IDamageable target);
}
