
public class Idle : State<Unit>
{
	private Unit m_unit;

	public override void Enter(Unit entity)
	{
		m_unit = entity;
		entity.LineOfSightController.OnEnemyUnitSpotted += HandleEnemySpotted;
		entity.LineOfSightController.OnEnemyBuildingSpotted += HandleEnemyBuildingSpotted;
		entity.OnDamageReceived += HandleDamageReceived;

		entity.Stop();
		entity.LastStationaryPosition = entity.transform.position;
	}

	public override void Execute(Unit entity)
	{
	}

	public override void Exit(Unit entity)
	{
		entity.LineOfSightController.OnEnemyUnitSpotted -= HandleEnemySpotted;
		entity.LineOfSightController.OnEnemyBuildingSpotted -= HandleEnemyBuildingSpotted;
		entity.OnDamageReceived -= HandleDamageReceived;
	}

	private void HandleEnemySpotted(Unit enemy)
	{
		if (m_unit.Stance != Unit.CombatStance.Static)
		{
			m_unit.UnitController.Attack(m_unit, enemy);
		}
	}

	private void HandleEnemyBuildingSpotted(Building building)
	{
		if (m_unit.Stance != Unit.CombatStance.Static)
		{
			m_unit.UnitController.Attack(m_unit, building);
		}
	}

	private void HandleDamageReceived(float remainingHealth, IAttacker attacker)
	{
		m_unit.UnitController.Attack(m_unit, attacker.gameObject.GetComponent<Unit>());
	}
}
