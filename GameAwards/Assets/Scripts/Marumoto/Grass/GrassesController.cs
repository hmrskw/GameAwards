using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassesController : MonoBehaviour {
	[SerializeField]
	List<Grass> _grasses;

	[SerializeField]
	private AnimationCurve _curve;
	[SerializeField]
	private float _growthBaseTime;
	[SerializeField]
	private float _witherBaseTime;
	[SerializeField]
	private Vector3 _randomMin;
	[SerializeField]
	private Vector3 _randomMax;

	private void Awake()
	{
		foreach(var _grass in _grasses)
		{
			_grass.Setup(_randomMin, _randomMax, _growthBaseTime, _witherBaseTime, _curve);
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
			StartCoroutine(_grass.Growth());
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
