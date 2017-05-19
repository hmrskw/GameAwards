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
		for (int i = 0; i < _grasses.Count; i++)
		{
			StartCoroutine(_grasses[i].Growth());
		}
	}

	public void Wither()
	{
		for (int i = 0; i < _grasses.Count; i++)
		{
			StartCoroutine(_grasses[i].Wither());
		}
	}

	public void ForceScaleZero()
	{
		for (int i = 0; i < _grasses.Count; i++)
		{
			_grasses[i].ForceScaleZero();
		}
	}
}
