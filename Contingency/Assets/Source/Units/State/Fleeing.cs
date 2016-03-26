
public class Fleeing : State<Unit>
{
	private Unit m_unit;

	public override void Enter(Unit entity)
	{
		m_unit = entity;
		entity.SteeringController.Flee.OnTargetEvaded += HandleTargetEvaded;
	}

	public override void Execute(Unit entity)
	{
	}

	public override void Exit(Unit entity)
	{
		entity.SteeringController.Flee.OnTargetEvaded -= HandleTargetEvaded;
	}

	private void HandleTargetEvaded()
	{
		m_unit.SteeringController.TurnOffBehaviour(SteeringController.BehaviourType.Flee);
		m_unit.StateMachine.ChangeState(new Idle());
	}
}
