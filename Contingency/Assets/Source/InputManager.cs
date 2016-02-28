using UnityEngine;

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

	public const int kRaycastLength = 1000;

	[SerializeField] private float m_mouseDragThreshold;
	private RaycastHit m_previousHitInfo;
	private Vector3 m_mouseDownPosition;

	private Ray m_ray;
	private RaycastHit m_hitInfo;

	public Ray Ray
	{
		get { return m_ray; }
	}

	void Update()
	{
		SendMouseEvents();
	}

	private void SendMouseEvents()
	{
		m_ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(m_ray, out m_hitInfo, kRaycastLength) && OnMouseEvent != null)
		{
			if (Input.GetMouseButtonDown(0))
			{
				m_mouseDownPosition = Input.mousePosition;
				OnMouseEvent(MouseEventType.OnLeftMouseDown, m_hitInfo);
			}
			else if (Input.GetMouseButtonUp(0))
			{
				OnMouseEvent(MouseEventType.OnLeftMouseUp, m_hitInfo);
			}
			else if (Input.GetMouseButtonDown(1))
			{
				OnMouseEvent(MouseEventType.OnRightMouseDown, m_hitInfo);
			}
			else if (Input.GetMouseButtonUp(1))
			{
				OnMouseEvent(MouseEventType.OnRightMouseUp, m_hitInfo);
			}
			else if (m_previousHitInfo.transform == null)
			{
				OnMouseEvent(MouseEventType.OnMouseOverEnter, m_hitInfo);
				m_previousHitInfo = m_hitInfo;
			}
			// Check mouse is over different object to trigger MouseOverExit
			else if (m_hitInfo.transform.gameObject != m_previousHitInfo.transform.gameObject)
			{
				OnMouseEvent(MouseEventType.OnMouseOverExit, m_previousHitInfo);
				m_previousHitInfo = m_hitInfo;

				if (m_hitInfo.transform.name != "Terrain")
				{
					OnMouseEvent(MouseEventType.OnMouseOverEnter, m_hitInfo);
				}
			}

			if (Input.GetMouseButton(0) && OnMouseDrag != null)
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
