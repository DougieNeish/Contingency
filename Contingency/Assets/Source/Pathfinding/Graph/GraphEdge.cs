﻿public class GraphEdge
{
	// Indexes of GraphNodes the edge connects
	protected int m_from;
	protected int m_to;

	protected float m_cost;

	public GraphEdge(int from, int to, float cost)
	{
		m_from = from;
		m_to = to;
		m_cost = cost;
	}

	public int From
	{
		get { return m_from; }
		set { m_from = value; }
	}

	public int To
	{
		get { return m_to; }
		set { m_to = value; }
	}

	public float Cost
	{
		get { return m_cost; }
		set { m_cost = value; }
	}
}