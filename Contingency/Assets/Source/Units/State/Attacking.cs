
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

		// If the target can no longer be attacked - i.e moves out of range - restart the attack sequence
		if (!entity.UnitController.CanAttack(entity, entity.CurrentTarget))
		{
			entity.UnitController.Attack(entity, entity.CurrentTarget);
		}
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
		else if (m_unit.LineOfSightController.NearbyEnemyBuildings.Count > 0)
		{
			if (m_unit.LineOfSightController.NearbyEnemyBuildings[0] != null)
			{
				m_unit.UnitController.Attack(m_unit, m_unit.LineOfSightController.NearbyEnemyBuildings[0]);
			}
		}
		else
		{
			m_unit.StateMachine.ChangeState(new Idle());
		}
	}
}
