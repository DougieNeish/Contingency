using UnityEngine;

public class MovingToAttack : State<Unit>
{
	public override void Enter(Unit entity)
	{
	}

	public override void Execute(Unit entity)
	{
		// TODO: Stop distance check being called every frame. Move to coroutine?
		if (entity.Stance == Unit.CombatStance.Defensive &&
			Vector3.Distance(entity.transform.position, entity.LastStationaryPosition) > Unit.kDefensiveRange)
		{
			entity.UnitController.MoveToPosition(entity, entity.LastStationaryPosition);
		}
	}

	public override void Exit(Unit entity)
	{
	}
}
