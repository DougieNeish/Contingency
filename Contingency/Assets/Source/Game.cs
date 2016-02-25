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

			m_buildings[1].GetComponent<Building>().Owner = player.GetComponent<Player>();
		}

		for (int i = 0; i < kAIPlayerCount; i++)
		{
			player = Instantiate(m_AIPlayerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			player.transform.SetParent(gameObject.transform);
			m_players[kHumanPlayerCount + i] = player.GetComponent<Player>();

			m_buildings[0].GetComponent<Building>().Owner = player.GetComponent<Player>();
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			m_players[0].GetComponent<UnitController>().CreateUnitOnMouse();
		}

		if (Input.GetKeyDown(KeyCode.E))
		{
			m_players[1].GetComponent<UnitController>().CreateUnitOnMouse();
		}
	}
}
