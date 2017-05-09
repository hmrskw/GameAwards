using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassDummyPoint
{
	private bool _hasGrown = false;

	public Vector3 Position { get; private set; }
	public bool CanGrow { get; private set; }

	public GrassDummyPoint(Vector3 _pos, bool _cond)
	{
		Position = _pos;
		CanGrow = _cond;
	}

	public void SetHasGrown(bool _cond)
	{
		_hasGrown = _cond;
	}
}
