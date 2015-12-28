using UnityEngine;
using System.Collections.Generic;

public class MouseCursor : MonoBehaviour
{
	[SerializeField] private Texture2D m_move1;
	[SerializeField] private Texture2D m_move2;

	[SerializeField] private Texture2D m_patrol1;
	[SerializeField] private Texture2D m_patrol;

	private bool m_unitsSelected;

	void Awake()
	{
		m_unitsSelected = false;
	}

	void OnEnable()
	{
		InputManager.OnMouseEvent += MouseInput;
		UnitController.OnSelectedUnitsUpdated += SelectedUnitUpdated;
	}

	void OnDisable()
	{
		InputManager.OnMouseEvent -= MouseInput;
		UnitController.OnSelectedUnitsUpdated -= SelectedUnitUpdated;
	}

	private void MouseInput(InputManager.MouseEventType eventType, RaycastHit hitInfo)
	{
		switch (eventType)
		{
			case InputManager.MouseEventType.OnRightMouseDown:
				{
					if (m_unitsSelected)
					{
						Cursor.SetCursor(m_move2, getCursorCentre(m_move2), CursorMode.ForceSoftware);
					}

					break;
				}

			case InputManager.MouseEventType.OnRightMouseUp:
				{
					if (m_unitsSelected)
					{
						Cursor.SetCursor(m_move1, getCursorCentre(m_move1), CursorMode.ForceSoftware);
					}

					break;
				}

			case InputManager.MouseEventType.OnMouseOverEnter:
				break;

			case InputManager.MouseEventType.OnMouseOverExit:
				break;

			default:
				break;
		}
	}

	private void SelectedUnitUpdated(List<GameObject> selectedUnits)
	{
		if (selectedUnits.Count > 0)
		{
			Cursor.SetCursor(m_move1, getCursorCentre(m_move1), CursorMode.ForceSoftware);
			m_unitsSelected = true;
		}
		else
		{
			Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
			m_unitsSelected = false;
		}
	}

	private Vector2 getCursorCentre(Texture2D cursor)
	{
		return new Vector2(cursor.width / 2, cursor.height / 2);
	}
}
