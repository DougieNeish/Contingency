using UnityEngine;

public static class GraphUtils
{
	public static float baseMovementCost = 1f;
	public static float sqrt2 = Mathf.Sqrt(2);

	public static void CreateGrid(this Graph graph, Terrain terrain, int numCellsX, int numCellsY)
	{
		TerrainData terrainData = terrain.terrainData;
		float cellWidth = terrainData.size.x / numCellsX;
		float cellHeight = terrainData.size.z / numCellsY;
		float cellXPosition = cellWidth / 2;
		float cellYPosition = cellHeight / 2;

		Vector3 nodePosition = Vector3.zero;
		float terrainHeightAtNode = 0f;
		
		// Add nodes
		for (int row = 0; row < numCellsY; row++)
		{
			for (int col = 0; col < numCellsX; col++)
			{
				nodePosition = new Vector3(cellXPosition + (col * cellWidth), 0f, cellYPosition + (row * cellHeight));
				terrainHeightAtNode = terrain.SampleHeight(nodePosition);
				nodePosition.y = terrainHeightAtNode;

				graph.AddNode(new GraphNode(graph.GetNextFreeNodeIndex(), nodePosition));
			}
		}

		// Add edges between nodes
		for (int row = 0; row < numCellsY; row++)
		{
			for (int col = 0; col < numCellsX; col++)
			{
				graph.AddEdgesFromNodeToNeighbours(row, col, numCellsX, numCellsY);
			}
		}
	}

	public static void AddEdgesFromNodeToNeighbours(this Graph graph, int row, int col, int numCellsX, int numCellsY)
	{
		int neighbourCol;
		int neighbourRow;

		for (int rowOffset = -1; rowOffset <= 1; rowOffset++)
		{
			for (int colOffset = -1; colOffset <= 1; colOffset++)
			{
				// Skip if graph offset equals this node
				if ((rowOffset == 0) && (colOffset == 0))
				{
					continue;
				}

				neighbourRow = row + rowOffset;
				neighbourCol = col + colOffset;

				if (IsValidNodePosition(neighbourCol, neighbourRow, numCellsX, numCellsY))
				{
					int nodeIndex = GetNodeIndexFromGraphCells(row, col, numCellsX);
					int neighbourIndex = GetNodeIndexFromGraphCells(neighbourRow, neighbourCol, numCellsX);

					Vector3 nodePosition = graph.Nodes[nodeIndex].Position;
					Vector3 neighbourPosition = graph.Nodes[neighbourIndex].Position;
					
					float cost = (rowOffset == 0f || colOffset == 0f) ? baseMovementCost : sqrt2;

					GraphEdge.EdgeDirection direction = GetEdgeDirectionFromCellOffset(colOffset, rowOffset);
					GraphEdge.EdgeDirection reverseDirection = ReverseDirection(direction);

					// Add edges from and to both nodes
					graph.AddEdge(new GraphEdge(graph.Nodes[nodeIndex], graph.Nodes[neighbourIndex], cost, direction));
					graph.AddEdge(new GraphEdge(graph.Nodes[neighbourIndex], graph.Nodes[nodeIndex], cost, reverseDirection));
				}
			}
		}
	}

	public static int GetNodeIndexFromGraphCells(int row, int col, int numCellsX)
	{
		// A position in a 2D array [x][y] is the same as [y * NumCellsX + x] in a 1D array
		return row * numCellsX + col;
	}

	public static bool IsValidNodePosition(int xPos, int yPos, int numCellsX, int numCellsY)
	{
		return ((xPos > 0) && (xPos < numCellsX) && (yPos > 0) && (yPos < numCellsY));
	}

	public static void DrawGrid(this Graph graph)
	{
		GraphNode node;
		for  (int i = 0; i < graph.Nodes.Length; i++)
		{
			node = graph.Nodes[i];

			for (int j = 0; j < node.Edges.Length; j++)
			{
				if (node.Edges[j] == null)
				{
					break;
				}

				Vector3 fromPos = node.Position;
				fromPos.y = 25f;
				Vector3 toPos = node.Edges[j].To.Position;
				toPos.y = 25f;

				Debug.DrawLine(fromPos, toPos, Color.blue, 10000f);
			}
		}
	}

	public static void PrintGraph(this Graph graph)
	{
		GraphNode node;
		for (int i = 0; i < graph.Nodes.Length; i++)
		{
			node = graph.Nodes[i];

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
			if (Vector3.Distance(node.Position, position) < smallestDistance)
			{
				closestNode = node;
				smallestDistance = Vector3.Distance(node.Position, position);
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

	public static void RemoveNodesBasedOnTerrainIncline(this Graph graph, Terrain terrain, float maxIncline, int numCellsX, int numCellsY)
	{
		TerrainData terrainData = terrain.terrainData;
		float cellWidth = terrainData.size.x / numCellsX;
		float cellHeight = terrainData.size.z / numCellsY;
		float cellXPosition = cellWidth / 2;
		float cellYPosition = cellHeight / 2;

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

	// TODO: Fix this abomination
	private static GraphEdge.EdgeDirection GetEdgeDirectionFromCellOffset(int colOffset, int rowOffset)
	{
		switch (colOffset)
		{
			case 0:
				{
					if (rowOffset == 1)
					{
						return GraphEdge.EdgeDirection.N;
					}
					else if (rowOffset == -1)
					{
						return GraphEdge.EdgeDirection.S;
					}

					break;
				}

			case 1:
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

					break;
				}

			case -1:
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

					break;
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
