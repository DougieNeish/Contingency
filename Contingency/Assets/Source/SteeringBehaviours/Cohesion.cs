using UnityEngine;
using System.Collections.Generic;

public class Cohesion : Seek
{
	private HashSet<GameObject> m_neighbours;

	public HashSet<GameObject> Neighbours
	{
		get { return m_neighbours; }
		set { m_neighbours = value; }
	}

	public Cohesion(SteeringController steeringController) : base(steeringController)
	{
		m_neighbours = new HashSet<GameObject>();
	}

	// Create a force that pushes the agent towards the average position of its neighbours
	public override Vector3 GetSteeringVector()
	{
		Vector3 averagePosition = Vector3.zero;

		foreach (GameObject neighbour in m_neighbours)
		{
			averagePosition += neighbour.transform.position;
		}

		if (m_neighbours.Count > 0)
		{
			averagePosition /= m_neighbours.Count;
			return base.GetSteeringVector(averagePosition);
		}

		return averagePosition;
	}
}
