using UnityEngine;

public interface IAttacker
{
	IDamageable Target { get; set; }

	GameObject gameObject { get; }

	void Attack(IDamageable target);
}
