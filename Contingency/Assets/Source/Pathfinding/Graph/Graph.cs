using UnityEngine.Assertions;

public class Graph
{
	private GraphNode[] m_nodes;
	private int m_nextNodeIndex;

	public Graph(int maxNodes)
	{
		m_nodes = new GraphNode[maxNodes];
		m_nextNodeIndex = 0;
	}

	public GraphNode[] Nodes
	{
		get { return m_nodes; }
		set { m_nodes = value; }
	}

	public void AddNode(GraphNode node)
	{
		if (node.Index < m_nextNodeIndex)
		{
			Assert.IsTrue(node.Index != GraphNode.kInvalidIndex, "Graph::AddNode : Trying to add node to index with existing node");
			m_nodes[node.Index] = node;
		}
		else
		{
			Assert.IsTrue(node.Index == m_nextNodeIndex, "Graph::AddNode : Invalid node index");
			m_nodes[m_nextNodeIndex++] = node;
		}
	}

	public void AddEdge(GraphEdge edge)
	{
		Assert.IsTrue(edge.From >= 0 && edge.From < m_nodes.Length, "Graph::AddEdge : 'from' node index out of range");
		Assert.IsTrue(edge.To >= 0 && edge.To < m_nodes.Length, "Graph::AddEdge : 'to' node index out of range");

		if (edge.To != GraphNode.kInvalidIndex && edge.From != GraphNode.kInvalidIndex)
		{
			GraphNode node;

			if (!EdgeExists(edge.From, edge.To))
			{
				node = m_nodes[edge.From];
				for (int i = 0; i < node.Edges.Length; i++)
				{
					if (node.Edges[i] == null)
					{
						node.Edges[i] = edge;
						break;
					}
				}
			}

			if (!EdgeExists(edge.To, edge.From))
			{
				node = m_nodes[edge.To];
				for (int i = 0; i < node.Edges.Length; i++)
				{
					if (node.Edges[i] == null)
					{
						node.Edges[i] = ReverseEdge(edge);
						break;
					}
				}
			}
		}
	}

	public void SetEdgeCost(int from, int to, float cost)
	{
		Assert.IsTrue(from >= 0 && from < m_nodes.Length, "Graph::SetEdgeCost : 'from' node index out of range");
		Assert.IsTrue(to >= 0 && to < m_nodes.Length, "Graph::SetEdgeCost : 'to' node index out of range");

		if (EdgeExists(from, to))
		{
			m_nodes[from].Edges[to].Cost = cost;
		}
	}

	public int GetNextFreeNodeIndex()
	{
		return m_nextNodeIndex;
	}

	private bool EdgeExists(int from, int to)
	{
		for (int i = 0; i < m_nodes[from].Edges.Length; i++)
		{
			if (m_nodes[from].Edges[i] != null && 
				m_nodes[from].Edges[i].To == to)
			{
				return true;
			}
		}
		return false;
	}

	private GraphEdge ReverseEdge(GraphEdge edge)
	{
		return new GraphEdge(edge.To, edge.From, edge.Cost);
	}
}
