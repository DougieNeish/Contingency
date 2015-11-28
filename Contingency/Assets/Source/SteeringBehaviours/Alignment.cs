using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Alignment
{
	private readonly SteeringController m_steeringController;
	private List<GameObject> m_neighbours;

	public List<GameObject> Neighbours
	{
		get { return m_neighbours; }
		set { m_neighbours = value; }
	}

	public Alignment(SteeringController steeringController)
	{
		m_neighbours = new List<GameObject>();
		m_steeringController = steeringController;
	}

	public Vector3 GetSteeringVector()
	{
		Vector3 averageHeading = Vector3.zero;

		foreach (GameObject neighbour in m_neighbours)
		{
			Rigidbody rb = neighbour.GetComponent<Rigidbody>();
			averageHeading += rb.velocity;
		}

		if (m_neighbours.Count > 0)
		{
			averageHeading /= m_neighbours.Count;
			averageHeading -= m_steeringController.Rigidbody.velocity;
		}

		return averageHeading;
	}
}
