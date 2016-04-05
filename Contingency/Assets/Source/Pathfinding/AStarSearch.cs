using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

public class AStarSearch
{
	private int m_nodeCount;
	private HashSet<GraphNode> m_openList;
	private HashSet<GraphNode> m_closedList;
	private float[] m_runningCost;

	// Waypoints are cached and stored separately so only one iteration is required
	private List<GraphNode> m_nodePath;
	private List<Vector3> m_waypoints;

	private bool m_simplifyPath;

	public AStarSearch(int nodeCount)
	{
		// Storing node count here could cause issues if number of nodes changes
		// during the game, depending on how long the AStarSearch object lasts
		m_nodeCount = nodeCount;
		m_openList = new HashSet<GraphNode>();
		m_closedList = new HashSet<GraphNode>();
		m_runningCost = new float[nodeCount];

		m_nodePath = new List<GraphNode>();
		m_waypoints = new List<Vector3>();

		m_simplifyPath = false;
	}

	public GraphNode[] Search(Graph graph, GraphNode startNode, GraphNode targetNode)
	{
		for (int i = 0; i < m_nodeCount; i++)
		{
			m_runningCost[i] = 0f;
		}

		// TODO: Move into above for loop if m_nodeCount == graph.Nodes.Length (which it should)?
		// Replace parenting with data structure in A star class instead stored locally to nodes to allow concurrent searches?
		for (int i = 0; i < graph.Nodes.Length; i++)
		{
			graph.Nodes[i].Parent = null;
		}

		m_openList.Clear();
		m_closedList.Clear();

		// Add start node to running cost
		m_runningCost[startNode.Index] = 0f; // not required due to above for loop init?

		// Add start node to the open list
		m_openList.Add(startNode);

		while (m_openList.Count > 0)
		{
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

			GraphNode currentNode = bestNode;
			m_openList.Remove(bestNode);
			
			// If current node equals target node end search
			if (currentNode.Index == targetNode.Index)
			{
				//Debug.Log("<color=green>Target node found</color>");
				return GetPathFromParents(startNode, targetNode);
			}

			m_closedList.Add(currentNode);

			// Loop through all neighbours of current node
			foreach (GraphEdge edge in currentNode.Edges)
			{
				if (edge == null || edge.To.Index == GraphNode.kInvalidIndex)
				{
					continue;
				}

				GraphNode nextNode = edge.To;
				if (nextNode == null || !nextNode.Enabled)
				{
					continue;
				}

				float cost = m_runningCost[currentNode.Index] + edge.Cost;

				if (m_closedList.Contains(nextNode) && m_runningCost[currentNode.Index] >= m_runningCost[nextNode.Index])
				{
					continue;
				}

				if (!m_openList.Contains(nextNode) || m_runningCost[currentNode.Index] < m_runningCost[nextNode.Index])
				{
					// f = g + h
					m_runningCost[nextNode.Index] = cost + DiagonalDistance(nextNode.Position, targetNode.Position);
					m_openList.Add(nextNode);
					nextNode.Parent = currentNode;
				}
			}
		}

		Debug.Log("<color=red>Target node not found</color>");

		// Clear return lists as search did not find valid path
		m_nodePath.Clear();
		m_waypoints.Clear();
		return null;
	}

	public Vector3[] Search(Graph graph, Vector3 startPosition, Vector3 targetPosition)
	{
		GraphNode startNode = graph.GetNodeNearestToPosition(startPosition);
		GraphNode targetNode = graph.GetNodeNearestToPosition(targetPosition);

		Search(graph, startNode, targetNode);
		return m_waypoints.ToArray();
	}

	private float DiagonalDistance(Vector3 nodePosition, Vector3 targetPosition)
	{
		float dx = Mathf.Abs(nodePosition.x - targetPosition.x);
		float dz = Mathf.Abs(nodePosition.z - targetPosition.z);
		return GraphUtils.kBaseMovementCost * (dx + dz) + (GraphUtils.kSqrt2 - 2 * GraphUtils.kBaseMovementCost) * Mathf.Min(dx, dz);
	}

	private GraphNode[] GetPathFromParents(GraphNode startNode, GraphNode targetNode)
	{
		m_nodePath.Clear();
		m_waypoints.Clear();

		GraphNode currentNode = targetNode;
		GraphEdge.EdgeDirection previousDirection = GraphEdge.EdgeDirection.Null;

		// Retrace parents to create list from target to start node
		while (currentNode != startNode)
		{
			if (m_simplifyPath)
			{
				GraphEdge connectingEdge = currentNode.GetEdgeToNode(currentNode.Parent);
				if (connectingEdge.Direction != previousDirection)
				{
					m_nodePath.Add(currentNode);
					m_waypoints.Add(currentNode.Position);

					previousDirection = connectingEdge.Direction;
				}
			}
			else
			{
				m_nodePath.Add(currentNode);
				m_waypoints.Add(currentNode.Position);
			}

			currentNode = currentNode.Parent;
			Assert.IsNotNull(currentNode, "GetPathFromParents: Parent node is null");
		}

		// Reverse path so it goes from start node to target
		m_nodePath.Reverse();
		m_waypoints.Reverse();

		return m_nodePath.ToArray();
	}
}
