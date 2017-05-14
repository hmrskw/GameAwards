using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassesController : MonoBehaviour {
	[SerializeField]
	List<Grass> _grasses;

	[SerializeField]
	private AnimationCurve _curve;
	[SerializeField, Tooltip("高さ１の植物が成長しきるまでにかかる時間(秒)")]
	private float _growthBaseTime;
	[SerializeField]
	private Vector3 _randomMin;
	[SerializeField]
	private Vector3 _randomMax;

	private void Awake()
	{
		foreach(var _grass in _grasses)
		{
			_grass.Setup(_randomMin, _randomMax, _growthBaseTime, _curve);
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
		foreach(var _grass in _grasses)
		{
			StartCoroutine(_grass.Wither());
		}
	}
}
