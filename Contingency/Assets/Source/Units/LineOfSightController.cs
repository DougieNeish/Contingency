using UnityEngine;
using System.Collections.Generic;

public class LineOfSightController : MonoBehaviour
{
	public delegate void EnemyUnitSpottedEventHandler(Unit enemy);
	public event EnemyUnitSpottedEventHandler OnEnemyUnitSpotted;

	public delegate void EnemyBuildingSpottedEventHandler(Building building);
	public event EnemyBuildingSpottedEventHandler OnEnemyBuildingSpotted;

	private Player m_owner;
	private Renderer m_renderer;

	private int m_visibleToUnitsCount;
	private List<Unit> m_nearbyEnemies;
	private List<Building> m_nearbyEnemyBuildings;

	public List<Unit> NearbyEnemies
	{
		get { return m_nearbyEnemies; }
	}

	public List<Building> NearbyEnemyBuildings
	{
		get { return m_nearbyEnemyBuildings; }
	}

	void Awake()
	{
		m_renderer = gameObject.transform.parent.GetComponent<Renderer>();

		m_visibleToUnitsCount = 0;
		m_nearbyEnemies = new List<Unit>();
		m_nearbyEnemyBuildings = new List<Building>();
	}

	void Start()
	{
		m_owner = gameObject.GetComponentInParent<Unit>().Owner;
		Physics.IgnoreCollision(GetComponent<SphereCollider>(), GameObject.FindGameObjectWithTag("Terrain").GetComponent<Collider>());
	}

	void Update()
	{
		if (m_owner.Type == Player.PlayerType.AI)
		{
			m_renderer.enabled = m_visibleToUnitsCount == 0 ? false : true;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Unit")
		{
			// Only counts as 'seen' if the other unit belongs to a human player
			Unit unit = other.GetComponent<Unit>();
			if (unit.Owner.Type == Player.PlayerType.Human)
			{
				m_visibleToUnitsCount++;
			}

			// If the unit doesn't belong to me, an enemy has been spotted
			if (unit.Owner.ID != m_owner.ID)
			{
				if (!m_nearbyEnemies.Contains(unit))
				{
					m_nearbyEnemies.Add(unit);
					unit.OnUnitKilled += HandleUnitKilled;
				}

				if (OnEnemyUnitSpotted != null)
				{
					OnEnemyUnitSpotted(unit);
				}
			}
		}
		else if (other.tag == "Static/AIBuilding" || other.tag == "Static/HumanBuilding")
		{
			Building building = other.GetComponent<Building>();

			// If the building doesn't belong to me, an enemy building has been spotted
			if (building.Owner.ID != m_owner.ID)
			{
				if (!m_nearbyEnemyBuildings.Contains(building))
				{
					m_nearbyEnemyBuildings.Add(building);
					building.OnBuildingDestroyed += HandleBuildingDestroyed;
				}

				if (OnEnemyBuildingSpotted != null)
				{
					OnEnemyBuildingSpotted(building);
				}
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Unit")
		{
			Unit unit = other.GetComponent<Unit>();
			if (unit.Owner.Type == Player.PlayerType.Human)
			{
				m_visibleToUnitsCount--;
			}

			// Handle enemies
			if (unit.Owner.ID != m_owner.ID)
			{
				if (!m_nearbyEnemies.Contains(unit))
				{
					m_nearbyEnemies.Remove(unit);
					unit.OnUnitKilled -= HandleUnitKilled;
				}
			}
		}
		else if (other.tag == "Static/AIBuilding")
		{
			Building building = other.GetComponent<Building>();

			if (!m_nearbyEnemyBuildings.Contains(building))
			{
				m_nearbyEnemyBuildings.Remove(building);
				building.OnBuildingDestroyed -= HandleBuildingDestroyed;
			}
		}
	}

	private void HandleUnitKilled(GameObject unit)
	{
		Unit enemy = unit.GetComponent<Unit>();
		m_nearbyEnemies.Remove(enemy);
		enemy.OnUnitKilled -= HandleUnitKilled;
	}

	private void HandleBuildingDestroyed(Building building)
	{
		m_nearbyEnemyBuildings.Remove(building);
		building.OnBuildingDestroyed -= HandleBuildingDestroyed;
	}

	// TODO: FINISH ME
	//private bool HasDirectLineOfSight(GameObject unit, GameObject target)
	//{
	//	Vector3 direction = target.transform.position - unit.transform.position;
	//	RaycastHit hit;
	//	//Debug.DrawRay(unit.transform.position, direction, Color.cyan, 5f);
	//	if (Physics.Raycast(unit.transform.position, direction, out hit, unit.AttackRange))
	//	{
	//		return hit.transform == target.transform;
	//	}

	//	return false;
	//}
}
