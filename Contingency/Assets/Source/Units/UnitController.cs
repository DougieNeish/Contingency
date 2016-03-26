﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitController : MonoBehaviour
{
	public enum UnitType
	{
		Laser,
		Sniper,
		Scout,
	}

	[System.Serializable]
	public class UnitLookup
	{
		public UnitType type;
		public GameObject prefab;
	}

	public delegate void UnitSpawnedEventHandler(GameObject unit);
	public event UnitSpawnedEventHandler OnUnitSpawned;

	public delegate void SelectedUnitsEventHandler(List<GameObject> selectedUnits);
	public event SelectedUnitsEventHandler OnSelectedUnitsUpdated;

	private static int m_nextUnitID;

	[SerializeField] private UnitLookup[] m_unitLookup;
	private List<GameObject> m_units;
	private List<GameObject> m_selectedUnits;

	private GameObject m_gameManager;
	private Player m_player;
	private SelectionManager m_selectionManager;
	private InputManager m_inputManager;
	private PathfindingController m_pathfindingController;

	private Ray m_ray;

	#region Properties
	public static int NextUnitID
	{
		get { return m_nextUnitID++; }
	}

	public List<GameObject> Units
	{
		get { return m_units; }
	}

	public List<GameObject> SelectedUnits
	{
		get { return m_selectedUnits; }
	}
	#endregion

	void Awake()
	{
		// TODO: This could be an issue if a player is added mid-game and m_nextUnitID is reset to 0
		m_nextUnitID = 0;

		m_units = new List<GameObject>();
		m_selectedUnits = new List<GameObject>();

		m_gameManager = GameObject.FindGameObjectWithTag("GameManager");
		m_player = GetComponent<Player>();
		m_pathfindingController = m_gameManager.GetComponent<PathfindingController>();
		m_ray = GetComponent<InputManager>().Ray;

		if (m_player.Type == Player.PlayerType.Human)
		{
			m_selectionManager = GetComponent<SelectionManager>();
			m_inputManager = GetComponent<InputManager>();
		}
	}

	void OnEnable()
	{
		if (m_player.Type == Player.PlayerType.Human)
		{
			m_selectionManager.OnNoObjectSelected += DeselectUnits;
			m_selectionManager.OnUnitSelected += UpdateSelectedUnitList;
			m_inputManager.OnMouseEvent += MouseInput;
		}
	}

	void OnDisable()
	{
		if (m_player.Type == Player.PlayerType.Human)
		{
			m_selectionManager.OnNoObjectSelected -= DeselectUnits;
			m_selectionManager.OnUnitSelected -= UpdateSelectedUnitList;
			m_inputManager.OnMouseEvent -= MouseInput;
		}

		//foreach (GameObject unit in m_units)
		//{
		//	unit.GetComponent<Unit>().OnUnitKilled -= HandleUnitDeath;
		//}
	}

	void Update()
	{
		m_ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		//Debug.DrawRay(m_ray.origin, m_ray.direction * InputManager.kRaycastLength, Color.cyan);

		//if (Input.GetKeyDown(KeyCode.Space))
		//{
		//	CreateUnitOnMouse();
		//}
	}

	public void MoveToPosition(Unit unit, Vector3 targetPosition, bool setMovingState = true)
	{
		// Don't override MovingToAttack state
		//if (!unit.StateMachine.IsInState(new MovingToAttack()))
		if (setMovingState)
		{
			unit.StateMachine.ChangeState(new Moving());
		}

		SteeringController steeringController = unit.SteeringController;
		steeringController.TurnOffBehaviour(SteeringController.BehaviourType.Flocking);

		Vector3[] waypoints;
		if (Input.GetKey(KeyCode.LeftShift))
		{
			waypoints = m_pathfindingController.Search(steeringController.PathFollowing.Path.EndPosition, targetPosition);
			steeringController.AddWaypoints(waypoints, false, false);
		}
		else
		{
			unit.Stop();

			waypoints = m_pathfindingController.Search(unit.transform.position, targetPosition);
			steeringController.AddWaypoints(waypoints, false, true);
		}

		//if (m_selectedUnits.Count > 1)
		//{
		//	//Vector3[] formationPositions = Formations.CalculateSquareFormation(m_selectedUnits.Count, 2f, 2f);

		//	// Start from 1 as unit 0 is the leader
		//	for (int i = 1; i < m_selectedUnits.Count; i++)
		//	{
		//		// TODO: Change this. Temporary A* search for all selected units
		//		Vector3[] waypoints = m_pathfindingController.Search(m_selectedUnits[i].transform.position, targetPosition);
		//		m_selectedUnits[i].GetComponent<SteeringController>().AddWaypoints(waypoints, false);


		//		//steeringController = m_selectedUnits[i].GetComponent<SteeringController>();
		//		//steeringController.TurnOnBehaviour(SteeringController.BehaviourType.Flocking);

		//		// TODO: Do I need to add all selected units as neighbours?

		//		//steeringController.OffsetPursuit.Leader = m_selectedUnits[0].GetComponent<Rigidbody>();
		//		//steeringController.OffsetPursuit.Offset = formationPositions[i];
		//		//steeringController.TurnOnBehaviour(SteeringController.BehaviourType.OffsetPursuit);
		//	}
		//}
	}

	public void Attack(Unit unit, IDamageable target)
	{
		// If the unit has the 'Unarmed' weapon, do nothing when told to attack
		if (unit.Weapon.GetType() == typeof(Unarmed))
		{
			return;
		}

		unit.StateMachine.ChangeState(new MovingToAttack());
		StartCoroutine(MoveToAttack(unit, target));
	}

	public void SetSelectedUnitsStance(Unit.CombatStance stance)
	{
		foreach (GameObject unit in m_selectedUnits)
		{
			unit.GetComponent<Unit>().Stance = stance;
			Debug.Log("Stance: " + stance);
		}
	}

	private void MouseInput(InputManager.MouseEventType eventType, RaycastHit hitInfo)
	{
		switch (eventType)
		{
			case InputManager.MouseEventType.OnRightMouseDown:
				{
					// If no units are selected, break
					if (m_selectedUnits.Count < 1)
					{
						break;
					}

					// If a unit or building that has been right-clicked doesn't belong to the player, attack it
					GameObject target = hitInfo.transform.gameObject;

					if (target.tag == "Unit" && target.GetComponent<Unit>().Owner.ID != m_player.ID ||
						target.tag == "Static/AIBuilding")
					{
						foreach (GameObject unit in m_selectedUnits)
						{
							Attack(unit.GetComponent<Unit>(), target.GetComponent<IDamageable>());
						}
					}
					else // move to the right-clicked location
					{
						foreach (GameObject unit in m_selectedUnits)
						{
							MoveToPosition(unit.GetComponent<Unit>(), hitInfo.point);
						}
					}
					break;
				}
		}
	}

	private void UpdateSelectedUnitList(List<GameObject> selectedObjects, bool modifyCurrentSelection)
	{
		if (selectedObjects == null)
		{
			m_selectedUnits.Clear();
		}
		else
		{
			if (modifyCurrentSelection)
			{
				foreach (GameObject unit in selectedObjects)
				{
					if (m_selectedUnits.Contains(unit))
					{
						m_selectedUnits.Remove(unit);
					}
					else
					{
						m_selectedUnits.Add(unit);
					}
                }
			}
			else
			{
				m_selectedUnits.Clear();

				foreach (GameObject unit in selectedObjects)
				{
					if (!m_selectedUnits.Contains(unit))
					{
						m_selectedUnits.Add(unit);
					}
				}
			}
		}

		if (OnSelectedUnitsUpdated != null)
		{
			OnSelectedUnitsUpdated(m_selectedUnits);
		}
	}

	private void DeselectUnits()
	{
		m_selectedUnits.Clear();
		if (OnSelectedUnitsUpdated != null)
		{
			OnSelectedUnitsUpdated(m_selectedUnits);
		}
	}

	private IEnumerator MoveToAttack(Unit unit, IDamageable target)
	{
		// TODO: FIX THIS, it's broken
		if (unit.Stance == Unit.CombatStance.Static)
		{
			if (CanAttack(unit, target))
			{
				unit.Attack(target);
			}
			else
			{
				StopCoroutine("MoveToAttack");
				unit.StateMachine.ChangeState(new Idle());
				yield return 1;
			}
		}

		Vector3 targetPosition = target.transform.position;

		// If target is a building
		if (target.transform.GetChild(0).tag == "Locator")
		{
			targetPosition = target.transform.GetChild(0).position;
		}

		MoveToPosition(unit, targetPosition, false);
		yield return new WaitUntil(() => CanAttack(unit, target));
		unit.Attack(target);
	}

	public bool CanAttack(Unit unit, IDamageable target)
	{
		if (unit == null || target.transform == null)
		{
			return false;
		}

		return IsInAttackRange(unit, target) && HasDirectLineOfSight(unit, target);
	}

	private bool IsInAttackRange(Unit unit, IDamageable target)
	{
		Vector3 velocityToTarget = target.transform.position - unit.transform.position;
		velocityToTarget.y = 0;
		float sqrDistance = velocityToTarget.sqrMagnitude;

		return sqrDistance < unit.Weapon.Range * unit.Weapon.Range;
	}

	private bool HasDirectLineOfSight(Unit unit, IDamageable target)
	{	
		Vector3 direction = target.transform.position - unit.transform.position;
		RaycastHit hit;
		Debug.DrawRay(unit.transform.position, direction, Color.cyan, 5f);
		if (Physics.Raycast(unit.transform.position, direction, out hit, unit.Weapon.Range))
		{
			return hit.transform == target.transform;
		}

		return false;
	}

	private void HandleUnitDeath(GameObject unit)
	{
		m_units.Remove(unit);
		m_selectedUnits.Remove(unit);
	}

	private GameObject GetUnitPrefab(UnitType unitType)
	{
		for (int i = 0; i < m_unitLookup.Length; i++)
		{
			if (m_unitLookup[i].type == unitType)
			{
				return m_unitLookup[i].prefab;
			}
		}
		return null;
	}

	public void CreateUnitOnMouse(UnitType unitType)
	{
		RaycastHit hitObject;
		if (Physics.Raycast(m_ray, out hitObject, InputManager.kRaycastLength))
		{
			Vector3 position = new Vector3(hitObject.point.x, hitObject.point.y + 0.7f, hitObject.point.z);
			GameObject newUnit = Instantiate(GetUnitPrefab(unitType), position, Quaternion.identity) as GameObject;

			Unit unit = newUnit.GetComponent<Unit>();
			// TODO: Move to init params?
			unit.Owner = m_player;
			unit.UnitController = this;
			unit.OnUnitKilled += HandleUnitDeath;

			m_units.Add(newUnit);

			if (OnUnitSpawned != null)
			{
				OnUnitSpawned(newUnit);
			}

			//newUnit.GetComponent<SteeringController>().TurnOnBehaviour(SteeringController.BehaviourType.ObstacleAvoidance);
			Physics.IgnoreCollision(newUnit.GetComponent<SphereCollider>(), GameObject.FindGameObjectWithTag("Terrain").GetComponent<Collider>());
		}
	}
}
