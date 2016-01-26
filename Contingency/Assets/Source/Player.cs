using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	public enum PlayerType
	{
		Human,
		AI,
	}

	private int m_id;
	[SerializeField] private PlayerType m_type;

	public int ID
	{
		get { return m_id; }
	}

	public PlayerType Type
	{
		get { return m_type; }
	}

	void Awake()
	{		

	}
}
