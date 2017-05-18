using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassesController : MonoBehaviour {
	[SerializeField]
	List<Grass> _grasses;

	[SerializeField]
	GrassData _grassData;

	private void Awake()
	{
		foreach(var _grass in _grasses)
		{
			_grass.Setup(_grassData.RandomMin, _grassData.RandomMax, _grassData.GrowthBaseTime, _grassData.WitherBaseTime, _grassData.Curve);
		}
	}

	public void Growth()
	{
		foreach(var _grass in _grasses)
		{
			StartCoroutine(_grass.Growth());
		}
	}

	public void Wither()
	{
		foreach (var _grass in _grasses)
		{
			StartCoroutine(_grass.Wither());
		}
	}

	public void ForceScaleZero()
	{
		foreach(var _grass in _grasses)
		{
			_grass.ForceScaleZero();
		}
	}
}
