using UnityEngine;
using System.Collections;

public abstract class Weapon : MonoBehaviour
{
	[SerializeField] private float m_damage;
	[SerializeField] private float m_range;
	[SerializeField] private float m_fireRate;

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
