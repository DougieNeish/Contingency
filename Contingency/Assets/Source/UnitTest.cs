using UnityEngine;
using System.Collections;

public class UnitTest : MonoBehaviour
{
	private PathfindingController m_pathfindingController;
	private const int kTestIterations = 50;

	void Awake()
    {
		m_pathfindingController = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PathfindingController>();
	}

	void Update()
	{
		if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.T))
		{
			for (int i = 0; i < kTestIterations; i++)
			{
				ExecutePathfindingTest();
			}
		}
	}

	private void ExecutePathfindingTest()
	{
		Graph navGraph = m_pathfindingController.NavGraph;
		Vector3 start = GetRandomNode(navGraph).Position;
		Vector3 target = GetRandomNode(navGraph).Position;

		Debug.Log("Searching from " + start.ToString() + " to " + target.ToString());

		m_pathfindingController.Search(start, target);
	}
	
	private GraphNode GetRandomNode(Graph graph)
	{
		return graph.Nodes[Random.Range(0, graph.Nodes.Length)];
	}
}
