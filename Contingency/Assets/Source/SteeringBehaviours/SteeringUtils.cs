using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class SteeringUtils
{
	public const float kEnableWaypointLoopDistance = 2f;

	public static void SetArriveTarget(SteeringController steeringController, Vector3 target)
	{
		steeringController.Arrive.TargetPosition = target;
		steeringController.TurnOnBehaviour(SteeringController.BehaviourType.Arrive);
	}

	public static void AddWaypoint(SteeringController steeringController, Vector3 waypoint, bool newPath = false)
	{
		if (newPath)
		{
			steeringController.PathFollowing.Path.ClearWaypoints();
		}

		if (steeringController.PathFollowing.Path.Waypoints.Count > 1)
		{
			if (Vector3.Distance(waypoint, steeringController.PathFollowing.Path.Waypoints[0]) < kEnableWaypointLoopDistance)
			{
				steeringController.PathFollowing.Path.Loop = true;
			}
		}

		GameObject waypointMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		waypointMarker.transform.position = waypoint;
		waypointMarker.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		Object.Destroy(waypointMarker.GetComponent<Collider>());

		steeringController.PathFollowing.Path.AddWaypoint(waypoint);
		steeringController.TurnOnBehaviour(SteeringController.BehaviourType.PathFollowing);
	}

	public static void SetFlocking(SteeringController steeringController, List<GameObject> neighbours)
	{
		steeringController.SetNeighbouringUnits(neighbours);
		steeringController.TurnOnBehaviour(SteeringController.BehaviourType.Flocking);
	}

	// TODO: Need to update ObstacleAvoidance.Obstacles with list of NEARBY obstacles, not just all of them
	public static void TurnOnObstacleAvoidance(SteeringController steeringController, List<GameObject> obstacles)
	{
		UpdateObstacleList(steeringController, obstacles);
		steeringController.TurnOnBehaviour(SteeringController.BehaviourType.ObstacleAvoidance);
	}

	public static void UpdateObstacleList(SteeringController steeringController, List<GameObject> obstacles)
	{
		steeringController.ObstacleAvoidance.Obstacles = obstacles;
	}
}
