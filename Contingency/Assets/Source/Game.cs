using UnityEngine;

public class Game : MonoBehaviour
{
	private static int m_nextPlayerID;

	private const int kHumanPlayerCount = 1;
	private const int kAIPlayerCount = 1;

	private Player[] m_players;
	[SerializeField] GameObject m_humanPlayerPrefab;
	[SerializeField] GameObject m_AIPlayerPrefab;
	private GameObject[] m_buildings;

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
		m_buildings = GameObject.FindGameObjectsWithTag("Static/Building");
	}

	void Start()
	{
		GameObject player;
		for (int i = 0; i < kHumanPlayerCount; i++)
		{
			player = Instantiate(m_humanPlayerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			player.transform.SetParent(gameObject.transform);
			m_players[i] = player.GetComponent<Player>();

			m_buildings[0].GetComponent<Building>().Owner = player.GetComponent<Player>();
		}

		for (int i = 0; i < kAIPlayerCount; i++)
		{
			player = Instantiate(m_AIPlayerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			player.transform.SetParent(gameObject.transform);
			m_players[kHumanPlayerCount + i] = player.GetComponent<Player>();

			m_buildings[1].GetComponent<Building>().Owner = player.GetComponent<Player>();
		}
	}

	void Update()
	{
		//if (Input.GetKeyDown(KeyCode.Space))
		//{
		//	m_players[0].GetComponent<UnitController>().CreateUnitOnMouse(UnitController.UnitType.Laser);
		//}

		//if (Input.GetKeyDown(KeyCode.E))
		//{
		//	m_players[1].GetComponent<UnitController>().CreateUnitOnMouse(UnitController.UnitType.Laser);
		//}

		// Unit spawning
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			m_players[0].GetComponent<UnitController>().CreateUnitOnMouse(UnitController.UnitType.Laser);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			m_players[0].GetComponent<UnitController>().CreateUnitOnMouse(UnitController.UnitType.Sniper);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			m_players[0].GetComponent<UnitController>().CreateUnitOnMouse(UnitController.UnitType.Scout);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			m_players[1].GetComponent<UnitController>().CreateUnitOnMouse(UnitController.UnitType.Laser);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			m_players[1].GetComponent<UnitController>().CreateUnitOnMouse(UnitController.UnitType.Sniper);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha6))
		{
			m_players[1].GetComponent<UnitController>().CreateUnitOnMouse(UnitController.UnitType.Scout);
		}

		// Unit stance
		if (Input.GetKeyDown(KeyCode.J))
		{
			m_players[0].GetComponent<UnitController>().SetSelectedUnitsStance(Unit.CombatStance.Aggressive);
		}
		else if (Input.GetKeyDown(KeyCode.K))
		{
			m_players[0].GetComponent<UnitController>().SetSelectedUnitsStance(Unit.CombatStance.Defensive);
		}
		else if (Input.GetKeyDown(KeyCode.L))
		{
			m_players[0].GetComponent<UnitController>().SetSelectedUnitsStance(Unit.CombatStance.Static);
		}
	}
}
