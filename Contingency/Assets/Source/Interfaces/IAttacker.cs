using UnityEngine;

public interface IAttacker
{
	IDamageable Target { get; }

	GameObject gameObject { get; }

	void Attack(IDamageable target);
}
