
public class AttackMove : State<Unit>
{
	private Unit m_unit;

	public override void Enter(Unit entity)
	{
		m_unit = entity;
		entity.LineOfSightController.OnEnemyUnitSpotted += HandleEnemySpotted;
	}

	public override void Execute(Unit entity)
	{
		// if take damage, return fire ??
	}

	public override void Exit(Unit entity)
	{
		entity.LineOfSightController.OnEnemyUnitSpotted -= HandleEnemySpotted;
	}

	private void HandleEnemySpotted(Unit enemy)
	{
		m_unit.UnitController.Attack(m_unit, enemy);
	}
}
