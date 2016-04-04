using UnityEngine;

public class GraphNode
{
	public const int kInvalidIndex = -1;
	public const int kMaxEdges = 8;

	private int m_index;
	private Vector3 m_position;
	private bool m_enabled;

	// m_edges always has length kMaxEdges and should never be set to null
	// If an edge does not exist its position in the array is null
	private GraphEdge[] m_edges;
	private GraphNode m_parent;

	public GraphNode(int index, Vector3 position)
	{
		m_index = index;
		m_position = position;
		m_enabled = true;
		m_edges = new GraphEdge[kMaxEdges];
		m_parent = null;
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

	public void Enable()
	{
		m_enabled = true;

		// TODO: Add edges to enabled neighbouring nodes
	}

	public void Disable()
	{
		// Loop through all the node's edges
		for (int i = 0; i < m_edges.Length; i++)
		{
			// If edge is not null and points to a valid node index
			if (m_edges[i] != null &&
				m_edges[i].To.Index != kInvalidIndex)
			{
				GraphNode neighbour = m_edges[i].To;

				// Loop through the neighbour's edges to find edges that point to this node
				for (int j = 0; j < neighbour.Edges.Length; j++)
				{
					// If node is not null and points to this node
					if (neighbour.Edges[j] != null &&
						neighbour.Edges[j].To.Index == m_index)
					{
						// Set edge that points to this node to null
						neighbour.Edges[j] = null;
					}
				}

				// Set this node's current edge to null
				m_edges[i] = null;
			}
		}

		m_enabled = false;
	}
}
