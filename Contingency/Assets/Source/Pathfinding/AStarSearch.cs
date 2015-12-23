using UnityEngine;
using System;
using System.Collections.Generic;

public class AStarSearch
{
	private int m_nodeCount;
	private HashSet<GraphNode> m_openList;
	private HashSet<GraphNode> m_closedList;
	private float[] m_runningCost;

	public AStarSearch(int nodeCount)
	{
		m_nodeCount = nodeCount;
		m_openList = new HashSet<GraphNode>();
		m_closedList = new HashSet<GraphNode>();
		m_runningCost = new float[nodeCount];
	}

	public GraphNode[] Search(Graph graph, GraphNode startNode, GraphNode targetNode)
	{
		Profiler.BeginSample("A* Search");

		for (int i = 0; i < m_nodeCount; i++)
		{
			//m_openList[i] = GraphNode.kInvalidIndex;
			//m_closedList[i] = GraphNode.kInvalidIndex;
			m_runningCost[i] = 0f;
		}

		// Move into above for loop if m_nodeCount == graph.Nodes.Length (which it should)?
		// Replace with data structure in A star class instead of nodes to allow concurrent searches
		for (int i = 0; i < graph.Nodes.Length; i++)
		{
			graph.Nodes[i].Parent = null;
		}

		m_openList.Clear();
		m_closedList.Clear();

		// Add start node to running cost
		//m_runningCost.Add(startNode, 0f);
		m_runningCost[startNode.Index] = 0f; // not required due to above for loop init?

		// Add target node to the open list
		//m_openList[0] = startNode;
		m_openList.Add(startNode);

		GraphNode currentNode;
		int lastOpenListIndex = 0;
		int lastClosedListIndex = 0;

		while (m_openList.Count > 0)
		{
			Profiler.BeginSample("Find lowest cost in open list");
			GraphNode bestNode = null;
			float lowestCost = float.MaxValue;
			foreach (GraphNode tempNode in m_openList)
			{
				if (m_runningCost[tempNode.Index] < lowestCost)
				{
					lowestCost = m_runningCost[tempNode.Index];
					bestNode = tempNode;
				}
			}
			Profiler.EndSample();

			currentNode = bestNode;
			m_openList.Remove(bestNode);
			
			// If current node equals target node end search
			if (currentNode.Index == targetNode.Index)
			{
				break;
			}

			// Add current node to the first empty slot in the closed list
			//for (int i = 0; i < m_closedList.Length; i++)
			//{
			//	if (m_closedList[i] == null)
			//	{
			//		m_closedList[i] = currentNode;
			//	}
			//}
			m_closedList.Add(currentNode);

			GraphNode nextNode;
			float cost;
			// Loop through all neighbours of current node
			Profiler.BeginSample("Loop through neighbours");
			foreach (GraphEdge edge in currentNode.Edges)
			{
				if (edge == null || edge.To.Index == GraphNode.kInvalidIndex)
				{
					continue;
				}

				nextNode = edge.To;
				cost = m_runningCost[currentNode.Index] + edge.Cost;

				// If in open list and cost is less than running cost to neighbour, remove from open list because new path is better
				//if (m_openList[currentNode.Index] != null && cost < m_runningCost[nextNode.Index])
				Profiler.BeginSample("Check open list contains node");
				if (m_openList.Contains(currentNode) && cost < m_runningCost[nextNode.Index])
				{
					Profiler.BeginSample("Remove node from open list");
					m_openList.Remove(currentNode);
					Profiler.EndSample();
				}
				Profiler.EndSample();

				// If in closed list and cost is less than running cost to neighbour, remove from closed list because new path is better
				//if (m_closedList[currentNode.Index] != null && cost < m_runningCost[nextNode.Index])
				if (m_closedList.Contains(currentNode) && cost < m_runningCost[nextNode.Index])
				{
					m_closedList.Remove(currentNode);
				}

				// If not in open or closed list
				Profiler.BeginSample("Not in open or closed list");
				if (!m_closedList.Contains(nextNode) && !m_openList.Contains(nextNode) && (m_runningCost[nextNode.Index] == 0f || cost < m_runningCost[nextNode.Index]))
				{
					// f = g + h
					m_runningCost[nextNode.Index] = cost + DiagonalDistance(nextNode.Position, targetNode.Position);
					Profiler.BeginSample("Add node to open list");
					m_openList.Add(nextNode);
					Profiler.EndSample();
					nextNode.Parent = currentNode;
				}
				Profiler.EndSample();

				//Profiler.BeginSample("Not in closed list");
				//bool inClosedList = m_closedList.Contains(nextNode);
				//Profiler.EndSample();

				//Profiler.BeginSample("Not in open list");
				//bool inOpenList = m_openList.Contains(nextNode);
				//Profiler.EndSample();

				//Profiler.BeginSample("running cost = 0");
				//bool costIsZero = m_runningCost[nextNode.Index] == 0f;
				//Profiler.EndSample();

				//Profiler.BeginSample("running cost < previous");
				//bool costLessThanPrevious = cost < m_runningCost[nextNode.Index];
				//Profiler.EndSample();

				//if (!inClosedList && !inOpenList && (costIsZero || costLessThanPrevious))
				//{

				//	m_runningCost[nextNode.Index] = cost; // + HEURISTIC
				//	Profiler.BeginSample("Add node to open list");
				//	m_openList.Add(nextNode);
				//	Profiler.EndSample();
				//	nextNode.Parent = currentNode;
				//}
			}
			Profiler.EndSample();
		}

		// Retrace parents to create list from target to start node
		GraphNode node = targetNode;
		List<GraphNode> pathFromTarget = new List<GraphNode>();
		
		while (node != startNode) // Add null check?
		{
			pathFromTarget.Add(node);
			node = node.Parent;
		}

		// Reverse path so it goes from start node to target
		pathFromTarget.Reverse();

		Profiler.EndSample();

		return pathFromTarget.ToArray();
		//return new GraphNode[0];
	}

	public GraphNode[] Search(Graph graph, Vector3 startPosition, Vector3 targetPosition)
	{
		GraphNode startNode = graph.GetNodeNearestToPosition(startPosition);
		GraphNode targetNode = graph.GetNodeNearestToPosition(targetPosition);
		return Search(graph, startNode, targetNode);
	}

	private float DiagonalDistance(Vector3 nodePosition, Vector3 targetPosition)
	{
		float dx = Mathf.Abs(nodePosition.x - targetPosition.x);
		float dz = Mathf.Abs(nodePosition.z - targetPosition.z);
		return GraphUtils.baseMovementCost * (dx + dz) + (GraphUtils.sqrt2 - 2 * GraphUtils.baseMovementCost) * Mathf.Min(dx, dz);
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

	private GraphNode GetFirstValidItem(GraphNode[] array)
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
