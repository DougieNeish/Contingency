using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitController : MonoBehaviour
{
	public delegate void UnitEventHandler(GameObject unit);
	public static event UnitEventHandler OnUnitCreated;

	public delegate void SelectedUnitsEventHandler(List<GameObject> selectedUnits);
	public static event SelectedUnitsEventHandler OnSelectedUnitsUpdated;

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
	//private const int kRaycastLength = 1000;

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
		Debug.DrawRay(m_ray.origin, m_ray.direction * InputManager.kRaycastLength, Color.cyan);

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
					//SetSelectedUnitsTargetPosition(hitInfo.point);
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
		OnSelectedUnitsUpdated(m_selectedUnits);
	}

	private void DeselectUnits()
	{
		m_selectedUnits.Clear();
		OnSelectedUnitsUpdated(m_selectedUnits);
	}

	private void SetSelectedUnitsTargetPosition(Vector3 position)
	{
		foreach (GameObject unit in m_selectedUnits)
		{
			//unit.GetComponent<NavMeshAgent>().destination = position;
			//SteeringController steeringController = unit.GetComponent<SteeringController>();
			//steeringController.TurnOnBehaviour(SteeringController.BehaviourType.Arrive);
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
		if (Physics.Raycast(m_ray, out hitObject, InputManager.kRaycastLength))
		{
			Vector3 position = new Vector3(hitObject.point.x, hitObject.point.y + 0.7f, hitObject.point.z);
			GameObject newUnit = Instantiate(m_unitPrefab, position, Quaternion.identity) as GameObject;
			//Unit newUnit = newGameObject.GetComponent<Unit>();
			m_units.Add(newUnit);
			OnUnitCreated(newUnit);
		}
	}
}
