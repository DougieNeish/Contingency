using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(LineRenderer))]
public class Laser : Weapon
{
	private LineRenderer m_laser;

	void Awake()
	{
		m_laser = GetComponent<LineRenderer>();
	}
	
	void Start()
	{
		m_laser.enabled = false;
		m_laser.SetWidth(0.1f, 0.3f);
		m_laser.SetVertexCount(2);
		m_laser.useWorldSpace = true;
	}

	public override IEnumerator Fire(IDamageable target)
	{
		while (target.Health > 0)
		{
			m_laser.SetPosition(0, transform.position);
			m_laser.SetPosition(1, target.transform.position);

			StartCoroutine(Pulse());
			target.ReceiveDamage(Damage);

			yield return new WaitForSeconds(FireRate);
		}
	}

	private IEnumerator Pulse()
	{
		m_laser.enabled = true;
		yield return new WaitForSeconds(0.4f);
		m_laser.enabled = false;
	}
}
