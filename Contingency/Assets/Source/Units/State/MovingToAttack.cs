using UnityEngine;

public class MovingToAttack : State<Unit>
{
	private Unit m_unit;

	public override void Enter(Unit entity)
	{
		m_unit = entity;
		entity.SteeringController.PathFollowing.OnPathCompleted += HandlePathCompleted;
		//entity.Weapon.StopAllCoroutines();
		entity.Weapon.Stop();
	}

	public override void Execute(Unit entity)
	{
		// TODO: Stop distance check being called every frame. Move to coroutine?
		if (entity.Stance == Unit.CombatStance.Defensive &&
			Vector3.Distance(entity.transform.position, entity.LastStationaryPosition) > Unit.kDefensiveRange)
		{
			entity.UnitController.MoveToPosition(entity, entity.LastStationaryPosition);
		}

		// If the target gets killed whilst moving towards it, go idle
		if (entity.Target != null && entity.Target.Health <= 0f)
		{
			entity.StateMachine.ChangeState(new Idle());
		}
	}

	public override void Exit(Unit entity)
	{
		entity.SteeringController.PathFollowing.OnPathCompleted -= HandlePathCompleted;
	}

	private void HandlePathCompleted()
	{
		// If unit reaches end of path and can't attack, go idle (unit has lost target)
		if (m_unit.Target == null || m_unit.Target.Health <= 0f || !m_unit.UnitController.CanAttack(m_unit, m_unit.Target))
		{
			m_unit.StateMachine.ChangeState(new Idle());
		}
	}
}
