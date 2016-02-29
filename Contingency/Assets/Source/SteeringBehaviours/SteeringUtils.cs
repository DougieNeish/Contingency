using UnityEngine;
using System.Collections.Generic;

public static class SteeringUtils
{
	public const float kWaypointLoopActivationDistance = 3f;

	public static void SetArriveTarget(this SteeringController steeringController, Vector3 target)
	{
		steeringController.Arrive.TargetPosition = target;
		steeringController.TurnOnBehaviour(SteeringController.BehaviourType.Arrive);
	}

	public static void AddWaypoint(this SteeringController steeringController, Vector3 waypoint, bool newPath = false)
	{
		Path path = steeringController.PathFollowing.Path;
		
		if (newPath)
		{
			path.ClearWaypoints();
		}
		//else
		//{
		//	if (path.Waypoints.Count > 1)
		//	{
		//		if (Vector3.Distance(waypoint, path.Waypoints[path.Waypoints.Count - 1]) < kWaypointLoopActivationDistance)
		//		{
		//			path.Loop = true;
		//		}
		//	}
		//}

		if (path.Waypoints.Count > 1)
		{
			if (Vector3.Distance(waypoint, path.Waypoints[0]) < kWaypointLoopActivationDistance)
			{
				path.Loop = true;
			}
		}

		path.AddWaypoint(waypoint);
		steeringController.TurnOnBehaviour(SteeringController.BehaviourType.PathFollowing);
	}

	public static void AddWaypoints(this SteeringController steeringController, Vector3[] waypoints, bool loop, bool newPath = false)
	{
		if (waypoints != null)
		{
			for (int i = 0; i < waypoints.Length; i++)
			{
				steeringController.AddWaypoint(waypoints[i], (newPath && i == 0));
			}
		}
	}

	public static void Stop(this SteeringController steeringController)
	{
		steeringController.TurnOffBehaviour(SteeringController.BehaviourType.PathFollowing);
		steeringController.Rigidbody.velocity = Vector3.zero;
	}
}
