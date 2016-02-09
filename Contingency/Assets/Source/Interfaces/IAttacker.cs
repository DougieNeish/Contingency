using UnityEngine;
using System.Collections;

public interface IAttacker
{
	IDamageable CurrentTarget
	{
		get;
	}

	float AttackRange
	{
		get;
	}

	void Attack(IDamageable target);
}
