using UnityEngine;

public class Unit : MonoBehaviour
{
	private int m_id;
	private Player m_owner;

	public int ID
	{
		get { return m_id; }
	}

	public Player Owner
	{
		get { return m_owner; }
		set { m_owner = value; }
	}

	void Awake()
	{
		m_id = UnitController.NextUnitID;
	}

	public static explicit operator GameObject(Unit unit)
	{
		return unit.gameObject;
	}
}
