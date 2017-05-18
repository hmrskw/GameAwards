using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum MoveDirection
{
	Right   =  1,
	Left    = -1,
	Forward =  1,
	Back    = -1
}

public class GrassManager : MonoBehaviour {
	[SerializeField]
	ObjectPooler _pooler;
	[SerializeField]
	Transform _player;
    [SerializeField]
    LayerMask _lm;
	[SerializeField]
	GrassData _grassData;

	private int _halfWidth;
	private int _halfDepth;

	private Vector3 _playerLocation = new Vector3();
	private Vector3 _oldLocation = new Vector3();
	private Vector3 _movedChunkDirection = new Vector3();

	//intキャストでのindexズレ防止のため変数を4つ用意。
	private int _startIndexX = 0;
	private int _startIndexZ = 0;
	private int _endIndexX = 0;
	private int _endIndexZ = 0;

	private int _tipSize;
	private int _oneLinePerChunkTipNum;
	private int _oneChunkTipNum;
	private int _oneChunkSize;
	private int _dummyPointElements;
	private int _maptipIndicesElements;

	private GrassDummyPoint[,] _maptipsDummyPoint;
	private Vector2[,,] _maptipsIndices;

	List<List<GrassObject>> _pooledObjects = new List<List<GrassObject>>();
	List<List<int>> _chunkIndices = new List<List<int>>();

	/// <summary>
	/// 渡された第一引数のPosition値から、対応するDummyPointのXとZのインデックスを代入する。
	/// </summary>
	/// <param name="_position">ポジション</param>
	/// <param name="_xIndex">Xのインデックスを受け取りたい変数</param>
	/// <param name="_zIndex">Zのインデックスを受け取りたい変数</param>
	public void SearchDummyPointIndex(Vector3 _position, out int _xIndex, out int _zIndex)
	{
		_xIndex = new int();
		_zIndex = new int();

		_xIndex = (int)_position.x / _tipSize;
		_zIndex = (int)_position.z / _tipSize;
	}

	/// <summary>
	/// DummyPointのBool値HasGrownを変更。
	/// </summary>
	/// <param name="_xIndex">ダミーポイントのXインデックス</param>
	/// <param name="_zIndex">ダミーポイントのZインデックス</param>
	/// <param name="_cond">生えていることにしたいならtrue、生えてないならfalse</param>
	public void ChangeHasGrown(int _xIndex, int _zIndex, bool _cond)
	{
		_maptipsDummyPoint[_zIndex, _xIndex].SetHasGrown(_cond);
	}

	private void Start ()
	{
		_tipSize = _grassData.TipSize;
		_oneLinePerChunkTipNum = _grassData.OneLinePerChunkTipNum;
		_oneChunkTipNum = _oneLinePerChunkTipNum * _oneLinePerChunkTipNum;
		_oneChunkSize = _tipSize * _oneLinePerChunkTipNum;
		_dummyPointElements = _grassData.TerrainSize / _tipSize;
		_maptipIndicesElements = _grassData.TerrainSize / _oneChunkSize;

		_maptipsDummyPoint = new GrassDummyPoint[_dummyPointElements, _dummyPointElements];
		_maptipsIndices = new Vector2[_maptipIndicesElements, _maptipIndicesElements, _oneChunkTipNum];

		if (_grassData.ChunkWidth % 2 == 1) _halfWidth = (_grassData.ChunkWidth - 1) / 2;
		else _halfWidth = _grassData.ChunkWidth / 2;
		if (_grassData.ChunkDepth % 2 == 1) _halfDepth = (_grassData.ChunkDepth - 1) / 2;
		else _halfDepth = _grassData.ChunkDepth / 2;

		_pooledObjects = _pooler.GetObjects();
		SetupDummyPoint();
		StartCoroutine(Search());
		SetIndex();
		InitGrasses();
	}
	
	private void Update ()
	{
		if (_oldLocation != _playerLocation)
		{
			_oldLocation = _playerLocation;
			SetIndex();
			UpdateChunkIndices();
		}
	}

	private IEnumerator Search()
	{
		_playerLocation = new Vector3((int)_player.position.x / _oneChunkSize,
									  0,
									  (int)_player.position.z / _oneChunkSize);

		while (true)
		{
			_oldLocation = _playerLocation;

			_playerLocation = new Vector3((int)_player.position.x / _oneChunkSize,
										  0,
										  (int)_player.position.z / _oneChunkSize);

			_movedChunkDirection = _playerLocation - _oldLocation;

			yield return new WaitForSeconds(0.1f);
		}
	}

	private void SetIndex()
	{
		int _startX = (int)System.Math.Max(0, _playerLocation.x - _halfWidth);
		int _endX = (int)System.Math.Min(_maptipIndicesElements, _playerLocation.x + _halfWidth);

		int _startZ = (int)System.Math.Max(0, _playerLocation.z - _halfDepth);
		int _endZ = (int)System.Math.Min(_maptipIndicesElements, _playerLocation.z + _halfDepth);

		_startIndexX = _startX;
		_startIndexZ = _startZ;
		_endIndexX = _endX;
		_endIndexZ = _endZ;
	}

	private void UpdateChunkIndices()
	{
		if (_movedChunkDirection.x == (int)MoveDirection.Right)
		{
			UpdateMoveX(0, _chunkIndices.Count - 1, _startIndexZ, _endIndexX);
		}
		else if (_movedChunkDirection.x == (int)MoveDirection.Left)
		{
			UpdateMoveX(_chunkIndices.Count - 1, 0, _startIndexZ, _startIndexX);
		}

		if (_movedChunkDirection.z == (int)MoveDirection.Forward)
		{
			UpdateMoveZ(0, _chunkIndices.Count - 1, _endIndexZ, _startIndexX);
		}
		else if (_movedChunkDirection.z == (int)MoveDirection.Back)
		{
			UpdateMoveZ(_chunkIndices.Count - 1, 0, _startIndexZ, _startIndexX);
		}
	}

	private void UpdateMoveX(int _removeIndex, int _insertIndex, int _depth, int _width)
	{
		int _count = 0;
		for (int i = 0; i <= _endIndexZ - _startIndexZ; i++)
		{
			var _index = _chunkIndices[i][_removeIndex];
			_chunkIndices[i].RemoveAt(_removeIndex);
			_chunkIndices[i].Insert(_insertIndex, _index);

			GrassUpdate(_width, _depth + _count, _index);
			_count++;
		}
	}

	private void UpdateMoveZ(int _removeIndex, int _insertIndex, int _depth, int _width)
	{
		var _indicesLine = _chunkIndices[_removeIndex];
		_chunkIndices.RemoveAt(_removeIndex);
		_chunkIndices.Insert(_insertIndex, _indicesLine);

		int _count = 0;
		foreach (var _index in _indicesLine)
		{
			GrassUpdate(_width + _count, _depth, _index);
			_count++;
		}
	}

	private void InitGrasses()
	{
		int _count = 0;

		for (int i = _startIndexZ; i <= _endIndexZ; i++)
		{
			List<int> _indicesLine = new List<int>();
			for(int j = _startIndexX; j <= _endIndexX; j++)
			{
				GrassUpdate(j, i, _count);
				_indicesLine.Add(_count);
				_count++;
			}
			_chunkIndices.Add(_indicesLine);
		}
	}

	private void GrassUpdate(int _chunkWidth, int _chunkDepth, int _count)
	{
		for (int i = 0; i < _oneChunkTipNum; i++)
		{
			Vector2 _index = _maptipsIndices[_chunkDepth, _chunkWidth, i];
			GrassDummyPoint _dummyPoint = _maptipsDummyPoint[(int)_index.x, (int)_index.y];
			GrassObject _targetObj = _pooledObjects[_count][i];

			if (!_dummyPoint.CanGrow)
			{
				_targetObj.Object.transform.SetPositionAndRotation(new Vector3(50000, -100, 0), _dummyPoint.Rotation * _targetObj.Object.transform.rotation);
				continue;
			}

			if (_dummyPoint.HasGrown)
			{
				_targetObj.Controller.Growth();
			}
			else
			{
				_targetObj.Controller.ForceScaleZero();
			}
			_targetObj.Object.transform.SetPositionAndRotation(_dummyPoint.Position, _dummyPoint.Rotation * _targetObj.Rotation);
		}
	}

	private void SetupMaptips()
	{

		int _countWidth = 0;
		int _countDepth = 0;

		for (int i = 0; i < _maptipIndicesElements; i++)
		{
			for (int j = 0; j < _oneLinePerChunkTipNum; j++)
			{
				for (int k = 0; k < _maptipIndicesElements; k++)
				{
					for (int l = 0; l < _oneLinePerChunkTipNum; l++)
					{
						_maptipsIndices[i, k, j * _oneLinePerChunkTipNum + l] = new Vector2(_countDepth, _countWidth);
						_countWidth++;
					}
				}
				_countWidth = 0;
				_countDepth++;
			}
		}
	}

	private void SetupDummyPoint()
	{
		for (int i = 0; i < _dummyPointElements; i++)
		{
			for (int j = 0; j < _dummyPointElements; j++)
			{
				SetPoint(j, i);
			}
		}

		SetupMaptips();
	}

	private void SetPoint(int _indexX, int _indexZ)
	{
		Vector3 _point = new Vector3(_indexX * _tipSize, 1000, _indexZ * _tipSize);
		Ray _ray = new Ray(_point, new Vector3(0, -1, 0));
		RaycastHit _hit;
		_maptipsDummyPoint[_indexZ, _indexX] = new GrassDummyPoint(new Vector3(_point.x, -1000, _point.z), Quaternion.identity, true, new Vector3(0, 0, 0));

		if (Physics.Raycast(_ray, out _hit, 2000, _lm))
		{
			_ray.origin = _point + new Vector3(_tipSize, 0, _tipSize);
			RaycastHit _anotherHit;

			if(Physics.Raycast(_ray,out _anotherHit, 2000, _lm))
			{
				if (CanGrow(_hit.point.y, _anotherHit.point.y))
				{
					_point = _hit.point;
					_maptipsDummyPoint[_indexZ, _indexX] = new GrassDummyPoint(_point, Quaternion.FromToRotation(-_ray.direction, _hit.normal), true, _hit.normal);
				}
			}
		}
	}

	private bool CanGrow(float _hit, float _anotherHit)
	{
		return ((-_grassData.Constraint < (_anotherHit - _hit)) && ((_anotherHit - _hit) < _grassData.Constraint));
	}
}
