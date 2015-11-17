using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour
{
	public enum MouseEventType
	{
		OnLeftMouseDown,
		OnLeftMouseUp,
		OnRightMouseDown,
		OnRightMouseUp,
		OnMouseOverEnter,
		OnMouseOverExit
	}

	public delegate void MouseInputEventHandler(MouseEventType eventType, RaycastHit hitInfo);
	public event MouseInputEventHandler OnMouseEvent;

	public delegate void MouseDragEventHandler(Vector3 mouseDownPosition, Vector3 currentMousePosition);
	public event MouseDragEventHandler OnMouseDrag;

	[SerializeField] private float m_mouseDragThreshold;
	private RaycastHit m_previousHitInfo;
	private Vector3 m_mouseDownPosition;

	void Start()
	{
	}
	
	void Update()
	{
		SendMouseEvents();
	}

	void SendMouseEvents()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		int raycastLength = 1000;
		RaycastHit hitInfo;

		if (Physics.Raycast(ray, out hitInfo, raycastLength) && OnMouseEvent != null)
		{
			if (Input.GetMouseButtonDown(0))
			{
				m_mouseDownPosition = Input.mousePosition;
				OnMouseEvent(MouseEventType.OnLeftMouseDown, hitInfo);
			}
			else if (Input.GetMouseButtonUp(0))
			{
				OnMouseEvent(MouseEventType.OnLeftMouseUp, hitInfo);
			}
			else if (Input.GetMouseButtonDown(1))
			{
				OnMouseEvent(MouseEventType.OnRightMouseDown, hitInfo);
			}
			else if (Input.GetMouseButtonUp(1))
			{
				OnMouseEvent(MouseEventType.OnRightMouseUp, hitInfo);
			}
			else if (m_previousHitInfo.transform == null)
			{
				OnMouseEvent(MouseEventType.OnMouseOverEnter, hitInfo);
				m_previousHitInfo = hitInfo;
			}
			// Check mouse is over different object to trigger MouseOverExit
			else if (hitInfo.transform.gameObject != m_previousHitInfo.transform.gameObject)
			{
				OnMouseEvent(MouseEventType.OnMouseOverExit, m_previousHitInfo);
				m_previousHitInfo = hitInfo;

				if (hitInfo.transform.name != "Terrain")
				{
					OnMouseEvent(MouseEventType.OnMouseOverEnter, hitInfo);
				}
			}

			if (Input.GetMouseButton(0))
			{
				float dragDistance = Vector3.Distance(m_mouseDownPosition, Input.mousePosition);
				if (dragDistance > m_mouseDragThreshold)
				{
					OnMouseDrag(m_mouseDownPosition, Input.mousePosition);
                }
			}
		}
	}
}
