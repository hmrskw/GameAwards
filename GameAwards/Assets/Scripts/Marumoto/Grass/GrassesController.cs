using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassesController : MonoBehaviour {
	[SerializeField]
	List<Grass> _grasses;

	public void Growth()
	{
		foreach(var _grass in _grasses)
		{
			StartCoroutine(_grass.Growth());
		}
	}
}
