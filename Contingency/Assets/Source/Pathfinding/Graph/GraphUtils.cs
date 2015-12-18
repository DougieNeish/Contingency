using UnityEngine;
using System.Collections.Generic;

public static class GraphUtils
{
	public static void CreateGrid(this Graph graph, float terrainWidth, float terrainHeight, int numCellsX, int numCellsY)
	{
		float cellWidth = terrainHeight / numCellsX;
		float cellHeight = terrainWidth / numCellsY;
		float cellXPosition = cellWidth / 2;
		float cellYPosition = cellHeight / 2;

		Vector3 nodePosition = Vector3.zero;

		for (int row = 0; row < numCellsY; row++)
		{
			for (int col = 0; col < numCellsX; col++)
			{
				nodePosition = new Vector3(cellXPosition + (col * cellWidth), 0f, cellYPosition + (row * cellHeight));
				graph.AddNode(new GraphNode(graph.GetNextFreeNodeIndex(), nodePosition));
			}
		}

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
					// Do something other than distance?
					float cost = Vector3.Distance(nodePosition, neighbourPosition);

					// Add edges from and to both nodes
					graph.AddEdge(new GraphEdge(nodeIndex, neighbourIndex, cost));
					graph.AddEdge(new GraphEdge(neighbourIndex, nodeIndex, cost));
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
				Vector3 toPos = graph.Nodes[node.Edges[j].To].Position;
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

	public static GraphNode[] GetNeighbours(this Graph graph, GraphNode node)
	{
		GraphNode[] neighbours = new GraphNode[node.Edges.Length];
		for (int i = 0; i < node.Edges.Length; i++)
		{
			neighbours[i] = graph.Nodes[node.Edges[i].To];
		}
		return neighbours;
	}

	public static GraphNode GetNodeFromIndex(this Graph graph, int index)
	{
		return graph.Nodes[index];
	}
}
