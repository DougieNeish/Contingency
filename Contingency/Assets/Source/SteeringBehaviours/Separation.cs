using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Separation
{
	private readonly SteeringController m_steeringController;
	private List<GameObject> m_neighbours;

	public List<GameObject> Neighbours
	{
		get { return m_neighbours; }
		set { m_neighbours = value; }
	}

	public Separation(SteeringController steeringController)
	{
		m_neighbours = new List<GameObject>();
		m_steeringController = steeringController;
	}

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
