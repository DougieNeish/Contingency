using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitController : MonoBehaviour
{
	public delegate void UnitEventHandler(Unit unit);
	public static event UnitEventHandler OnUnitCreated;

	public delegate void SelectedUnitsEventHandler(List<Unit> selectedUnits);
	public static event SelectedUnitsEventHandler OnSelectedUnitsUpdated;

	public List<Unit> Units
	{
		get
		{
			return m_units;
		}
	}

	[SerializeField] private GameObject m_unitPrefab;
	private List<Unit> m_units;
	private List<Unit> m_selectedUnits;

	private Ray m_ray;
	private int m_raycastLength;

	void Awake()
	{
		m_units = new List<Unit>();
		m_selectedUnits = new List<Unit>();

		m_raycastLength = 1000;
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
		Debug.DrawRay(m_ray.origin, m_ray.direction * m_raycastLength, Color.cyan);

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
					SetSelectedUnitsTargetPosition(hitInfo.point);
                    break;
				}
		}
	}

	void UpdateSelectedUnitList(List<GameObject> selectedGameObjects, bool shiftModifier)
	{
		if (selectedGameObjects == null)
		{
			m_selectedUnits.Clear();
		}
		else
		{
			List<Unit> selectedUnits = ConvertGameObjectsToUnits(selectedGameObjects);

			if (shiftModifier)
			{
				foreach (Unit unit in selectedUnits)
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

				foreach (Unit unit in selectedUnits)
				{
					if (!m_selectedUnits.Contains(unit))
					{
						m_selectedUnits.Add(unit);
					}
				}
			}
		}
		OnSelectedUnitsUpdated(m_selectedUnits);
	}

	private void DeselectUnits()
	{
		m_selectedUnits.Clear();
		OnSelectedUnitsUpdated(m_selectedUnits);
	}

	private void SetSelectedUnitsTargetPosition(Vector3 position)
	{
		foreach (Unit unit in m_selectedUnits)
		{
			//unit.GetComponent<NavMeshAgent>().destination = position;
			SteeringController steeringController = unit.GetComponent<SteeringController>();
			steeringController.TurnOnBehaviour(SteeringController.BehaviourType.Arrive);
			steeringController.TargetPosition = position;
			steeringController.FleeActivationDistance = 20.0f;
		}
	}

	private List<Unit> ConvertGameObjectsToUnits(List<GameObject> gameObjects)
	{
		List<Unit> unitList = new List<Unit>();
		foreach (GameObject item in gameObjects)
		{
			unitList.Add(item.GetComponent<Unit>());
		}
		return unitList;
	}

	private void CreateUnitOnMouse()
	{
		RaycastHit hitObject;
		if (Physics.Raycast(m_ray, out hitObject, m_raycastLength))
		{
			Vector3 position = new Vector3(hitObject.point.x, hitObject.point.y + 0.7f, hitObject.point.z);
			GameObject newGameObject = Instantiate(m_unitPrefab, position, Quaternion.identity) as GameObject;
			Unit newUnit = newGameObject.GetComponent<Unit>();
			m_units.Add(newUnit);
			OnUnitCreated(newUnit);
		}
	}
}
