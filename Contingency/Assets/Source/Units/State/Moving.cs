
public class Moving : State<Unit>
{
	private Unit m_unit;

	public override void Enter(Unit entity)
	{
		m_unit = entity;
		entity.SteeringController.PathFollowing.OnPathCompleted += HandlePathCompleted;
		//entity.LineOfSightRenderer.OnEnemyUnitSpotted += HandleEnemySpotted;
	}

	public override void Execute(Unit entity)
	{
		// if (stance == aggressive && taking damage) attack
	}

	public override void Exit(Unit entity)
	{
		entity.SteeringController.PathFollowing.OnPathCompleted -= HandlePathCompleted;
		//entity.LineOfSightRenderer.OnEnemyUnitSpotted -= HandleEnemySpotted;
	}

	private void HandlePathCompleted()
	{
		m_unit.StateMachine.ChangeState(new Idle());
	}

	//private void HandleEnemySpotted(Unit enemy)
	//{
	//	m_unit.UnitController.Attack(m_unit, enemy);
	//}
}
