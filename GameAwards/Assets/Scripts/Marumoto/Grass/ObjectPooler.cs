using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour {
	[SerializeField]
	private GameObject _pooledTargetObject;
	[SerializeField]
	Transform _parent;
	[SerializeField]
	GrassData _grassData;

	private int _poolingBlockNum;
	private int _oneChunkTipNum;
	List<List<GrassObject>> _pooledObjects = new List<List<GrassObject>>();

	void Awake ()
	{
		_poolingBlockNum = _grassData.ChunkDepth * _grassData.ChunkWidth;
		_oneChunkTipNum = _grassData.OneLinePerChunkTipNum * _grassData.OneLinePerChunkTipNum;
		CreateGrassChunks();
	}

	private void CreateGrassChunks()
	{
		for (int i = 0; i < _poolingBlockNum; i++)
		{
			List<GrassObject> _chunk = new List<GrassObject>();
			var _emptyObj = new GameObject();
			_emptyObj.transform.SetParent(_parent);

			for (int j = 0; j < _oneChunkTipNum; j++)
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
