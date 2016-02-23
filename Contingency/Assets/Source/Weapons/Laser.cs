using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(LineRenderer))]
public class Laser : Weapon
{
	private LineRenderer m_laser;
	private const float kPulseTime = 0.4f;

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
		while (target.Health > 0f && target.transform != null)
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
		yield return new WaitForSeconds(kPulseTime);
		m_laser.enabled = false;
	}
}
