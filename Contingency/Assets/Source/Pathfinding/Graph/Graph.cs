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
		Assert.IsTrue(edge.From.Index >= 0 && edge.From.Index < m_nodes.Length, "Graph::AddEdge : 'from' node index out of range");
		Assert.IsTrue(edge.To.Index >= 0 && edge.To.Index < m_nodes.Length, "Graph::AddEdge : 'to' node index out of range");

		if (edge.To.Index != GraphNode.kInvalidIndex && edge.From.Index != GraphNode.kInvalidIndex)
		{
			GraphNode node;

			if (!EdgeExists(edge.From, edge.To))
			{
				node = edge.From;
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
				node = edge.To;
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

	public int GetNextFreeNodeIndex()
	{
		return m_nextNodeIndex;
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

	private GraphEdge ReverseEdge(GraphEdge edge)
	{
		return new GraphEdge(edge.To, edge.From, edge.Cost);
	}
}
