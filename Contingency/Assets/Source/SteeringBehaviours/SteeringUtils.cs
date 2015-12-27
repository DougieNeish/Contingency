using UnityEngine;
using System.Collections.Generic;

public static class SteeringUtils
{
	public const float kWaypointLoopActivationDistance = 2f;

	public static void SetArriveTarget(this SteeringController steeringController, Vector3 target)
	{
		steeringController.Arrive.TargetPosition = target;
		steeringController.TurnOnBehaviour(SteeringController.BehaviourType.Arrive);
	}

	public static void AddWaypoint(this SteeringController steeringController, Vector3 waypoint, bool end, bool newPath = false)
	{
		if (newPath)
		{
			steeringController.PathFollowing.Path.ClearWaypoints();
		}

		if (steeringController.PathFollowing.Path.Waypoints.Count > 1)
		{
			if (Vector3.Distance(waypoint, steeringController.PathFollowing.Path.Waypoints[0]) < kWaypointLoopActivationDistance)
			{
				steeringController.PathFollowing.Path.Loop = true;
			}
		}

		GameObject waypointMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		waypointMarker.transform.position = waypoint;
		waypointMarker.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		Object.Destroy(waypointMarker.GetComponent<Collider>());

		if (end)
		{
			waypointMarker.transform.localScale = new Vector3(1f, 1f, 1f);
		}

		steeringController.PathFollowing.Path.AddWaypoint(waypoint);
		steeringController.TurnOnBehaviour(SteeringController.BehaviourType.PathFollowing);
	}

	public static void AddWaypoints(this SteeringController steeringController, Vector3[] waypoints)
	{
		for (int i = 0; i < waypoints.Length; i++)
		{
			steeringController.AddWaypoint(waypoints[i], (i == waypoints.Length - 1), (i == 1));
		}
	}

	public static void SetFlocking(this SteeringController steeringController, List<GameObject> neighbours)
	{
		steeringController.SetNeighbouringUnits(neighbours);
		steeringController.TurnOnBehaviour(SteeringController.BehaviourType.Flocking);
	}

	// TODO: Need to update ObstacleAvoidance.Obstacles with list of NEARBY obstacles, not just all of them
	public static void TurnOnObstacleAvoidance(this SteeringController steeringController, List<GameObject> obstacles)
	{
		steeringController.UpdateObstacleList(obstacles);
		steeringController.TurnOnBehaviour(SteeringController.BehaviourType.ObstacleAvoidance);
	}

	public static void UpdateObstacleList(this SteeringController steeringController, List<GameObject> obstacles)
	{
		steeringController.ObstacleAvoidance.Obstacles = obstacles;
	}
}
