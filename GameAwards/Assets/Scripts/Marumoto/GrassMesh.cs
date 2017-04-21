using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassMesh : MonoBehaviour {
	[SerializeField]
	private GameObject _grassObject;
	[SerializeField]
	private Material _material;
	[SerializeField]
	private int _maxWidth;
	[SerializeField]
	private int _maxDepth;
	[SerializeField]
	Drawer _drawer;
	[SerializeField]
	private int _chunkWidth;
	[SerializeField]
	private int _chunkDepth;

	enum Drawer
	{
		DrawMesh=0,
		DrawMeshInstanced,
	}

	List<List<Matrix4x4>> _matrices = new List<List<Matrix4x4>>();
	List<Matrix4x4[]> _matrixArray = new List<Matrix4x4[]>();
	private Mesh _mesh;

	void Awake ()
	{
		_mesh = _grassObject.GetComponent<MeshFilter>().sharedMesh;

		for(int i = 0; i < _chunkDepth; i++)
		{
			for(int j = 0; j < _chunkWidth; j++)
			{
				SetupMatrix(j, i);
			}
		}

		foreach(List<Matrix4x4> _mat in _matrices)
		{
			_matrixArray.Add(_mat.ToArray());
		}
	}

	void Update()
	{
		DrawMeshInstanced();

		switch (_drawer)
		{
			case Drawer.DrawMesh:
				DrawMesh();
				break;

			case Drawer.DrawMeshInstanced:
				DrawMeshInstanced();
				break;

			default:
				break;
		}
	}

	void DrawMesh()
	{
		for (int j = 0; j < _maxDepth; j++)
		{
			for (int i = 0; i < _maxWidth; i++)
			{
				Graphics.DrawMesh(
					_mesh,
					Matrix4x4.TRS(new Vector3(1 * i, 0.5f, 1 * j), Quaternion.identity, new Vector3(1, 1, 1)),
					_material,
					1);
			}
		}
	}

	void DrawMeshInstanced()
	{
		foreach (Matrix4x4[] _matrix in _matrixArray)
		{
			Graphics.DrawMeshInstanced(_mesh, 0, _material, _matrix, 900);
		}
	}

	void DrawMeshInstancedIndirect()
	{
		//Graphics.DrawMeshInstancedIndirect()
	}

	void SetupMatrix(int _Width, int _Depth)
	{
		List<Matrix4x4> _matWidth = new List<Matrix4x4>();
		for (int j = 0; j < 30; j++)
		{
			for (int i = 0; i < 30; i++)
			{
				_matWidth.Add(Matrix4x4.TRS(new Vector3(1 * i + 30 * _Width, 0.5f, 1 * j + 30 * _Depth),
													   Quaternion.identity,
													   new Vector3(1, 1, 1)));
			}
		}
		_matrices.Add(_matWidth);
	}
}