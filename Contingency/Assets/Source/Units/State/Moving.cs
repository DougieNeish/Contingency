
public class Moving : State<Unit>
{
	private Unit m_unit;

	public override void Enter(Unit entity)
	{
		m_unit = entity;
		entity.SteeringController.PathFollowing.OnPathCompleted += HandlePathCompleted;
		entity.OnDamageReceived += HandleOnDamageReceived;
	}

	public override void Execute(Unit entity)
	{
	}

	public override void Exit(Unit entity)
	{
		entity.SteeringController.PathFollowing.OnPathCompleted -= HandlePathCompleted;
		entity.OnDamageReceived -= HandleOnDamageReceived;
	}

	private void HandlePathCompleted()
	{
		m_unit.StateMachine.ChangeState(new Idle());
	}

	private void HandleOnDamageReceived(float remainingHealth, IAttacker attacker)
	{
		if (m_unit.Stance == Unit.CombatStance.Aggressive)
		{
			m_unit.UnitController.Attack(m_unit, attacker.gameObject.GetComponent<Unit>());
		}
	}
}
