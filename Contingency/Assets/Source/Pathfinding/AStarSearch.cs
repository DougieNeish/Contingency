using UnityEngine;
using System;
using System.Collections.Generic;

public class AStarSearch
{
	//private GraphNode[] m_openList;
	//private GraphNode[] m_closedList;
	//private Dictionary<GraphNode, float> m_runningCost;

	private int m_nodeCount;
	//private int[] m_openList;
	//private int[] m_closedList;
	private GraphEdge[] m_openList;
	private GraphEdge[] m_closedList;
	private float[] m_runningCost;

	public AStarSearch(int nodeCount)
	{
		m_nodeCount = nodeCount;
		m_openList = new GraphEdge[nodeCount];
		m_closedList = new GraphEdge[nodeCount];
		m_runningCost = new float[nodeCount];
	}

	public GraphNode[] Search(Graph graph, int startNode, int targetNode) //GraphNode startNode, GraphNode targetNode)
	{
		for (int i = 0; i < m_nodeCount; i++)
		{
			m_openList[i] = null;//GraphNode.kInvalidIndex;
			m_closedList[i] = null;//GraphNode.kInvalidIndex;
			m_runningCost[i] = 0f;
		}

		//Array.Clear(m_openList, 0, m_openList.Length);
		//Array.Clear(m_closedList, 0, m_closedList.Length);
		//m_runningCost.Clear();

		// Add start node to running cost
		//m_runningCost.Add(startNode, 0f);

		// Add target node to the open list
		//m_openList[0] = targetNode;

		GraphEdge currentEdge;
		while (m_openList.Length > 0)
		{
			currentEdge = GetFirstValidItem(m_openList);

			if (currentEdge.To == targetNode)
			{
				break;
			}

			// Add current node to the first empty slot in the closed list
			for (int i = 0; i < m_closedList.Length; i++)
			{
				if (m_closedList[i] == null)
				{
					m_closedList[i] = currentEdge;
				}
			}

			int nextNode;
			float cost;
			// Loop through all neighbours of current node
			foreach (GraphEdge edge in graph.GetNodeFromIndex(currentEdge.To).Edges)
			{
				//nextNode = graph.GetNodeFromIndex(edge.To);
				//cost = m_runningCost[currentEdge] + edge.Cost;
				
				//if (m_openList[currentEdge] != GraphNode.kInvalidIndex && cost < m_runningCost[nextNode])
				//{
				//	m_openList[currentEdge] == GraphNode.kInvalidIndex
				//}


				//if (!m_runningCost.ContainsKey(nextNode) || cost < m_runningCost[nextNode])
				//if (!NodeArrayContains(m_closedList, nextNode) || cost < m_closedList[nextNode])
				//{
				//	m_runningCost[nextNode]
				//}
			}

		}

		return new GraphNode[0];
	}

	private bool NodeArrayContains(GraphNode[] nodeArray, GraphNode node)
	{
		for (int i = 0; i < nodeArray.Length; i++)
		{
			if (nodeArray[i] == node)
			{
				return true;
			}
		}

		return false;
	}

	//private GraphNode GetLowestCost(GraphNode[] nodeArray)
	//{
	//	float lowestCost = float.MaxValue;
	//	for (int i = 0; i < nodeArray.Length; i++)
	//	{
	//		if (nodeArray[i].)
	//	}

	//	return lowestCost;
	//}

	private GraphEdge GetFirstValidItem(GraphEdge[] array)
	{
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				return array[i];
			}
		}

		return null;
	}
}
