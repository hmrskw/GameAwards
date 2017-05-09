using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour {
	[SerializeField]
	private GameObject _pooledTargetObject;
	[SerializeField]
	private int _poolingBlockNum;
	[SerializeField]
	Transform _parent;


	List<List<GameObject>> _pooledObjects = new List<List<GameObject>>();

	void Awake ()
	{
		CreateGrassChunks();
	}

	private void CreateGrassChunks()
	{
		for (int i = 0; i < _poolingBlockNum; i++)
		{
			List<GameObject> _chunk = new List<GameObject>();

			for (int j = 0; j < 25; j++)
			{
				var _refObj = Instantiate(_pooledTargetObject, _parent);
				_chunk.Add(_refObj);
			}

			_pooledObjects.Add(_chunk);
		}
	}

	public List<List<GameObject>> GetObjects()
	{
		return _pooledObjects;
	}
}
