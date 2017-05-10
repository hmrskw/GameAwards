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

	private Vector3 _playerLocation = new Vector3();
	private Vector3 _oldLocation = new Vector3();

	private Vector3 _startIndex = new Vector3();
	private Vector3 _endIndex = new Vector3();
	private int _limitIndexCount = 800;

	private GrassDummyPoint[,] _maptipsDummyPoint = new GrassDummyPoint[2000, 2000];
	private Vector2[,,] _maptipsIndices = new Vector2[800, 800, 25];

	List<List<GameObject>> _pooledObjects = new List<List<GameObject>>();

	void Start ()
	{
		_pooledObjects = _pooler.GetObjects();
		SetupDummyPoint();
		StartCoroutine(Search());
		InitGrasses();
	}
	
	void Update ()
	{
		if (_oldLocation != _playerLocation)
		{
			InitGrasses();
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

			SetIndex();

			yield return new WaitForSeconds(0.3f);
		}
	}

	private void SetIndex()
	{
		int _halfWidth = _createWidth / 2;
		int _halfDepth = _createDepth / 2;

		int _startX = (int)System.Math.Max(0, _playerLocation.x - _halfWidth);
		int _endX = (int)System.Math.Min(_limitIndexCount, _playerLocation.x + _halfWidth);

		int _startZ = (int)System.Math.Max(0, _playerLocation.z - _halfDepth);
		int _endZ = (int)System.Math.Min(_limitIndexCount, _playerLocation.z + _halfDepth);

		_startIndex = new Vector3(_startX, 0, _startZ);
		_endIndex = new Vector3(_endX, 0, _endZ);
	}

	private void InitGrasses()
	{
		int _count = 0;
		for (int i = (int)_startIndex.z; i < _endIndex.z; i++)
		{
			for(int j = (int)_startIndex.x; j < _endIndex.x; j++)
			{
				GrassUpdate(j, i, _count);
				_count++;
			}
		}
	}

	private void GrassUpdate(int _chunkWidth, int _chunkDepth, int _count)
	{
		for (int i = 0; i < 25; i++)
		{
			Vector2 _index = _maptipsIndices[_chunkDepth, _chunkWidth, i];
			_pooledObjects[_count][i].transform.SetPositionAndRotation(_maptipsDummyPoint[(int)_index.x, (int)_index.y].Position, Quaternion.identity);
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
			_maptipsDummyPoint[_indexZ, _indexX] = new GrassDummyPoint(_point, true);
		}
		else
		{
            _maptipsDummyPoint[_indexZ, _indexX] = new GrassDummyPoint(new Vector3(_point.x, 0, _point.z), true);
		}
	}
}
