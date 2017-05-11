using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassDummyPoint
{
	private bool _hasGrown = false;

	public Vector3 Position { get; private set; }
	public Vector3 Normal { get; private set; }
	public Quaternion Rotation { get; private set; }
	public bool CanGrow { get; private set; }

	public GrassDummyPoint(Vector3 _pos, Quaternion _rot, bool _cond, Vector3 _nor)
	{
		Position = _pos;
		Rotation = _rot;
		CanGrow = _cond;
		Normal = _nor;
	}

	public void SetHasGrown(bool _cond)
	{
		_hasGrown = _cond;
	}
}
