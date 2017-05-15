using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassDummyPoint
{
	public Vector3 Position { get; private set; }
	public Vector3 Normal { get; private set; }
	public Quaternion Rotation { get; private set; }
	public bool CanGrow { get; private set; }
	public bool HasGrown { get; private set; }

	public GrassDummyPoint(Vector3 _pos, Quaternion _rot, bool _cond, Vector3 _nor)
	{
		Position = _pos;
		Rotation = _rot;
		CanGrow = _cond;
		Normal = _nor;
		HasGrown = false;
	}

	public void SetHasGrown(bool _cond)
	{
		HasGrown = _cond;
	}
}
