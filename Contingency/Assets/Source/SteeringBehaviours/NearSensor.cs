using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SphereCollider))]
public class NearSensor : MonoBehaviour
{
	private HashSet<GameObject> m_nearbyObstacles = new HashSet<GameObject>();
	private HashSet<GameObject> m_nearbyUnits = new HashSet<GameObject>();

	public HashSet<GameObject> NearbyObstacles
	{
		get { return m_nearbyObstacles; }
	}

	public HashSet<GameObject> NearbyUnits
	{
		get { return m_nearbyUnits; }
	}

	void Awake()
	{
		// Ensure near sensor does not collide with the terrain
		if (GameObject.FindGameObjectWithTag("Terrain") == null)
		{
			Debug.LogWarning("NearSensor requires an object with the tag 'Terrain' to avoid collision with the terrain/floor.");
		}

		Physics.IgnoreCollision(GetComponent<SphereCollider>(), GameObject.FindGameObjectWithTag("Terrain").GetComponent<Collider>());
	}

	void OnTriggerEnter(Collider other)
	{
		m_nearbyObstacles.Add(other.transform.parent.gameObject);

		if (other.tag ==  "Unit")
		{
			m_nearbyUnits.Add(other.transform.parent.gameObject);
		}
	}

	void OnTriggerExit(Collider other)
	{
		m_nearbyObstacles.Remove(other.transform.parent.gameObject);

		if (other.tag == "Unit")
		{
			m_nearbyUnits.Remove(other.transform.parent.gameObject);
		}
	}
}
