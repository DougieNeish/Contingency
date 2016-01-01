public class GraphEdge
{
	public enum EdgeDirection
	{
		N,
		NE,
		E,
		SE,
		S,
		SW,
		W,
		NW,
		Null,
	}

	private GraphNode m_from;
	private GraphNode m_to;
	private float m_cost;
	private EdgeDirection m_direction;

	public GraphEdge(GraphNode from, GraphNode to, float cost, EdgeDirection direction)
	{
		m_from = from;
		m_to = to;
		m_cost = cost;
		m_direction = direction;
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

	public EdgeDirection Direction
	{
		get { return m_direction; }
		set { m_direction = value; }
	}
}
