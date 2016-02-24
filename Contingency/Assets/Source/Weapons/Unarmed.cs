using UnityEngine;
using System.Collections;
using System;

public class Unarmed : Weapon
{
	public override IEnumerator Fire(IDamageable target)
	{
		yield return 1;
	}
}
