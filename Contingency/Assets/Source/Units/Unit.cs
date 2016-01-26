using UnityEngine;

public class Unit : MonoBehaviour
{
	private int m_id;

	public int ID
	{
		get { return m_id; }
	}

	void Awake()
	{
		m_id = UnitController.NextUnitID;
	}
	
	void Update()
	{
	
	}

	public static explicit operator GameObject(Unit unit)
	{
		return unit.gameObject;
	}
}
