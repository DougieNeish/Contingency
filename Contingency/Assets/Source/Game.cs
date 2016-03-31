using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour
{
	private static int m_nextPlayerID;

	private const int kHumanPlayerCount = 1;
	private const int kAIPlayerCount = 1;
	private const int kMinActiveEnemies = 5;
	private const float kEnemySpawnRate = 3f;

	private Player[] m_players;
	[SerializeField] private GameObject m_humanPlayerPrefab;
	[SerializeField] private GameObject m_AIPlayerPrefab;
	private GameObject m_humanBuilding;
	private GameObject m_AIBuilding;

	private InputManager m_inputManager;
	private bool m_unitSpawnMode;
	private int m_playerForSpawn;
	private UnitController.UnitType m_unitTypeForSpawn;

	public static int NextPlayerID
	{
		get { return m_nextPlayerID++; }
	}

	public int PlayerCount
	{
		get { return kHumanPlayerCount + kAIPlayerCount; }
	}

	void Awake()
	{
		m_nextPlayerID = 0;
		m_players = new Player[PlayerCount];
		m_humanBuilding = GameObject.FindGameObjectWithTag("Static/HumanBuilding");
		m_AIBuilding = GameObject.FindGameObjectWithTag("Static/AIBuilding");

		GameObject player;
		for (int i = 0; i < kHumanPlayerCount; i++)
		{
			player = Instantiate(m_humanPlayerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			player.transform.SetParent(gameObject.transform);
			m_players[i] = player.GetComponent<Player>();

			m_humanBuilding.GetComponent<Building>().Owner = player.GetComponent<Player>();
		}

		for (int i = 0; i < kAIPlayerCount; i++)
		{
			player = Instantiate(m_AIPlayerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			player.transform.SetParent(gameObject.transform);
			m_players[kHumanPlayerCount + i] = player.GetComponent<Player>();

			m_AIBuilding.GetComponent<Building>().Owner = player.GetComponent<Player>();
		}

		m_inputManager = m_players[0].GetComponent<InputManager>();
		m_unitSpawnMode = false;
		m_playerForSpawn = -1;
	}

	void Start()
	{
		StartCoroutine(SpawnEnemies());
	}

	void OnEnable()
	{
		m_inputManager.OnMouseEvent += HandleMouseInput;
	}

	void OnDisable()
	{
		m_inputManager.OnMouseEvent -= HandleMouseInput;
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.LeftShift))
		{
			m_unitSpawnMode = false;
		}
	}

	public void BeginSpawn(int player, UnitController.UnitType unitType)
	{
		m_unitSpawnMode = true;
		m_playerForSpawn = player;
		m_unitTypeForSpawn = unitType;
	}

	private void SpawnUnit(int player, UnitController.UnitType unitType)
	{
		m_players[player].GetComponent<UnitController>().SpawnUnitOnMouse(unitType);
	}

	private void HandleMouseInput(InputManager.MouseEventType eventType, RaycastHit hitInfo)
	{
		switch (eventType)
		{
			case InputManager.MouseEventType.OnLeftMouseDown:
				{
					if (m_unitSpawnMode)
					{
						SpawnUnit(m_playerForSpawn, m_unitTypeForSpawn);

						// If left shift is held down multiple units can be placed without re-selecting a unit type
						if (!Input.GetKey(KeyCode.LeftShift))
						{
							m_unitSpawnMode = false;
						}
					}
					break;
				}
			case InputManager.MouseEventType.OnRightMouseDown:
				{
					m_unitSpawnMode = false;
					break;
				}
		}
	}

	private IEnumerator SpawnEnemies()
	{
		// Delay to ensure everything is initialized first
		yield return new WaitForSeconds(3f);

		UnitController enemy = m_players[1].GetComponent<UnitController>();

		while (true)
		{
			if (enemy.Units.Count < kMinActiveEnemies && DebugInfo.Instance.AutoSpawnAI)
			{
				UnitController.UnitType randomUnit = (UnitController.UnitType)Random.Range(0, (int)UnitController.UnitType.Count);
				enemy.SpawnAutoUnit(randomUnit);
			}

			yield return new WaitForSeconds(kEnemySpawnRate);
		}
	}
}
