using UnityEngine;
using System.Collections.Generic;

public class UnitController : MonoBehaviour
{
	public delegate void UnitEventHandler(GameObject unit);
	public event UnitEventHandler OnUnitCreated;

	public delegate void SelectedUnitsEventHandler(List<GameObject> selectedUnits);
	public event SelectedUnitsEventHandler OnSelectedUnitsUpdated;

	public delegate void PathEventHandler(int unitID, Vector3 waypoints);
	public event PathEventHandler OnPathCreated;

	private static int m_nextUnitID;

	[SerializeField] private GameObject m_unitPrefab;
	private List<GameObject> m_units;
	private List<GameObject> m_selectedUnits;

	private GameObject m_gameManager;
	private Player m_player;
	private SelectionManager m_selectionManager;
	private PathfindingController m_pathfindingController;

	private Ray m_ray;

	public static int NextUnitID
	{
		get { return m_nextUnitID++; }
	}

	public List<GameObject> Units
	{
		get
		{
			return m_units;
		}
	}

	public List<GameObject> SelectedUnits
	{
		get
		{
			return m_selectedUnits;
		}
	}

	void Awake()
	{
		// TODO: This could be an issue if a player is added mid-game and m_nextUnitID is reset to 0
		m_nextUnitID = 0;

		m_units = new List<GameObject>();
		m_selectedUnits = new List<GameObject>();

		m_gameManager = GameObject.FindGameObjectWithTag("GameManager");
		m_player = GetComponent<Player>();
		m_selectionManager = GetComponent<SelectionManager>();
		m_pathfindingController = m_gameManager.GetComponent<PathfindingController>();
		m_ray = GetComponent<InputManager>().Ray;
	}

	void OnEnable()
	{
		m_selectionManager.OnNoObjectSelected += DeselectUnits;
		m_selectionManager.OnUnitSelected += UpdateSelectedUnitList;
		InputManager.OnMouseEvent += MouseInput;
	}

	void OnDisable()
	{
		m_selectionManager.OnNoObjectSelected -= DeselectUnits;
		m_selectionManager.OnUnitSelected -= UpdateSelectedUnitList;
		InputManager.OnMouseEvent -= MouseInput;
	}

	void Update()
	{
		m_ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		//Debug.DrawRay(m_ray.origin, m_ray.direction * InputManager.kRaycastLength, Color.cyan);

		if (Input.GetKeyDown(KeyCode.Space))
		{
			CreateUnitOnMouse();
		}
	}

	void MouseInput(InputManager.MouseEventType eventType, RaycastHit hitInfo)
	{
		switch (eventType)
		{
			case InputManager.MouseEventType.OnRightMouseDown:
				{
					if (m_selectedUnits.Count < 1)
					{
						break;
					}

					SteeringController steeringController = m_selectedUnits[0].GetComponent<SteeringController>();
					steeringController.TurnOffBehaviour(SteeringController.BehaviourType.Flocking);

					if (Input.GetKey(KeyCode.LeftShift))
					{
						steeringController.AddWaypoint(hitInfo.point, true);
					}
					else
					{
						AStarSearch search = new AStarSearch(m_pathfindingController.CellCount);
						Vector3[] waypoints = search.Search(m_pathfindingController.NavGraph, m_selectedUnits[0].transform.position, hitInfo.point);
						
						// If a path to the target was found, add the path as waypoints
						if (waypoints != null)
						{
							steeringController.PathFollowing.Path.Loop = false;
							steeringController.AddWaypoints(waypoints);
						}
					}

					if (m_selectedUnits.Count > 1)
					{
						//Vector3[] formationPositions = Formations.CalculateSquareFormation(m_selectedUnits.Count, 2f, 2f);

						// Start from 1 as unit 0 is the leader
						for (int i = 1; i < m_selectedUnits.Count; i++)
						{
							// TODO: Change this. Temporary A* search for all selected units
							AStarSearch search = new AStarSearch(m_pathfindingController.CellCount);
							Vector3[] waypoints = search.Search(m_pathfindingController.NavGraph, m_selectedUnits[i].transform.position, hitInfo.point);

							// If a path to the target was found, add the path as waypoints
							if (waypoints != null)
							{
								m_selectedUnits[i].GetComponent<SteeringController>().PathFollowing.Path.Loop = false;
								m_selectedUnits[i].GetComponent<SteeringController>().AddWaypoints(waypoints);
							}


							//steeringController = m_selectedUnits[i].GetComponent<SteeringController>();
							//steeringController.TurnOnBehaviour(SteeringController.BehaviourType.Flocking);

							// TODO: Do I need to add all selected units as neighbours?

							//steeringController.OffsetPursuit.Leader = m_selectedUnits[0].GetComponent<Rigidbody>();
							//steeringController.OffsetPursuit.Offset = formationPositions[i];
							//steeringController.TurnOnBehaviour(SteeringController.BehaviourType.OffsetPursuit);
						}
					}

					break;
				}
		}
	}

	void UpdateSelectedUnitList(List<GameObject> selectedObjects, bool modifyCurrentSelection)
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

	private void CreateUnitOnMouse()
	{
		RaycastHit hitObject;
		if (Physics.Raycast(m_ray, out hitObject, InputManager.kRaycastLength))
		{
			Vector3 position = new Vector3(hitObject.point.x, hitObject.point.y + 0.7f, hitObject.point.z);
			GameObject newUnit = Instantiate(m_unitPrefab, position, Quaternion.identity) as GameObject;
			m_units.Add(newUnit);

			if (OnUnitCreated != null)
			{
				OnUnitCreated(newUnit);
			}

			newUnit.GetComponent<SteeringController>().TurnOnBehaviour(SteeringController.BehaviourType.ObstacleAvoidance);
			Physics.IgnoreCollision(newUnit.GetComponent<SphereCollider>(), GameObject.FindGameObjectWithTag("Terrain").GetComponent<Collider>());
		}
	}
}
