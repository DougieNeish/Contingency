
public class Attacking : State<Unit>
{
	private Unit m_unit;

	public override void Enter(Unit entity)
	{
		m_unit = entity;
		entity.Weapon.OnTargetKilled += OnTargetKilled;
	}

	public override void Execute(Unit entity)
	{
		// TODO: What happens if someone else kills the target?

		//if (entity.CurrentTarget == null)
		//{
		//	if (entity.LineOfSightRenderer.NearbyEnemies.Count > 0)
		//	{
		//		entity.UnitController.Attack(entity, entity.LineOfSightRenderer.NearbyEnemies[0]);
		//	}
		//	else
		//	{
		//		entity.StateMachine.ChangeState(new Idle());
		//	}
		//}
	}

	public override void Exit(Unit entity)
	{
		entity.Weapon.OnTargetKilled -= OnTargetKilled;	
	}

	private void OnTargetKilled()
	{
		if (m_unit.LineOfSightController.NearbyEnemies.Count > 0)
		{
			m_unit.UnitController.Attack(m_unit, m_unit.LineOfSightController.NearbyEnemies[0]);
		}
		else
		{
			m_unit.StateMachine.ChangeState(new Idle());
		}
	}
}
