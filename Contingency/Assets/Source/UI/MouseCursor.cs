using UnityEngine;
using System.Collections.Generic;

public class MouseCursor : MonoBehaviour
{
	[SerializeField] private Texture2D m_move1;
	[SerializeField] private Texture2D m_move2;

	[SerializeField] private Texture2D m_patrol1;
	[SerializeField] private Texture2D m_patrol2;

	[SerializeField] private Texture2D m_attack1;
	[SerializeField] private Texture2D m_attack2;

	private Player m_player;
	private UnitController m_unitController;

	private bool m_unitsSelected;
	private bool m_mouseOverEnemy;

	void Awake()
	{
		m_player = GetComponent<Player>();
		m_unitController = GetComponent<UnitController>();

		m_unitsSelected = false;
		m_mouseOverEnemy = false;
	}

	void OnEnable()
	{
		InputManager.OnMouseEvent += MouseInput;
		m_unitController.OnSelectedUnitsUpdated += SelectedUnitUpdated;
	}

	void OnDisable()
	{
		InputManager.OnMouseEvent -= MouseInput;
		m_unitController.OnSelectedUnitsUpdated -= SelectedUnitUpdated;
	}

	private void MouseInput(InputManager.MouseEventType eventType, RaycastHit hitInfo)
	{
		switch (eventType)
		{
			case InputManager.MouseEventType.OnRightMouseDown:
				{
					if (m_unitsSelected)
					{
						if (m_mouseOverEnemy)
						{
							Cursor.SetCursor(m_attack2, getCursorCentre(m_attack2), CursorMode.ForceSoftware);
						}
						else
						{
							Cursor.SetCursor(m_move2, getCursorCentre(m_move2), CursorMode.ForceSoftware);
						}
					}

					break;
				}

			case InputManager.MouseEventType.OnRightMouseUp:
				{
					if (m_unitsSelected)
					{
						if (m_mouseOverEnemy)
						{
							Cursor.SetCursor(m_attack1, getCursorCentre(m_attack1), CursorMode.ForceSoftware);
						}
						else
						{
							Cursor.SetCursor(m_move1, getCursorCentre(m_move1), CursorMode.ForceSoftware);
						}
					}

					break;
				}

			case InputManager.MouseEventType.OnMouseOverEnter:
				{
					if (m_unitsSelected)
					{
						GameObject target = hitInfo.transform.gameObject;

						if (target.tag == "Unit" && target.GetComponent<Unit>().Owner.ID != m_player.ID ||
							target.tag == "Static/Building" && target.GetComponent<Building>().Owner.ID != m_player.ID)
						{
							Cursor.SetCursor(m_attack1, getCursorCentre(m_attack1), CursorMode.ForceSoftware);
							m_mouseOverEnemy = true;
						}
					}
					break;
				}

			case InputManager.MouseEventType.OnMouseOverExit:
				{
					if (m_unitsSelected)
					{
						Cursor.SetCursor(m_move1, getCursorCentre(m_move1), CursorMode.ForceSoftware);
						m_mouseOverEnemy = false;
					}
					else
					{
						Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
					}
					break;
				}
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
