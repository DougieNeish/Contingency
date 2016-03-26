
public class Global : State<Unit>
{
	private Unit m_unit;

	public override void Enter(Unit entity)
	{
		m_unit = entity;
		entity.OnDamageReceived += HandleDamageReceived;
	}

	public override void Execute(Unit entity)
	{
	}

	public override void Exit(Unit entity)
	{
		entity.OnDamageReceived -= HandleDamageReceived;
	}

	private void HandleDamageReceived(float remainingHealth, IAttacker attacker)
	{
		if (remainingHealth <= 50f && UnityEngine.Random.Range(0, 5) == 0)
		{
			m_unit.UnitController.Flee(m_unit, attacker.gameObject.transform.position);
		}
	}
}
