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
	int _createWidth;
	[SerializeField]
	int _createDepth;
    [SerializeField]
    LayerMask _lm;

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

	private int _limitIndexCount = 800;

	private GrassDummyPoint[,] _maptipsDummyPoint = new GrassDummyPoint[2000, 2000];
	private Vector2[,,] _maptipsIndices = new Vector2[800, 800, 25];

	List<List<GameObject>> _pooledObjects = new List<List<GameObject>>();
	List<List<int>> _chunkIndices = new List<List<int>>();

	/// <summary>
	/// 渡された第一引数のPosition値から、対応するDummyPointのXとZのインデックスを代入する。
	/// </summary>
	/// <param name="_position">ポジション</param>
	/// <param name="_xIndex">Xのインデックスを受け取りたい変数</param>
	/// <param name="_zIndex">Zのインデックスを受け取りたい変数</param>
	public void SearchDummyPointIndex(Vector3 _position, out int _xIndex, out int _zIndex)
	{
		const int _objectSize = 4;
		_xIndex = new int();
		_zIndex = new int();

		_xIndex = (int)_position.x / _objectSize;
		_zIndex = (int)_position.z / _objectSize;
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
		if (_createWidth % 2 == 1) _halfWidth = (_createWidth - 1) / 2;
		else _halfWidth = _createWidth / 2;
		if (_createDepth % 2 == 1) _halfDepth = (_createDepth - 1) / 2;
		else _halfDepth = _createDepth / 2;

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
		int _chunkSize = 20;
		_playerLocation = new Vector3((int)_player.position.x / _chunkSize,
									  0,
									  (int)_player.position.z / _chunkSize);

		while (true)
		{
			_oldLocation = _playerLocation;

			_playerLocation = new Vector3((int)_player.position.x / _chunkSize,
										  0,
										  (int)_player.position.z / _chunkSize);

			_movedChunkDirection = _playerLocation - _oldLocation;

			yield return new WaitForSeconds(0.1f);
		}
	}

	private void SetIndex()
	{
		int _startX = (int)System.Math.Max(0, _playerLocation.x - _halfWidth);
		int _endX = (int)System.Math.Min(_limitIndexCount, _playerLocation.x + _halfWidth);

		int _startZ = (int)System.Math.Max(0, _playerLocation.z - _halfDepth);
		int _endZ = (int)System.Math.Min(_limitIndexCount, _playerLocation.z + _halfDepth);

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

			for (int k = 0; k < 25; k++)
			{
				GrassUpdate(_width, _depth + _count, _index);
			}
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
		for (int i = 0; i < 25; i++)
		{
			Vector2 _index = _maptipsIndices[_chunkDepth, _chunkWidth, i];
			GrassDummyPoint _dummyPoint = _maptipsDummyPoint[(int)_index.x, (int)_index.y];
			GameObject _targetObj =_pooledObjects[_count][i];

			if (_dummyPoint.CanGrow)
			{
				if (_dummyPoint.HasGrown)
				{
					_targetObj.GetComponentInChildren<GrassesController>().Growth();
				}
				else
				{
					_targetObj.GetComponentInChildren<GrassesController>().ForceScaleZero();
				}
			}
			_targetObj.transform.SetPositionAndRotation(_dummyPoint.Position, _dummyPoint.Rotation);
		}
	}

	private void SetupMaptips()
	{
		int _chunkWidth = 800;
		int _chunkDepth = 800;
		int _tipWidth = 5;
		int _tipDepth = 5;

		int _countWidth = 0;
		int _countDepth = 0;

		for (int i = 0; i < _chunkDepth; i++)
		{
			for (int j = 0; j < _tipDepth; j++)
			{
				for (int k = 0; k < _chunkWidth; k++)
				{
					for (int l = 0; l < _tipWidth; l++)
					{
						_maptipsIndices[i, k, j * 5 + l] = new Vector2(_countDepth, _countWidth);
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
		const int _tipNum = 2000;
		
		for (int i = 0; i < _tipNum; i++)
		{
			for (int j = 0; j < _tipNum; j++)
			{
				SetPoint(j, i);
			}
		}

		SetupMaptips();
	}

	private void SetPoint(int _indexX, int _indexZ)
	{
		Vector3 _point = new Vector3(_indexX * 4, 100, _indexZ * 4);
		Ray _ray = new Ray(_point, new Vector3(0, -1, 0));
		RaycastHit _hit = new RaycastHit();

		if (Physics.Raycast(_ray, out _hit, 200, _lm))
		{
			_point = _hit.point;
			_maptipsDummyPoint[_indexZ, _indexX] = new GrassDummyPoint(_point, Quaternion.FromToRotation(-_ray.direction, _hit.normal), true,_hit.normal);
		}
		else
		{
			_maptipsDummyPoint[_indexZ, _indexX] = new GrassDummyPoint(new Vector3(_point.x, 0, _point.z), Quaternion.identity, true, new Vector3(0, 0, 0));
		}
	}
}
