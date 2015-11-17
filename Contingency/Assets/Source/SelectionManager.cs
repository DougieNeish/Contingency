using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour
{
	public delegate void ObectNotSelectedEventHandler();
	public event ObectNotSelectedEventHandler OnNoObjectSelected;

	public delegate void ObjectSelectedEventHandler(List<GameObject> selectedObjects, bool shiftModifier); // TODO: rename shiftModifier - it's an addTo/RemoveFromCurrentSelection flag
	public event ObjectSelectedEventHandler OnUnitSelected;

	public delegate void MultiSelectionStartEventHandler(Vector3 mouseDownPosition, Vector3 currentMousePosition);
	public event MultiSelectionStartEventHandler OnMultiSelectionStart;

	public delegate void MultiSelectionEndEventHandler();
	public event MultiSelectionEndEventHandler OnMultiSelectionEnd;

	private List<GameObject> m_selectedUnits;

	void Awake()
	{
		m_selectedUnits = new List<GameObject>();
	}

	void Start()
	{
		Game.Instance.InputManager.OnMouseEvent += SelectionFromMouseEvents; // Moving to OnEnable causes call to InputManager while it's null
		Game.Instance.InputManager.OnMouseDrag += SelectionFromMouseDrag;
	}

	void OnEnable()
	{
		
	}

	void OnDisable()
	{
		Game.Instance.InputManager.OnMouseEvent -= SelectionFromMouseEvents;
		Game.Instance.InputManager.OnMouseDrag -= SelectionFromMouseDrag;
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
						OnNoObjectSelected();
					}

					if (m_selectedUnits.Count > 0)
					{
						if (Input.GetKey(KeyCode.LeftShift))
						{
							OnUnitSelected(m_selectedUnits, true);
						}
						else
						{
							OnUnitSelected(m_selectedUnits, false);
						}
						m_selectedUnits.Clear();
					}
					break;
				}

			case InputManager.MouseEventType.OnLeftMouseUp:
				{
					m_selectedUnits.Clear();
					OnMultiSelectionEnd();
                    break;
				}
		}
	}

	private void SelectionFromMouseDrag(Vector3 mouseDownPosition, Vector3 currentMousePosition)
	{
		OnMultiSelectionStart(mouseDownPosition, currentMousePosition);

		List<Unit> units = Game.Instance.UnitController.Units;
		foreach (Unit unit in units)
		{
			GameObject unitGameObject = unit.transform.gameObject;
			bool alreadySelected = m_selectedUnits.Contains(unitGameObject);

			if (!alreadySelected && IsObjectWithinSelectionBounds(unitGameObject, mouseDownPosition, currentMousePosition))
			{
				m_selectedUnits.Add(unitGameObject);
            }
			// Remove units that are no longer within the bounds
			else if (alreadySelected && !IsObjectWithinSelectionBounds(unitGameObject, mouseDownPosition, currentMousePosition))
			{
				m_selectedUnits.Remove(unitGameObject);
			}
		}
		OnUnitSelected(m_selectedUnits, false);
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
