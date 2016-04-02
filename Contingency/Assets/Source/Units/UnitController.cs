using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitController : MonoBehaviour
{
	public enum UnitType
	{
		Laser,
		Sniper,
		Scout,
		Count,
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
	private GameObject[] m_offensiveMarkers;
	private GameObject[] m_defensiveMarkers;
	private GameObject[] m_scoutMarkers;
	private GameObject m_AISpawn;

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

		m_offensiveMarkers = GameObject.FindGameObjectsWithTag("MoveMarker/Offensive");
		m_defensiveMarkers = GameObject.FindGameObjectsWithTag("MoveMarker/Defensive");
		m_scoutMarkers = GameObject.FindGameObjectsWithTag("MoveMarker/Scout");
		m_AISpawn = GameObject.FindGameObjectWithTag("AISpawn");

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
	}

	void Update()
	{
		m_ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		//Debug.DrawRay(m_ray.origin, m_ray.direction * InputManager.kRaycastLength, Color.cyan);
	}

	public void MoveToPosition(Unit unit, Vector3 targetPosition, bool setMovingState = true, bool isManualCommand = false)
	{
		if (setMovingState)
		{
			unit.StateMachine.ChangeState(new Moving());
		}

		SteeringController steeringController = unit.SteeringController;
		steeringController.MaxVelocity = Unit.kMaxVelocity;
		steeringController.TurnOffBehaviour(SteeringController.BehaviourType.Flee);

		Vector3[] waypoints;
		if (isManualCommand && Input.GetKey(KeyCode.LeftShift))
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
	}

	public void Attack(Unit unit, IDamageable target, bool isManualCommand = false)
	{
		// If the unit has the 'Unarmed' weapon, do nothing when told to attack
		if (unit.Weapon.GetType() == typeof(Unarmed))
		{
			return;
		}

		unit.StateMachine.ChangeState(new MovingToAttack());
		StartCoroutine(MoveToAttack(unit, target, isManualCommand));
	}

	public bool CanAttack(Unit unit, IDamageable target)
	{
		if (unit == null || target.transform == null)
		{
			return false;
		}

		return IsInAttackRange(unit, target) && HasDirectLineOfSight(unit, target);
	}

	public void Flee(Unit unit, Vector3 target)
	{
		unit.StateMachine.ChangeState(new Fleeing());
		unit.SteeringController.Flee.TargetPosition = target;
		unit.SteeringController.Flee.PanicDistance = 80f;
		unit.SteeringController.Flee.DecelerateToStop = false;
		unit.SteeringController.MaxVelocity = Unit.kFleeVelocity;
		unit.SteeringController.TurnOnBehaviour(SteeringController.BehaviourType.Flee);
	}

	public void SpawnUnitOnMouse(UnitType unitType)
	{
		RaycastHit hitObject;
		if (Physics.Raycast(m_ray, out hitObject, InputManager.kRaycastLength))
		{
			Vector3 position = new Vector3(hitObject.point.x, hitObject.point.y + 0.7f, hitObject.point.z);
			GameObject newUnit = Instantiate(GetUnitPrefab(unitType), position, Quaternion.identity) as GameObject;
			Unit unit = newUnit.GetComponent<Unit>();
			unit.Owner = m_player;
			unit.UnitController = this;
			unit.StateMachine.ChangeState(new Idle());
			unit.OnUnitKilled += HandleUnitDeath;

			m_units.Add(newUnit);

			if (OnUnitSpawned != null)
			{
				OnUnitSpawned(newUnit);
			}
		}
	}

	public void SpawnAutoUnit(UnitType unitType)
	{
		Unit.CombatStance stance = Unit.CombatStance.Aggressive;
		GameObject moveMarker = null;

		switch (unitType)
		{
			case UnitType.Laser:
				stance = Unit.CombatStance.Aggressive;
				moveMarker = m_offensiveMarkers[Random.Range(0, m_offensiveMarkers.Length)];
				break;
			case UnitType.Sniper:
				stance = Unit.CombatStance.Defensive;
				moveMarker = m_defensiveMarkers[Random.Range(0, m_defensiveMarkers.Length)];
				break;
			case UnitType.Scout:
				stance = Unit.CombatStance.Static;
				moveMarker = m_scoutMarkers[Random.Range(0, m_scoutMarkers.Length)];
				break;
		}

		GameObject newUnit = Instantiate(GetUnitPrefab(unitType), m_AISpawn.transform.position, Quaternion.identity) as GameObject;
		Unit unit = newUnit.GetComponent<Unit>();
		unit.Owner = m_player;
		unit.UnitController = this;
		unit.Stance = stance;
		unit.OnUnitKilled += HandleUnitDeath;

		m_units.Add(newUnit);

		if (OnUnitSpawned != null)
		{
			OnUnitSpawned(newUnit);
		}

		MoveToPosition(unit, moveMarker.transform.position);
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
							Attack(unit.GetComponent<Unit>(), target.GetComponent<IDamageable>(), true);
						}
					}
					else // move to the right-clicked location
					{
						foreach (GameObject unit in m_selectedUnits)
						{
							MoveToPosition(unit.GetComponent<Unit>(), hitInfo.point, true, true);
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

	private IEnumerator MoveToAttack(Unit unit, IDamageable target, bool isManualCommand)
	{
		if (unit.Stance == Unit.CombatStance.Static)
		{
			if (CanAttack(unit, target))
			{
				unit.Attack(target);
			}
			else
			{
				unit.StateMachine.ChangeState(new Idle());
				StopAllCoroutines();
				yield return 1;
			}
		}

		// If target is a building use its locator as the target position
		Transform child = target.transform.GetChild(0);
		Vector3 targetPosition = child.tag == "Locator" ? child.position : target.transform.position;

		MoveToPosition(unit, targetPosition, false, isManualCommand);
		yield return new WaitUntil(() => CanAttack(unit, target));
		unit.Attack(target);
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
}
