using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour
{
	private static int m_nextPlayerID;

	private const int kMaxHumanPlayers = 1;
	private const int kMaxAIPlayers = 1;

	private Player[] m_players;
	[SerializeField] GameObject m_humanPlayerPrefab;
	[SerializeField] GameObject m_AIPlayerPrefab;

	public static int NextPlayerID
	{
		get { return m_nextPlayerID++; }
	}

	public int MaxPlayers
	{
		get { return kMaxHumanPlayers + kMaxAIPlayers; }
	}

	void Awake()
	{
		m_nextPlayerID = 0;
		m_players = new Player[MaxPlayers];
	}

	void Start()
	{
		GameObject player;
		for (int i = 0; i < kMaxHumanPlayers; i++)
		{
			player = Instantiate(m_humanPlayerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			player.transform.SetParent(gameObject.transform);
			m_players[i] = player.GetComponent<Player>();
		}

		for (int i = 0; i < kMaxAIPlayers; i++)
		{
			player = Instantiate(m_AIPlayerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			player.transform.SetParent(gameObject.transform);
			m_players[kMaxHumanPlayers + i] = player.GetComponent<Player>();
		}
	}
}
