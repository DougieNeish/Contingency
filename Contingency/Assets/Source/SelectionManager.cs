using UnityEngine;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour
{
	public delegate void ObectNotSelectedEventHandler();
	public event ObectNotSelectedEventHandler OnNoObjectSelected;

	public delegate void ObjectSelectedEventHandler(List<GameObject> selectedObjects, bool modifyCurrentSelection);
	public event ObjectSelectedEventHandler OnUnitSelected;

	public delegate void MultiSelectionStartEventHandler(Vector3 mouseDownPosition, Vector3 currentMousePosition);
	public event MultiSelectionStartEventHandler OnMultiSelectionStart;

	public delegate void MultiSelectionEndEventHandler();
	public event MultiSelectionEndEventHandler OnMultiSelectionEnd;

	private Player m_player;

	private List<GameObject> m_selectedUnits;
	private UnitController m_unitController;
	private InputManager m_inputManager;

	private List<GameObject>[] m_controlGroups;
	private int m_selectedControlGroup;

	private const int kMaxControlGroups = 10;
	private const int kInvalidControlGroup = -1;

	void Awake()
	{
		m_player = GetComponent<Player>();

		m_selectedUnits = new List<GameObject>();
		m_unitController = GetComponent<UnitController>();
		m_inputManager = GetComponent<InputManager>();

		m_controlGroups = new List<GameObject>[kMaxControlGroups];
		for (int i = 0; i < m_controlGroups.Length; i++)
		{
			m_controlGroups[i] = new List<GameObject>();
		}

		m_selectedControlGroup = kInvalidControlGroup;
	}

	void OnEnable()
	{
		m_inputManager.OnMouseEvent += SelectionFromMouseEvents;
		m_inputManager.OnMouseDrag += SelectionFromMouseDrag;
	}

	void OnDisable()
	{
		m_inputManager.OnMouseEvent -= SelectionFromMouseEvents;
		m_inputManager.OnMouseDrag -= SelectionFromMouseDrag;
	}

	void Update()
	{
		ControlGroupSelection();
	}

	// TODO: Move to debug info class
	Unit m_lastSelectedUnit;
	void OnGUI()
	{
		if (m_lastSelectedUnit != null)
		{
			GUI.Label(new Rect(5, 30, 100, 25), m_lastSelectedUnit.StateMachine.CurrentState.GetType().ToString());
		}
	}

	private void SelectionFromMouseEvents(InputManager.MouseEventType eventType, RaycastHit hitInfo)
	{
		switch (eventType)
		{
			case InputManager.MouseEventType.OnLeftMouseDown:
				{
					if (hitInfo.transform.tag == "Unit")
					{
						m_lastSelectedUnit = hitInfo.transform.GetComponent<Unit>();

						if (hitInfo.transform.gameObject.GetComponent<Unit>().Owner.ID == m_player.ID)
						{
							m_selectedUnits.Add(hitInfo.transform.gameObject);
						}
					}
					else
					{
						if (OnNoObjectSelected != null)
						{
							OnNoObjectSelected();
						}
					}

					if (m_selectedUnits.Count > 0)
					{
						if (OnUnitSelected != null)
						{
							if (Input.GetKey(KeyCode.LeftShift))
							{
								OnUnitSelected(m_selectedUnits, true);
							}
							else
							{
								OnUnitSelected(m_selectedUnits, false);
							}
						}
						m_selectedUnits.Clear();
					}
					break;
				}

			case InputManager.MouseEventType.OnLeftMouseUp:
				{
					m_selectedUnits.Clear();
					if (OnMultiSelectionEnd != null)
					{
						OnMultiSelectionEnd();
					}
                    break;
				}
		}
	}

	private void SelectionFromMouseDrag(Vector3 mouseDownPosition, Vector3 currentMousePosition)
	{
		if (OnMultiSelectionStart != null)
		{
			OnMultiSelectionStart(mouseDownPosition, currentMousePosition);
		}

		List<GameObject> units = m_unitController.Units;
		foreach (GameObject unit in units)
		{
			bool alreadySelected = m_selectedUnits.Contains(unit);

			if (!alreadySelected &&
				IsObjectWithinSelectionBounds(unit, mouseDownPosition, currentMousePosition) &&
				unit.GetComponent<Unit>().Owner.ID == m_player.ID)
			{
				m_selectedUnits.Add(unit);
            }
			// Remove units that are no longer within the bounds
			else if (alreadySelected && !IsObjectWithinSelectionBounds(unit, mouseDownPosition, currentMousePosition))
			{
				m_selectedUnits.Remove(unit);
			}
		}

		if (OnUnitSelected != null)
		{
			OnUnitSelected(m_selectedUnits, false);
		}
	}

	private bool IsObjectWithinSelectionBounds(GameObject gameObject, Vector3 mouseDownPosition, Vector3 currentMousePosition)
	{
		Bounds viewportBounds = GetViewportBounds(Camera.main, mouseDownPosition, currentMousePosition);
		return viewportBounds.Contains(Camera.main.WorldToViewportPoint(gameObject.transform.position));
	}

	private Bounds GetViewportBounds(Camera camera, Vector3 screenPosition1, Vector3 screenPosition2)
	{
		Vector3 boundsPosition1 = camera.ScreenToViewportPoint(screenPosition1);
		Vector3 boundsPosition2 = camera.ScreenToViewportPoint(screenPosition2);
		Vector3 min = Vector3.Min(boundsPosition1, boundsPosition2);
		Vector3 max = Vector3.Max(boundsPosition1, boundsPosition2);
		min.z = camera.nearClipPlane;
		max.z = camera.farClipPlane;

		Bounds bounds = new Bounds();
		bounds.SetMinMax(min, max);
		return bounds;
	}

	private void ControlGroupSelection()
	{
		m_selectedControlGroup = kInvalidControlGroup;

		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			m_selectedControlGroup = 1;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			m_selectedControlGroup = 2;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			m_selectedControlGroup = 3;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			m_selectedControlGroup = 4;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			m_selectedControlGroup = 5;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha6))
		{
			m_selectedControlGroup = 6;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha7))
		{
			m_selectedControlGroup = 7;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha8))
		{
			m_selectedControlGroup = 8;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha9))
		{
			m_selectedControlGroup = 9;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha0))
		{
			m_selectedControlGroup = 0;
		}

		if (m_selectedControlGroup != kInvalidControlGroup)
		{
			// Note: GetKey to detect key being held, not just pressed
			if (Input.GetKey(KeyCode.LeftShift))
			{
				m_controlGroups[m_selectedControlGroup].Clear();
				m_controlGroups[m_selectedControlGroup].AddRange(m_unitController.SelectedUnits);
			}
			else
			{
				if (OnUnitSelected != null)
				{
					OnUnitSelected(m_controlGroups[m_selectedControlGroup], false);
				}
			}
		}
	}
}
