using UnityEngine;

public class GraphNode
{
	private int m_index;
	private Vector3 m_position;
	private bool m_enabled;
	private GraphEdge[] m_edges;

	private GraphNode m_parent;

	public const int kInvalidIndex = -1;
	public const int kMaxEdges = 8;

	public GraphNode(int index, Vector3 position)
	{
		m_index = index;
		m_position = position;
		m_enabled = true;

		m_edges = new GraphEdge[kMaxEdges];
	}

	public int Index
	{
		get { return m_index; }
		set { m_index = value; }
	}

	public Vector3 Position
	{
		get { return m_position; }
		set { m_position = value; }
	}

	public bool Enabled
	{
		get { return m_enabled; }
	}

	public GraphEdge[] Edges
	{
		get { return m_edges; }
		set { m_edges = value; }
	}

	public GraphNode Parent
	{
		get { return m_parent; }
		set { m_parent = value; }
	}

	public void Enable(Graph graph)
	{
		m_enabled = true;
		m_edges = new GraphEdge[kMaxEdges];
	}

	public void Disable(Graph graph)
	{
		m_enabled = false;

		for (int i = 0; i < m_edges.Length; i++)
		{
			if (m_edges[i] != null && m_edges[i].To != kInvalidIndex)
			{
				int otherNode = m_edges[i].To;

				for (int j = 0; j < graph.Nodes[otherNode].Edges.Length; j++)
				{
					if (graph.Nodes[otherNode].Edges[j] != null &&
						graph.Nodes[otherNode].Edges[j].To == m_edges[i].From)
					{
						graph.Nodes[otherNode].Edges[j].To = kInvalidIndex;
					}
				}

				m_edges[i].To = kInvalidIndex;
				m_edges[i].From = kInvalidIndex;
			}
		}
	}
}
