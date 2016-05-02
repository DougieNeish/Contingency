using System;
using System.Collections;

public class Unarmed : Weapon
{
	public override IEnumerator Fire(IDamageable target)
	{
		yield return 1;
	}

	public override void Stop()
	{
	}
}
