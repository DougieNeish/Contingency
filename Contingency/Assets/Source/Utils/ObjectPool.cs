using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool
{
	private List<GameObject> m_objectPool;
	private GameObject m_prefab;
	private int m_poolSize;
	private bool m_canGrow;

	public ObjectPool(GameObject prefab, int poolSize, bool canGrow)
	{
		m_objectPool = new List<GameObject>();
		m_prefab = prefab;
		m_poolSize = poolSize;
		m_canGrow = canGrow;

		for (int i = 0; i < m_poolSize; i++)
		{
			GameObject newObject = Object.Instantiate(prefab);
			newObject.SetActive(false);
			m_objectPool.Add(newObject);
		}
	}

	public GameObject getObject()
	{
		for (int i = 0; i < m_objectPool.Count; i++)
		{
			if (!m_objectPool[i].activeSelf)
			{
				m_objectPool[i].SetActive(true);
				return m_objectPool[i];
			}
		}

		if (m_canGrow)
		{
			GameObject newObject = Object.Instantiate(m_prefab);
			newObject.SetActive(true);
			m_objectPool.Add(newObject);
			return newObject;
		}

		return null;
	}
}
