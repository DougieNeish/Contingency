using UnityEngine;
using System.Collections;

public interface IAttacker
{
	IDamageable CurrentTarget {	get; }
	float AttackRange {	get; }
	float AttackDamage { get; }
	float AttackSpeed { get; }

	void Attack(IDamageable target);
}
