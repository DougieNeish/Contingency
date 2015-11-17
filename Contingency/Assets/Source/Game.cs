using UnityEngine;
using System.Collections;

public class Game : Singleton<Game>
{
	public InputManager InputManager
	{
		get
		{
			return m_inputManager;
		}
	}

	public SelectionManager SelectionManager
	{
		get
		{
			return m_selectionManager;
		}
	}

	public UnitController UnitController
	{
		get
		{
			return m_unitController;
		}
	}

	private InputManager m_inputManager;
	private SelectionManager m_selectionManager;
	private UnitController m_unitController;
	
	void Awake()
	{
		m_inputManager = GetComponent<InputManager>();
		m_selectionManager = GetComponent<SelectionManager>();
		m_unitController = GetComponent<UnitController>();
	}

	void Start()
	{
	}
	
	void Update()
	{
	}
}
