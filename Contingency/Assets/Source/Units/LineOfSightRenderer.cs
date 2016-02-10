using UnityEngine;
using System.Collections;

public class LineOfSightRenderer : MonoBehaviour
{
	private Player m_owner;
	private Renderer m_renderer;

	private int m_seenByUnits;

	void Awake()
	{
		m_owner = gameObject.GetComponentInParent<Unit>().Owner;
		m_renderer = gameObject.transform.parent.GetComponent<Renderer>();

		m_seenByUnits = 0;
	}

	void Start()
	{
		Physics.IgnoreCollision(GetComponent<SphereCollider>(), GameObject.FindGameObjectWithTag("Terrain").GetComponent<Collider>());
	}

	void Update()
	{
		m_owner = gameObject.GetComponentInParent<Unit>().Owner;
		if (m_owner.Type == Player.PlayerType.AI)
		{
			m_renderer.enabled = m_seenByUnits == 0 ? false : true;

			//if (m_seenByUnits == 0)
			//{
			//	m_renderer.enabled = false;
			//}
			//else
			//{
			//	m_renderer.enabled = true;
			//}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Unit")
		{
			// Only counts as 'seen' if the other unit belongs to a human player

			Unit unit = other.GetComponent<Unit>();
			if (unit.Owner.Type == Player.PlayerType.Human)
			{
				m_seenByUnits++;
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Unit")
		{
			Unit unit = other.GetComponent<Unit>();
			if (unit.Owner.Type == Player.PlayerType.Human)
			{
				m_seenByUnits--;
			}
		}
	}

	// TODO: FINISH ME
	//private bool HasDirectLineOfSight(GameObject unit, GameObject target)
	//{
	//	Vector3 direction = target.transform.position - unit.transform.position;
	//	RaycastHit hit;
	//	//Debug.DrawRay(unit.transform.position, direction, Color.cyan, 5f);
	//	if (Physics.Raycast(unit.transform.position, direction, out hit, unit.AttackRange))
	//	{
	//		return hit.transform == target.transform;
	//	}

	//	return false;
	//}
}
