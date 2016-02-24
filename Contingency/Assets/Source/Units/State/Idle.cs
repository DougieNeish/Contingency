﻿
public class Idle : State<Unit>
{
	private Unit m_unit;

	public override void Enter(Unit entity)
	{
		m_unit = entity;
		entity.LineOfSightRenderer.OnEnemyUnitSpotted += HandleEnemySpotted;
		entity.OnDamageReceived += HandleDamageReceived;

		entity.Stop();
	}

	public override void Execute(Unit entity)
	{
		// if take damage, return fire
	}

	public override void Exit(Unit entity)
	{
		entity.LineOfSightRenderer.OnEnemyUnitSpotted -= HandleEnemySpotted;
		entity.OnDamageReceived -= HandleDamageReceived;
	}

	private void HandleEnemySpotted(Unit enemy)
	{
		m_unit.UnitController.Attack(m_unit, enemy);
	}

	private void HandleDamageReceived(float remainingHealth, IAttacker attacker)
	{
		m_unit.UnitController.Attack(m_unit, attacker.gameObject.GetComponent<Unit>());
	}
}
