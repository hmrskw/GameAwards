using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	[SerializeField]
	Transform _aa;

	private int _halfWidth;
	private int _halfDepth;

	private Vector3 _playerLocation = new Vector3();
	private Vector3 _oldLocation = new Vector3();
	private Vector3 _movedChunkDirection = new Vector3();

	private Vector3 _startIndex = new Vector3();
	private Vector3 _endIndex = new Vector3();
	private int _limitIndexCount = 800;

	private GrassDummyPoint[,] _maptipsDummyPoint = new GrassDummyPoint[2000, 2000];
	private Vector2[,,] _maptipsIndices = new Vector2[800, 800, 25];

	List<List<GameObject>> _pooledObjects = new List<List<GameObject>>();
	List<List<int>> _chunkIndices = new List<List<int>>();

	void Start ()
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
	
	void Update ()
	{
		if (_oldLocation != _playerLocation)
		{
			_oldLocation = _playerLocation;
			SetIndex();
			//InitGrasses();
			UpdateChunkIndices();
			Debug.Log(_aa.position);
		}
	}

	IEnumerator Search()
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

		_startIndex = new Vector3(_startX, 0, _startZ);
		_endIndex = new Vector3(_endX, 0, _endZ);
	}

	private void UpdateChunkIndices()
	{
		if (_movedChunkDirection == Vector3.zero) return;

		if (_movedChunkDirection.x == 1)
		{
			int _count = 0;
			for (int i = 0; i <= _endIndex.z - _startIndex.z; i++)
			{
				var _index = _chunkIndices[i][0];
				_chunkIndices[i].RemoveAt(0);
				_chunkIndices[i].Insert(_chunkIndices[i].Count, _index);

				for (int k = 0; k < 25; k++)
				{
					Vector2 _ind = _maptipsIndices[(int)_startIndex.z + _count, (int)_endIndex.x, k];
					GrassDummyPoint _dummyPoint = _maptipsDummyPoint[(int)_ind.x, (int)_ind.y];
					_pooledObjects[_index][k].transform.SetPositionAndRotation(_dummyPoint.Position, _dummyPoint.Rotation);
				}
				_count++;
			}
		}
		else if (_movedChunkDirection.x == -1)
		{
			int _count = 0;
			for (int i = 0; i <= _endIndex.z - _startIndex.z; i++)
			{
				var _index = _chunkIndices[i][_chunkIndices[i].Count - 1];
				_chunkIndices[i].RemoveAt(_chunkIndices[i].Count - 1);
				_chunkIndices[i].Insert(0, _index);

				for (int k = 0; k < 25; k++)
				{
					Vector2 _ind = _maptipsIndices[(int)_startIndex.z + _count, (int)_startIndex.x, k];
					GrassDummyPoint _dummyPoint = _maptipsDummyPoint[(int)_ind.x, (int)_ind.y];
					_pooledObjects[_index][k].transform.SetPositionAndRotation(_dummyPoint.Position, _dummyPoint.Rotation);
				}
				_count++;
			}
		}

		if (_movedChunkDirection.z == 1)
		{
			var _indicesLine = _chunkIndices[0];
			_chunkIndices.RemoveAt(0);
			_chunkIndices.Insert(_chunkIndices.Count, _indicesLine);

			int _count = 0;
			foreach (var _index in _indicesLine)
			{
				for (int i = 0; i < 25; i++)
				{
					Vector2 _ind = _maptipsIndices[(int)_endIndex.z, (int)_startIndex.x + _count, i];
					GrassDummyPoint _dummyPoint = _maptipsDummyPoint[(int)_ind.x, (int)_ind.y];
					_pooledObjects[_index][i].transform.SetPositionAndRotation(_dummyPoint.Position, _dummyPoint.Rotation);
				}
				_count++;
			}
		}
		else if (_movedChunkDirection.z == -1)
		{
			var _indicesLine = _chunkIndices[_chunkIndices.Count - 1];
			_chunkIndices.RemoveAt(_chunkIndices.Count - 1);
			_chunkIndices.Insert(0,_indicesLine);

			int _count = 0;
			foreach (var _index in _indicesLine)
			{
				for (int i = 0; i < 25; i++)
				{
					Vector2 _ind = _maptipsIndices[(int)_startIndex.z, (int)_startIndex.x + _count, i];
					GrassDummyPoint _dummyPoint = _maptipsDummyPoint[(int)_ind.x, (int)_ind.y];
					_pooledObjects[_index][i].transform.SetPositionAndRotation(_dummyPoint.Position, _dummyPoint.Rotation);
				}
				_count++;
			}
		}
	}

	private void InitGrasses()
	{
		int _count = 0;

		for (int i = (int)_startIndex.z; i <= _endIndex.z; i++)
		{
			List<int> _indicesLine = new List<int>();
			for(int j = (int)_startIndex.x; j <= _endIndex.x; j++)
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
			_pooledObjects[_count][i].transform.SetPositionAndRotation(_dummyPoint.Position, _dummyPoint.Rotation);
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

		for (int i = 0; i < 5; i++)
		{
			string _str = "";
			for (int j = 0; j < 5; j++)
			{
				_str += "(" + _maptipsIndices[0, 1, i * 5 + j].x + ", " + _maptipsIndices[0, 1, i * 5 + j].y + "), ";
			}
		}
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
