using UnityEngine;

public static class Formations
{
	public enum FormationType
	{
		Square,
		Wedge,
	}

	private static int[] m_squareNumbers = new int[] {0, 1, 4, 9, 16, 25, 36, 49, 64, 81 };

	public static Vector3[] GetFormationPositions(FormationType type, Vector3 leaderPosition, int formationSize)
	{
		switch (type)
		{
			case FormationType.Wedge:
				break;

			default:
				break;
		}

		return new Vector3[0];
	}

	public static Vector3[] CalculateSquareFormation(int formationSize,	float verticalSeparation, float horizontalSeparation)
	{
		UnityEngine.Assertions.Assert.IsTrue(formationSize <= m_squareNumbers[m_squareNumbers.Length - 1], "Formation size greater than largest stored square number");

		int closestSqrNumber = 0;
		int sqrRoot = 0;
		// Find closest square number greater than formation size
		for (int i = 0; i < m_squareNumbers.Length; i++)
		{
			if (m_squareNumbers[i] >= formationSize)
			{
				closestSqrNumber = m_squareNumbers[i];
				sqrRoot = i;
				break;
			}
		}

		int arrayIndex = 0;
		Vector3[] leaderOffset = new Vector3[closestSqrNumber];

		float x, y, z;

		for (int i = 0; i < sqrRoot; i++)
		{
			for (int j = 0; j < sqrRoot; j++)
			{
				x = horizontalSeparation * i;
				y = 0f;
				z = verticalSeparation * j;
				leaderOffset[arrayIndex++] = new Vector3(x, y, z);
			}
		}

		return leaderOffset;
	}

	//private static Vector3[] CalculateWedgeFormation(Vector3 leaderPosition, int formationSize,
	//	float verticalSeparation, float horizontalSeparation)
	//{
	//	Vector3[] positions = new Vector3[formationSize];
	//	positions[0] = leaderPosition;

	//	// Start from 1 as 0 is the leader
	//	for (int i = 1; i < formationSize; i++)
	//	{
	//		float x = leaderPosition.x + (horizontalSeparation ;
	//		float y = leaderPosition.y;
	//		float z = leaderPosition.z + verticalSeparation;

	//		positions[i] = new Vector3(x, y, z);
	//	}

	//	return positions;
	//}
}
