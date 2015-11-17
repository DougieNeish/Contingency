using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{
	//public float MaxSpeed;
	//public float MaxForce;

	void Start()
	{
	
	}
	
	void Update()
	{
	
	}

	public static explicit operator GameObject(Unit unit)
	{
		return unit.gameObject;
	}
}
