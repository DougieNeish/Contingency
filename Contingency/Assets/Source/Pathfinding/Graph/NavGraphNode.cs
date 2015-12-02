using UnityEngine;

public class NavGraphNode : GraphNode
{
	private Vector3 m_position;

	public NavGraphNode(int index, Vector3 position) : base(index)
	{
		m_position = position;
	}

	public Vector3 Position
	{
		get { return m_position; }
		set { m_position = value; }
	}
}
