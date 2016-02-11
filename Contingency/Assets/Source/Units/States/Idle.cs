
public class Idle : State<Unit>
{
	private Unit m_unit;

	public override void Enter(Unit entity)
	{
		m_unit = entity;
		m_unit.LineOfSightRenderer.OnEnemyUnitSpotted += HandleEnemySpotted;
	}

	public override void Execute(Unit entity)
	{
		
	}

	public override void Exit(Unit entity)
	{
		entity.LineOfSightRenderer.OnEnemyUnitSpotted -= HandleEnemySpotted;
	}

	private void HandleEnemySpotted(Unit enemy)
	{
		m_unit.UnitController.Attack(m_unit, enemy);
	}
}
