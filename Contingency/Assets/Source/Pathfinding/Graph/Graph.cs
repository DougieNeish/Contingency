using UnityEngine.Assertions;

public class Graph
{
	private int m_columnCount;
	private int m_rowCount;
	private GraphNode[] m_nodes;
	private int m_nextNodeIndex;

	public int NextNodeIndex
	{
		get { return m_nextNodeIndex; }
	}

	public Graph(int columnCount, int rowCount)
	{
		m_columnCount = columnCount;
		m_rowCount = rowCount;
		m_nodes = new GraphNode[m_columnCount * m_rowCount];
		m_nextNodeIndex = 0;
	}

	public int ColumnCount
	{
		get { return m_columnCount; }
	}

	public int RowCount
	{
		get { return m_rowCount; }
	}

	public GraphNode[] Nodes
	{
		get { return m_nodes; }
		set { m_nodes = value; }
	}

	public void AddNode(GraphNode node)
	{
		// If node already exists
		if (node.Index < m_nextNodeIndex)
		{
			// Ensure existing node is not overwritten, unless it has been deactivated
			Assert.IsTrue(node.Index == GraphNode.kInvalidIndex, "Graph.AddNode : Trying to add node with duplicate ID");
			m_nodes[node.Index] = node;
		}
		else
		{
			// Ensure a valid ID is being used
			Assert.IsTrue(node.Index == m_nextNodeIndex, "Graph.AddNode : Invalid node index");
			m_nodes[m_nextNodeIndex++] = node;
		}
	}

	public void AddEdge(GraphEdge edge)
	{
		Assert.IsTrue(edge.From.Index >= 0 && edge.From.Index < m_nodes.Length, "Graph.AddEdge : 'from' node index out of range");
		Assert.IsTrue(edge.To.Index >= 0 && edge.To.Index < m_nodes.Length, "Graph.AddEdge : 'to' node index out of range");

		// Ensure nodes are valid before creating edge between them
		if (edge.To.Index != GraphNode.kInvalidIndex &&
			edge.From.Index != GraphNode.kInvalidIndex)
		{
			if (!EdgeExists(edge.From, edge.To))
			{
				GraphNode node = edge.From;
				// Find first available (i.e. null) edge slot
				for (int i = 0; i < node.Edges.Length; i++)
				{
					if (node.Edges[i] == null)
					{
						node.Edges[i] = edge;
						break;
					}
				}
			}
		}
	}

	private bool EdgeExists(GraphNode from, GraphNode to)
	{
		for (int i = 0; i < from.Edges.Length; i++)
		{
			if (from.Edges[i] != null && 
				from.Edges[i].To.Index == to.Index)
			{
				return true;
			}
		}
		return false;
	}
}
