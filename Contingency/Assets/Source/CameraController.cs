using UnityEngine;

public class CameraController : MonoBehaviour
{
	private GameObject m_cameraChild;
	private RaycastHit m_hit;

	[SerializeField] private float m_moveSpeed = 300f;
	[SerializeField] private float m_rotationSpeed = 150f;
	[SerializeField] private float m_zoomAmount = 20f;
	[SerializeField] private float m_zoomSpeed = 100f;

	private bool m_isRotating;
	private Vector3 m_rotatePosition;

	private Quaternion m_targetRotation;
	private float m_targetRotationY;
	private float m_targetRotationX;
	private float m_mouseRotationSpeed = 5f;
	private float m_smoothness = 0.85f;

	private float m_zoomTolerance = 0.5f;
	private Vector3 m_targetZoom;

	void Start()
	{
		m_cameraChild = transform.GetChild(0).gameObject;
		m_targetZoom = transform.position;
		m_isRotating = false;

		m_targetRotation = transform.rotation;
		m_targetRotationY = transform.localRotation.eulerAngles.y;
		m_targetRotationX = transform.localRotation.eulerAngles.x;
	}
	
	void Update()
	{
		// Zoom
		if (Input.GetAxis("Mouse ScrollWheel") != 0f)
		{
			m_targetZoom = transform.position + (Input.GetAxis("Mouse ScrollWheel") * m_zoomAmount * transform.forward);
		}
		
		if(Vector3.Distance(transform.position, m_targetZoom) > m_zoomTolerance)
		{
			transform.position = Vector3.Lerp(transform.position, m_targetZoom, Time.deltaTime * m_zoomSpeed);
		}
		else 
		{
			m_targetZoom = transform.position;
		}
		
		
		if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 || m_isRotating)
		{
			m_targetZoom = transform.position;
		}

		// Rotation
		if (Physics.Raycast(transform.position, transform.forward, out m_hit, float.MaxValue))
		{
			m_rotatePosition = m_hit.point;
		}

		if (Input.GetKey(KeyCode.E))
		{
			transform.RotateAround(m_rotatePosition, Vector3.up, m_rotationSpeed * Time.deltaTime);
			m_isRotating = true;
		}

		if (Input.GetKey(KeyCode.Q))
		{
			transform.RotateAround(m_rotatePosition, Vector3.up, -m_rotationSpeed * Time.deltaTime);
			m_isRotating = true;
		}

		if (Input.GetKeyUp(KeyCode.E) || Input.GetKeyUp(KeyCode.Q))
		{
			m_isRotating = false;
		}
		
		// Tilt
		if (Input.GetMouseButton(2))
		{
			Cursor.visible = false;
			//m_targetRotationY += Input.GetAxis("Mouse X") * m_rotationSpeed;
			m_targetRotationY = transform.localRotation.eulerAngles.y;
			m_targetRotationX -= Input.GetAxis("Mouse Y") * m_mouseRotationSpeed;
			m_targetRotation = Quaternion.Euler(m_targetRotationX, m_targetRotationY, 0f);

			transform.rotation = Quaternion.Lerp(transform.rotation, m_targetRotation, (1f - m_smoothness));
		}
		else
		{
			Cursor.visible = true;
		}

		// Position
		transform.position += Input.GetAxis("Horizontal") * transform.right * (m_moveSpeed  * Time.deltaTime);
		transform.position += Input.GetAxis("Vertical") * m_cameraChild.transform.forward * (m_moveSpeed * Time.deltaTime);

		// Clamp Y position
		transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, 25f, 180f), transform.position.z);
	}
}
