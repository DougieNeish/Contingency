using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectionUIController : MonoBehaviour
{
	[SerializeField] private int m_lineSegments;
	[SerializeField] private float m_circleRadius;
	[SerializeField] private float m_circleWidth;
	[SerializeField] private float m_selectionBoxBorderThickness;

	private UnitController m_unitController;

	private Texture2D m_selectionBoxTexture;
	private Vector3 m_mouseDownPosition;
	private Vector3 m_currentMousePosition;
	private bool m_mouseIsDragging;

	void Awake()
	{
		m_unitController = GetComponent<UnitController>();

		m_selectionBoxTexture = new Texture2D(1, 1);
		m_selectionBoxTexture.SetPixel(0, 0, Color.white);
		m_selectionBoxTexture.Apply();
		m_mouseIsDragging = false;
    }

	void Start()
	{
		UnitController.OnUnitCreated += SetLineRendererToCircle;
		UnitController.OnSelectedUnitsUpdated += UpdateSelectionMarkers;
		SelectionManager.OnMultiSelectionStart += EnableSelectionBox;
		SelectionManager.OnMultiSelectionEnd += DisableSelectionBox;
	}
	
	void OnEnable()
	{

	}

	void OnDisable()
	{
		UnitController.OnUnitCreated -= SetLineRendererToCircle;
		UnitController.OnSelectedUnitsUpdated -= UpdateSelectionMarkers;
		SelectionManager.OnMultiSelectionStart -= EnableSelectionBox;
		SelectionManager.OnMultiSelectionEnd -= DisableSelectionBox;
	}

	void Update()
	{
	}

	void OnGUI()
	{
		if (m_mouseIsDragging)
		{
			// Draw selection box
			Rect rect = GetScreenRect(m_mouseDownPosition, m_currentMousePosition);
			DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
			DrawScreenRectBorder(rect, m_selectionBoxBorderThickness, new Color(0.8f, 0.8f, 0.95f));
		}
	}

	private void SetLineRendererToCircle(GameObject unit)
	{
		LineRenderer line = unit.GetComponent<LineRenderer>();

		if (line == null)
		{
			return;
		}

		line.SetVertexCount(m_lineSegments + 1);
		line.SetWidth(m_circleWidth, m_circleWidth);
		line.useWorldSpace = false;

		float x;
		float y = -0.5f;
		float z;
		float angle = 0.0f;

		for (int i = 0; i < (m_lineSegments + 1); i++)
		{
			x = Mathf.Sin(Mathf.Deg2Rad * angle) * m_circleRadius;
			z = Mathf.Cos(Mathf.Deg2Rad * angle) * m_circleRadius;

			line.SetPosition(i, new Vector3(x, y, z));

			angle += (360f / m_lineSegments);
		}
	}

	private void UpdateSelectionMarkers(List<GameObject> selectedUnits)
	{
		List<GameObject> units = m_unitController.Units;
		foreach (GameObject unit in units)
		{
			if (selectedUnits.Contains(unit))
			{
				unit.GetComponent<LineRenderer>().enabled = true;
			}
			else
			{
				unit.GetComponent<LineRenderer>().enabled = false;
			}
		}
	}

	private void EnableSelectionBox(Vector3 mouseDownPosition, Vector3 currentMousePosition)
	{
		m_mouseDownPosition = mouseDownPosition;
		m_currentMousePosition = currentMousePosition;
		m_mouseIsDragging = true;
	}

	private void DisableSelectionBox()
	{
		m_mouseIsDragging = false;
	}

	private Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
	{
		// Move origin from bottom left to top left
		screenPosition1.y = Screen.height - screenPosition1.y;
		screenPosition2.y = Screen.height - screenPosition2.y;

		Vector3 topLeft = Vector3.Min(screenPosition1, screenPosition2);
		Vector3 bottomRight = Vector3.Max(screenPosition1, screenPosition2);

		return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
	}

	private void DrawScreenRect(Rect rect, Color color)
	{
		GUI.color = color;
		GUI.DrawTexture(rect, m_selectionBoxTexture);
		//GUI.color = Color.white;
	}

	private void DrawScreenRectBorder(Rect rect, float thickness, Color color)
	{
		// Top
		DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
		// Left
		DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
		// Right
		DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
		// Bottom
		DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
	}
}
