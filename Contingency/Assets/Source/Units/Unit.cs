using UnityEngine;

public class Unit : MonoBehaviour
{
	private int m_id;
	[SerializeField] private Player m_owner;

	[SerializeField] private float m_sightRadius;

	private Renderer m_renderer;

	public int ID
	{
		get { return m_id; }
	}

	public Player Owner
	{
		get { return m_owner; }
		set { m_owner = value; }
	}

	public float SightRadius
	{
		get { return m_sightRadius; }
	}

	void Awake()
	{
		m_id = UnitController.NextUnitID;
		m_renderer = GetComponent<Renderer>();
	}

	void Start()
	{
		gameObject.GetComponentInChildWithTag<SphereCollider>("SightRadius").radius = m_sightRadius;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(transform.position, m_sightRadius);
	}

	public static explicit operator GameObject(Unit unit)
	{
		return unit.gameObject;
	}
}
