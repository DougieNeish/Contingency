using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

using EdgeList = System.Collections.Generic.List<GraphEdge>;

public class Graph
{
	private List<GraphNode> m_nodes;
	private List<EdgeList> m_edges;

	private int m_nextNodeIndex;

	public Graph()
	{
		m_nextNodeIndex = 0;
	}

	public GraphNode GetNode(int index)
	{
		Assert.IsTrue(index >= 0 && index < m_nodes.Count, "Graph::GetNode : Node index out of range");
		return m_nodes[index];
	}

	public GraphEdge GetEdge(int from, int to)
	{
		Assert.IsTrue(from >= 0 && from < m_nodes.Count, "Graph::GetEdge : 'from' node index out of range");
		Assert.IsTrue(to >= 0 && to < m_nodes.Count, "Graph::GetEdge : 'to' node index out of range");

		for (int i = 0; i < m_edges.Count - 1; i++)
		{
			if (m_edges[from][i].To == to)
			{
				return m_edges[from][i];
			}
		}

		return null;
	}
	
	public int AddNode(GraphNode node)
	{
		if (node.Index < m_nodes.Count)
		{
			Assert.IsTrue(node.Index != GraphNode.kInvalidIndex, "Graph::AddNode : Trying to add node to index with existing node");

			m_nodes[node.Index] = node;
			return m_nextNodeIndex;
		}
		else
		{
			Assert.IsTrue(node.Index == m_nextNodeIndex, "Graph::AddNode : Invalid node index");

			m_nodes.Add(node);
			m_edges[node.Index] = new EdgeList();
			return m_nextNodeIndex++;
		}
	}

	public void AddEdge(GraphEdge edge)
	{
		Assert.IsTrue(edge.From >= 0 && edge.From < m_nodes.Count, "Graph::AddEdge : 'from' node index out of range");
		Assert.IsTrue(edge.To >= 0 && edge.To < m_nodes.Count, "Graph::AddEdge : 'to' node index out of range");

		if (edge.To != GraphNode.kInvalidIndex && edge.From != GraphNode.kInvalidIndex)
		{
			if (!EdgeExists(edge.From, edge.To))
			{
				m_edges[edge.From].Add(edge);
			}

			if (!EdgeExists(edge.To, edge.From))
			{
				m_edges[edge.To].Add(new GraphEdge(edge.To, edge.From, edge.Cost));
			}
		}
	}

	public void SetEdgeCost(int from, int to, float cost)
	{
		Assert.IsTrue(from >= 0 && from < m_nodes.Count, "Graph::SetEdgeCost : 'from' node index out of range");
		Assert.IsTrue(to >= 0 && to < m_nodes.Count, "Graph::SetEdgeCost : 'to' node index out of range");

		if (EdgeExists(from, to))
		{
			m_edges[from][to].Cost = cost;
		}
	}

	public bool IsEmpty()
	{
		return m_nodes.Count < 1;
	}

	public int GetNextFreeNodeIndex()
	{
		return m_nextNodeIndex;
	}

	private bool EdgeExists(int from, int to)
	{
		for (int i = 0; i < m_edges[from].Count; i++)
		{
			if (m_edges[from][i].To == to)
			{
				return false;
			}
		}

		return true;
	}
}
