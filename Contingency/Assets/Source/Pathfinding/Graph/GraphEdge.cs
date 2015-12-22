public class GraphEdge
{
	// Indexes of GraphNodes the edge connects
	private GraphNode m_from;
	private GraphNode m_to;
	private float m_cost;

	public GraphEdge(GraphNode from, GraphNode to, float cost)
	{
		m_from = from;
		m_to = to;
		m_cost = cost;
	}

	public GraphNode From
	{
		get { return m_from; }
		set { m_from = value; }
	}

	public GraphNode To
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
