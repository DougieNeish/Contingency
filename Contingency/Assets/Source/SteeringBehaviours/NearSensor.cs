using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SphereCollider))]
public class NearSensor : MonoBehaviour
{
	private HashSet<GameObject> m_nearbyObstacles;
	private HashSet<GameObject> m_nearbyUnits;

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
		m_nearbyObstacles = new HashSet<GameObject>();
		m_nearbyUnits = new HashSet<GameObject>();

		// Ensure near sensor does not collide with the terrain
		if (GameObject.FindGameObjectWithTag("Terrain") == null)
		{
			Debug.LogWarning("NearSensor requires an object with the tag 'Terrain' to avoid collision with the terrain/floor.");
		}

		Physics.IgnoreCollision(GetComponent<SphereCollider>(), GameObject.FindGameObjectWithTag("Terrain").GetComponent<Collider>());
	}

	void OnTriggerEnter(Collider collider)
	{
		m_nearbyObstacles.Add(collider.transform.parent.gameObject);

		if (collider.transform.parent.tag == "Unit")
		{
			m_nearbyUnits.Add(collider.transform.parent.gameObject);
		}
	}

	void OnTriggerExit(Collider collider)
	{
		m_nearbyObstacles.Remove(collider.transform.parent.gameObject);

		if (collider.transform.parent.tag == "Unit")
		{
			m_nearbyUnits.Remove(collider.transform.parent.gameObject);
		}
	}
}
