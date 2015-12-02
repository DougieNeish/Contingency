using UnityEngine;
using System.Collections.Generic;

public class Cohesion : Seek
{
	private List<GameObject> m_neighbours;

	public List<GameObject> Neighbours
	{
		get { return m_neighbours; }
		set { m_neighbours = value; }
	}

	public Cohesion(SteeringController steeringController) : base(steeringController)
	{
		m_neighbours = new List<GameObject>();
	}

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
