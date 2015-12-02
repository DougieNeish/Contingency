public class GraphNode
{
	protected int m_index;
	public const int kInvalidIndex = -1;

	public GraphNode(int index)
	{
		m_index = index;
	}

	public int Index
	{
		get { return m_index; }
		set { m_index = value; }
	}
}
