using UnityEngine;
using System.Collections.Generic;

public class UnitController : MonoBehaviour
{
	public delegate void UnitEventHandler(GameObject unit);
	public static event UnitEventHandler OnUnitCreated;

	public delegate void SelectedUnitsEventHandler(List<GameObject> selectedUnits);
	public static event SelectedUnitsEventHandler OnSelectedUnitsUpdated;

	private const float kEnableWaypointLoopDistance = 2f;

	public List<GameObject> Units
	{
		get
		{
			return m_units;
		}
	}

	[SerializeField] private GameObject m_unitPrefab;
	private List<GameObject> m_units;
	private List<GameObject> m_selectedUnits;

	private Ray m_ray;

	void Awake()
	{
		m_units = new List<GameObject>();
		m_selectedUnits = new List<GameObject>();
		m_ray = InputManager.Ray;
	}

	void Start()
	{
		SelectionManager.OnNoObjectSelected += DeselectUnits;
        SelectionManager.OnUnitSelected += UpdateSelectedUnitList; // Moving to OnEnable causes call to SelectionManager while it's null
		InputManager.OnMouseEvent += MouseInput;
	}

	void OnEnable()
	{
		
	}

	void OnDisable()
	{
		SelectionManager.OnNoObjectSelected -= DeselectUnits;
		SelectionManager.OnUnitSelected -= UpdateSelectedUnitList;
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
						SteeringUtils.AddWaypoint(steeringController, hitInfo.point);
					}
					else
					{
						steeringController.PathFollowing.Path.Loop = false;
						SteeringUtils.AddWaypoint(steeringController, hitInfo.point, true);
					}

					if (m_selectedUnits.Count > 1)
					{
						// Start from 1 as unit 0 is the leader
						for (int i = 1; i < m_selectedUnits.Count; i++)
						{
							steeringController = m_selectedUnits[i].GetComponent<SteeringController>();
							// Offset pursuit would probably be better than flocking
							SteeringUtils.SetFlocking(steeringController, m_selectedUnits);
						}
					}

					break;
				}
		}
	}

	void UpdateSelectedUnitList(List<GameObject> selectedObjects, bool shiftModifier)
	{
		if (selectedObjects == null)
		{
			m_selectedUnits.Clear();
		}
		else
		{
			if (shiftModifier)
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

			SteeringController.Obstacles = m_units.ToArray();
			newUnit.GetComponent<SteeringController>().TurnOnBehaviour(SteeringController.BehaviourType.ObstacleAvoidance);
			Physics.IgnoreCollision(newUnit.GetComponent<SphereCollider>(), GameObject.FindGameObjectWithTag("Terrain").GetComponent<Collider>());
		}
	}
}
