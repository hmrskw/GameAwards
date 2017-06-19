using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ******************************************************
/// 制作者：丸本慶大
/// ******************************************************
/// ダミーポイント一つごとを管理するクラス。
/// 基本的にデータを持つ。
/// </summary>
public class GrassDummyPoint
{
	public Vector3 Position { get; private set; }
	public Vector3 Normal { get; private set; }
	public Quaternion Rotation { get; private set; }
	public bool CanGrow { get; private set; }
	public bool HasGrown { get; private set; }
	public int TexIndex { get; private set; }

	public GrassDummyPoint(Vector3 _pos, Quaternion _rot, bool _cond, Vector3 _nor)
	{
		Position = _pos;
		Rotation = _rot;
		CanGrow = _cond;
		Normal = _nor;
		HasGrown = false;
		TexIndex = 0;
	}

	public void SetHasGrown(bool _cond)
	{
		HasGrown = _cond;
	}

	public void SetTexIndex(int _index)
	{
		TexIndex = _index;
	}
}
