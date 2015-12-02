using UnityEngine;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour
{
	public delegate void ObectNotSelectedEventHandler();
	public static event ObectNotSelectedEventHandler OnNoObjectSelected;

	public delegate void ObjectSelectedEventHandler(List<GameObject> selectedObjects, bool shiftModifier); // TODO: rename shiftModifier - it's an addTo/RemoveFromCurrentSelection flag
	public static event ObjectSelectedEventHandler OnUnitSelected;

	public delegate void MultiSelectionStartEventHandler(Vector3 mouseDownPosition, Vector3 currentMousePosition);
	public static event MultiSelectionStartEventHandler OnMultiSelectionStart;

	public delegate void MultiSelectionEndEventHandler();
	public static event MultiSelectionEndEventHandler OnMultiSelectionEnd;

	private List<GameObject> m_selectedUnits;
	private UnitController m_unitController;

	void Awake()
	{
		m_selectedUnits = new List<GameObject>();
		m_unitController = GetComponent<UnitController>();
	}

	void Start()
	{
		InputManager.OnMouseEvent += SelectionFromMouseEvents; // Moving to OnEnable causes call to InputManager while it's null
		InputManager.OnMouseDrag += SelectionFromMouseDrag;
	}

	void OnEnable()
	{
		
	}

	void OnDisable()
	{
		InputManager.OnMouseEvent -= SelectionFromMouseEvents;
		InputManager.OnMouseDrag -= SelectionFromMouseDrag;
	}	

	void Update()
	{
	}

	private void SelectionFromMouseEvents(InputManager.MouseEventType eventType, RaycastHit hitInfo)
	{
		switch (eventType)
		{
			case InputManager.MouseEventType.OnLeftMouseDown:
				{
					if (hitInfo.transform.tag == "Unit")
					{
						m_selectedUnits.Add(hitInfo.transform.gameObject);
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

			if (!alreadySelected && IsObjectWithinSelectionBounds(unit, mouseDownPosition, currentMousePosition))
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
}
