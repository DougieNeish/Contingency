
using UnityEngine;

public interface IDamageable
{
	float Health
	{
		get;
		set;
	}

	Transform transform
	{
		get;
	}

	void ReceiveDamage(float damage);
}
