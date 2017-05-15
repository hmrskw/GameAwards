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


	List<List<GrassObject>> _pooledObjects = new List<List<GrassObject>>();

	void Awake ()
	{
		CreateGrassChunks();
	}

	private void CreateGrassChunks()
	{
		for (int i = 0; i < _poolingBlockNum; i++)
		{
			List<GrassObject> _chunk = new List<GrassObject>();
			var _emptyObj = new GameObject();
			_emptyObj.transform.SetParent(_parent);

			for (int j = 0; j < 25; j++)
			{
				var _refObj = Instantiate(_pooledTargetObject, _emptyObj.transform);
				_chunk.Add(new GrassObject(_refObj, _refObj.GetComponentInChildren<GrassesController>()));
			}

			_pooledObjects.Add(_chunk);
		}
	}

	public List<List<GrassObject>> GetObjects()
	{
		return _pooledObjects;
	}
}
