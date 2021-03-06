﻿using UnityEngine;
using System.Collections.Generic;

public static class GraphUtils
{
	public static float kBaseMovementCost = 1f;
	public static float kSqrt2 = Mathf.Sqrt(2f);

	public static void CreateGrid(this Graph graph, Terrain terrain)
	{
		TerrainData terrainData = terrain.terrainData;
		float cellWidth = terrainData.size.x / graph.ColumnCount;
		float cellHeight = terrainData.size.z / graph.RowCount;
		Vector2 cellPosition = new Vector2(cellWidth / 2, cellHeight / 2);
		
		// Add nodes
		for (int row = 0; row < graph.RowCount; row++)
		{
			for (int col = 0; col < graph.ColumnCount; col++)
			{
				Vector3 nodePosition = new Vector3(cellPosition.x + (col * cellWidth), 0f, cellPosition.y + (row * cellHeight));
				float terrainHeightAtNode = terrain.SampleHeight(nodePosition);
				nodePosition.y = terrainHeightAtNode;

				graph.AddNode(new GraphNode(graph.NextNodeIndex, nodePosition));
			}
		}

		// Add edges between nodes
		for (int row = 0; row < graph.RowCount; row++)
		{
			for (int col = 0; col < graph.ColumnCount; col++)
			{
				graph.AddEdgesFromNodeToNeighbours(row, col);
			}
		}
	}

	public static void AddEdgesFromNodeToNeighbours(this Graph graph, int row, int col)
	{
		for (int rowOffset = -1; rowOffset <= 1; rowOffset++)
		{
			for (int colOffset = -1; colOffset <= 1; colOffset++)
			{
				// Skip if graph offset equals this node
				if ((rowOffset == 0) && (colOffset == 0))
				{
					continue;
				}

				int neighbourRow = row + rowOffset;
				int neighbourCol = col + colOffset;

				if (IsValidNodePosition(neighbourCol, neighbourRow, graph.ColumnCount, graph.RowCount))
				{
					int nodeIndex = GetNodeIndexFromGraphCell(row, col, graph.ColumnCount);
					int neighbourIndex = GetNodeIndexFromGraphCell(neighbourRow, neighbourCol, graph.ColumnCount);
					
					float cost = (rowOffset == 0f || colOffset == 0f) ? kBaseMovementCost : kSqrt2;

					GraphEdge.EdgeDirection direction = GetEdgeDirectionFromCellOffset(colOffset, rowOffset);
					GraphEdge.EdgeDirection reverseDirection = ReverseDirection(direction);

					// Add edges from and to both nodes
					graph.AddEdge(new GraphEdge(graph.Nodes[nodeIndex], graph.Nodes[neighbourIndex], cost, direction));
					graph.AddEdge(new GraphEdge(graph.Nodes[neighbourIndex], graph.Nodes[nodeIndex], cost, reverseDirection));
				}
			}
		}
	}

	public static int GetNodeIndexFromGraphCell(int row, int column, int columnCount)
	{
		// A position in a 2D array [x][y] is the same as [y * NumCellsX + x] in a 1D array
		return row * columnCount + column;
	}

	public static bool IsValidNodePosition(int column, int row, int columnCount, int rowCount)
	{
		return ((column > 0) && (column < columnCount) && (row > 0) && (row < rowCount));
	}

	public static void DrawGrid(this Graph graph)
	{
		const float kOffsetY = 25f;

		for (int i = 0; i < graph.Nodes.Length; i++)
		{
			GraphNode node = graph.Nodes[i];

			for (int j = 0; j < node.Edges.Length; j++)
			{
				if (node.Edges[j] == null)
				{
					break;
				}

				Vector3 fromPos = node.Position;
				fromPos.y = kOffsetY;
				Vector3 toPos = node.Edges[j].To.Position;
				toPos.y = kOffsetY;

				Debug.DrawLine(fromPos, toPos, Color.blue, 10000f);
			}
		}
	}

	public static void PrintGraph(this Graph graph)
	{
		for (int i = 0; i < graph.Nodes.Length; i++)
		{
			GraphNode node = graph.Nodes[i];
			if (node == null)
			{
				continue;
			}

			for (int j = 0; j < node.Edges.Length; j++)
			{
				if (node.Edges[j] != null)
				{
					Debug.Log("Node [" + i + "] - Edge to [" + node.Edges[j].To + "]");
				}
			}
		}
	}

	public static GraphNode GetNodeNearestToPosition(this Graph graph, Vector3 position)
	{
		// TODO: Is there something more efficient than looping through every node checking distance?
		GraphNode closestNode = null;
		float smallestDistance = float.MaxValue;
		foreach (GraphNode node in graph.Nodes)
		{
			float distance = Vector3.Distance(node.Position, position);
			if (distance < smallestDistance)
			{
				closestNode = node;
				smallestDistance = distance;
			}
		}

		return closestNode;
	}

	public static GraphEdge GetEdgeToNode(this GraphNode node1, GraphNode node2)
	{
		GraphEdge connectingEdge = null;
		foreach (GraphEdge edge in node1.Edges)
		{
			if (edge != null && edge.To.Index == node2.Index)
			{
				connectingEdge = edge;
				break;
			}
		}

		return connectingEdge;
	}

	public static void RemoveNodesBasedOnTerrainHeight(this Graph graph, Terrain terrain, float maxHeight)
	{
		for (int i = 0; i < graph.Nodes.Length; i++)
		{
			if (terrain.SampleHeight(graph.Nodes[i].Position) > maxHeight)
			{
				graph.Nodes[i].Disable();
			}
		}
	}

	public static void RemoveNodesBasedOnTerrainIncline(this Graph graph, Terrain terrain, float maxIncline)
	{
		TerrainData terrainData = terrain.terrainData;
		float cellWidth = terrainData.size.x / graph.ColumnCount;
		float cellHeight = terrainData.size.z / graph.RowCount;

		for (int i = 0; i < graph.Nodes.Length; i++)
		{
			// Calculate min/max node x and z values
			float minX = graph.Nodes[i].Position.x - cellWidth / 2;
			float maxX = graph.Nodes[i].Position.x + cellWidth / 2;
			float minZ = graph.Nodes[i].Position.z - cellHeight / 2;
			float maxZ = graph.Nodes[i].Position.z + cellHeight / 2;

			// Calculate left/right/top/bottom/centre node positions
			Vector3 centre = graph.Nodes[i].Position;
			Vector3 left = new Vector3(minX, 0f, centre.z);
			Vector3 right = new Vector3(maxX, 0f, centre.z);
			Vector3 top = new Vector3(centre.x, 0f, minZ);
			Vector3 bottom = new Vector3(centre.x, 0f, maxZ);

			// Sample height of terrain at left/right/top/bottom/centre positions
			// Add them to array for easy iteration
			float centreHeight = terrain.SampleHeight(graph.Nodes[i].Position);
			float[] edgeHeights = new float[4];
			edgeHeights[0] = terrain.SampleHeight(left);
			edgeHeights[1] = terrain.SampleHeight(right);
			edgeHeights[2] = terrain.SampleHeight(top);
			edgeHeights[3] = terrain.SampleHeight(bottom);

			// For each sampled height
			for (int j = 0; j < edgeHeights.Length; j++)
			{
				// Check if difference between left/right/top/bottom positions
				// and node centre are greater than the max incline
				if (centreHeight - edgeHeights[j] > maxIncline)
				{
					graph.Nodes[i].Disable();
					break;
				}
			}
		}
	}

	public static void RemoveNodesFromObstacles(this Graph graph, Terrain terrain)
	{
		TerrainData terrainData = terrain.terrainData;
		float cellWidth = terrainData.size.x / graph.ColumnCount;
		float cellHeight = terrainData.size.z / graph.RowCount;

		Bounds bounds = new Bounds(Vector3.zero, new Vector3(cellHeight, 1f, cellHeight));

		List<GameObject> obstacles = new List<GameObject>();
		obstacles.AddRange(GameObject.FindGameObjectsWithTag("Static/Misc"));
		obstacles.Add(GameObject.FindGameObjectWithTag("Static/HumanBuilding"));
		obstacles.Add(GameObject.FindGameObjectWithTag("Static/AIBuilding"));

		// Move a bounds object to each node, and check whether it intersects with an obstacle
		foreach (GraphNode node in graph.Nodes)
		{
			bounds.center = node.Position;
			foreach (GameObject obstacle in obstacles)
			{
				if (bounds.Intersects(obstacle.GetComponent<Collider>().bounds))
				{
					node.Disable();
				}
			}
		}
	}

	// TODO: Fix this abomination
	private static GraphEdge.EdgeDirection GetEdgeDirectionFromCellOffset(int colOffset, int rowOffset)
	{
		if (colOffset == 0)
		{
			if (rowOffset == 1)
			{
				return GraphEdge.EdgeDirection.N;
			}
			else if (rowOffset == -1)
			{
				return GraphEdge.EdgeDirection.S;
			}
		}
		else if (colOffset == 1)
		{
			if (rowOffset == 0)
			{
				return GraphEdge.EdgeDirection.E;
			}
			else if (rowOffset == 1)
			{
				return GraphEdge.EdgeDirection.NE;
			}
			else if (rowOffset == -1)
			{
				return GraphEdge.EdgeDirection.SE;
			}
		}
		else if (colOffset == -1)
		{
			if (rowOffset == 0)
			{
				return GraphEdge.EdgeDirection.W;
			}
			else if (rowOffset == 1)
			{
				return GraphEdge.EdgeDirection.NW;
			}
			else if (rowOffset == -1)
			{
				return GraphEdge.EdgeDirection.SW;
			}
		}

		// Should never get here
		return GraphEdge.EdgeDirection.Null;
	}

	private static GraphEdge.EdgeDirection ReverseDirection(GraphEdge.EdgeDirection direction)
	{
		int newDirection = (int)direction + 4;

		if (newDirection > 7)
		{
			newDirection -= 8;
		}

		return (GraphEdge.EdgeDirection)newDirection;
	}
}
