﻿using UnityEngine;
using System.Collections.Generic;

public class PathfindingController : MonoBehaviour
{
	[SerializeField] private Terrain m_terrain;
	[SerializeField] private int m_numCellsX;
	[SerializeField] private int m_numCellsY;

	[SerializeField] private bool m_drawNodes;
	[SerializeField] private bool m_drawEdges;

	private int m_numCells;
	private Graph m_navGraph;

	// Should change based on number of grid cells
	private const float kNavGraphMaxIncline = 0.6f;

	void Awake()
	{
		m_numCells = m_numCellsX * m_numCellsY;
		m_navGraph = new Graph(m_numCells);
	}

	void Start()
	{
		Vector3 terrainSize = m_terrain.terrainData.size;
		m_navGraph.CreateGrid(m_terrain, m_numCellsX, m_numCellsY);
		m_navGraph.RemoveNodesBasedOnTerrainIncline(m_terrain, kNavGraphMaxIncline, m_numCellsX, m_numCellsY);
		// TODO: Remove groups of nodes isolated from the rest of the nav graph
	}

	void OnDrawGizmos()
	{
		if (!Application.isPlaying)
		{
			return;
		}

		if (!m_drawNodes && !m_drawEdges)
		{
			return;
		}

		GraphNode node;
		for (int i = 0; i < m_navGraph.Nodes.Length; i++)
		{
			if (!m_navGraph.Nodes[i].Enabled)
			{
				continue;
			}

			float markerSize = 2.5f;
			if (m_drawNodes)
			{
				Gizmos.color = Color.white;
				Gizmos.DrawWireCube(m_navGraph.Nodes[i].Position, new Vector3(markerSize, 0f, markerSize));
			}

			if (!m_drawEdges)
			{
				continue;
			}

			node = m_navGraph.Nodes[i];
			for (int j = 0; j < node.Edges.Length; j++)
			{
				if (node.Edges[j] == null)
				{
					break;
				}

				Vector3 fromPos = node.Position;
				Vector3 toPos = m_navGraph.Nodes[node.Edges[j].To].Position;

				Gizmos.color = Color.white;
				Gizmos.DrawLine(fromPos, toPos);
			}
		}
	}
}
