using UnityEngine;
using System.Collections.Generic;

public class Separation
{
	private readonly SteeringController m_steeringController;
	private HashSet<GameObject> m_neighbours;

	public HashSet<GameObject> Neighbours
	{
		get { return m_neighbours; }
		set { m_neighbours = value; }
	}

	public Separation(SteeringController steeringController)
	{
		m_neighbours = new HashSet<GameObject>();
		m_steeringController = steeringController;
	}

	// Create a force that pushes the agent away from all neighbours
	public Vector3 GetSteeringVector()
	{
		Vector3 steeringForce = Vector3.zero;

		foreach (GameObject neighbour in m_neighbours)
		{
			Vector3 toAgent = m_steeringController.transform.position - neighbour.transform.position;
			steeringForce += toAgent.normalized / toAgent.magnitude;
		}

		return steeringForce;
	}
}
