using UnityEngine;

public interface IAttacker
{
	IDamageable CurrentTarget {	get; }

	GameObject gameObject { get; }

	void Attack(IDamageable target);
}
