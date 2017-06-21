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

/// <summary>
/// ******************************************************
/// 制作者：丸本慶大
/// ******************************************************
/// チャンクロード方式を管理するクラス。
/// 一定範囲のみ草を読み込んで生やす。
/// </summary>
public class GrassManager : MonoBehaviour {
	[SerializeField]
	ObjectPooler _pooler;
	[SerializeField]
	Transform _player;
    [SerializeField]
    LayerMask _lm;
	[SerializeField]
	GrassData _grassData;
	[SerializeField]
	List<Texture> _textures;

	private MaterialPropertyBlock _matPropBlock;

	private Vector3 _playerLocation = new Vector3();
	private Vector3 _oldLocation = new Vector3();
	private Vector3 _movedChunkDirection = new Vector3();

	//intキャストでのindexズレ防止のため変数を4つ用意。
	private int _startIndexX = 0;
	private int _startIndexZ = 0;
	private int _endIndexX = 0;
	private int _endIndexZ = 0;

	private GrassChunk _grassChunk;
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

		_xIndex = (int)_position.x / _grassChunk.TipSize;
		_zIndex = (int)_position.z / _grassChunk.TipSize;
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

	/// <summary>
	/// DummyPointのTextureのインデックスを変更。
	/// </summary>
	/// <param name="_xIndex">ダミーポイントのXインデックス</param>
	/// <param name="_zIndex">ダミーポイントのZインデックス</param>
	/// <param name="_texIndex">テクスチャの状態に合わせてインデックスを変更。</param>
	public void ChangeTexIndex(int _xIndex, int _zIndex, int _texIndex)
	{
		_maptipsDummyPoint[_zIndex, _xIndex].SetTexIndex(_texIndex);
	}

	/// <summary>
	/// インデックスからダミーポイントを取得し返却する。
	/// </summary>
	/// <param name="_xIndex"></param>
	/// <param name="_zIndex"></param>
	/// <returns></returns>
	public GrassDummyPoint GetDummyPoint(int _xIndex, int _zIndex)
	{
		return _maptipsDummyPoint[_zIndex, _xIndex];
	}

	/// <summary>
	/// 与えられたインデックスに応じたテクスチャをセットしたMaterialPropetyBlockを返す。
	/// </summary>
	/// <param name="_texIndex"></param>
	/// <returns></returns>
	public MaterialPropertyBlock GetMatPropBlock(int _texIndex)
	{
		_matPropBlock.SetTexture("_MainTex", _textures[_texIndex]);
		return _matPropBlock;
	}

	/// <summary>
	/// 1から引数で与えられた数値までのランダム値を返却。
	/// </summary>
	/// <param name="_maxTexCount"></param>
	/// <returns></returns>
	public int GetRandomTextureIndex(int _maxTexCount)
	{
		return Random.Range(1, _maxTexCount);
	}

	private void Start ()
	{
		_grassChunk = new GrassChunk(_grassData);
		_matPropBlock = new MaterialPropertyBlock();

		_maptipsDummyPoint = new GrassDummyPoint[_grassChunk.DummyPointElements, _grassChunk.DummyPointElements];
		_maptipsIndices = new Vector2[_grassChunk.MaptipIndicesElements, _grassChunk.MaptipIndicesElements, _grassChunk.OneChunkTipNum];

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
		_playerLocation = new Vector3((int)_player.position.x / _grassChunk.OneChunkSize,
									  0,
									  (int)_player.position.z / _grassChunk.OneChunkSize);

		var _waitSeconds = new WaitForSeconds(0.2f);

		while (true)
		{
			_oldLocation = _playerLocation;

			_playerLocation = new Vector3((int)_player.position.x / _grassChunk.OneChunkSize,
										  0,
										  (int)_player.position.z / _grassChunk.OneChunkSize);

			_movedChunkDirection = _playerLocation - _oldLocation;

			yield return _waitSeconds;
		}
	}

	private void SetIndex()
	{
		int _startX = (int)System.Math.Max(0, _playerLocation.x - _grassChunk.HalfWidth);
		int _endX = (int)System.Math.Min(_grassChunk.MaptipIndicesElements, _playerLocation.x + _grassChunk.HalfWidth);

		int _startZ = (int)System.Math.Max(0, _playerLocation.z - _grassChunk.HalfDepth);
		int _endZ = (int)System.Math.Min(_grassChunk.MaptipIndicesElements, _playerLocation.z + _grassChunk.HalfDepth);

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
		for(int i = 0; i < _indicesLine.Count; i++)
		{
			GrassUpdate(_width + _count, _depth, _indicesLine[i]);
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
		for (int i = 0; i < _grassChunk.OneChunkTipNum; i++)
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

			_matPropBlock.SetTexture("_MainTex", _textures[_dummyPoint.TexIndex]);
			_targetObj.Controller.ChangeMaterials(_matPropBlock);
		}
	}

	private void SetupMaptips()
	{

		int _countWidth = 0;
		int _countDepth = 0;

		for (int i = 0; i < _grassChunk.MaptipIndicesElements; i++)
		{
			for (int j = 0; j < _grassChunk.OneLinePerChunkTipNum; j++)
			{
				for (int k = 0; k < _grassChunk.MaptipIndicesElements; k++)
				{
					for (int l = 0; l < _grassChunk.OneLinePerChunkTipNum; l++)
					{
						_maptipsIndices[i, k, j * _grassChunk.OneLinePerChunkTipNum + l] = new Vector2(_countDepth, _countWidth);
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
		for (int i = 0; i < _grassChunk.DummyPointElements; i++)
		{
			for (int j = 0; j < _grassChunk.DummyPointElements; j++)
			{
				SetPoint(j, i);
			}
		}

		SetupMaptips();
	}

	private void SetPoint(int _indexX, int _indexZ)
	{
		Vector3 _point = new Vector3(_indexX * _grassChunk.TipSize, 1000, _indexZ * _grassChunk.TipSize);
		Ray _ray = new Ray(_point, new Vector3(0, -1, 0));
		RaycastHit _hit;
		_maptipsDummyPoint[_indexZ, _indexX] = new GrassDummyPoint(new Vector3(_point.x, -1000, _point.z), Quaternion.identity, true, new Vector3(0, 0, 0));

		if (Physics.Raycast(_ray, out _hit, 2000, _lm))
		{
			_ray.origin = _point + new Vector3(_grassChunk.TipSize, 0, _grassChunk.TipSize);
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
