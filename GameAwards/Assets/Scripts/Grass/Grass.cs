using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
	[SerializeField]
	private Transform _tagTransform;

	private AnimationCurve _animationCurve;
	private AnimationCurve _changedTexCurve;
	private Vector3 _limitGrowthScale;
	private float _growthTime = 0.0f;
	private float _witherTime = 0.0f;

	private bool _isAnimation = false;
	private IEnumerator _coroutine;

	public void Setup(Vector3 _randomMin, Vector3 _randomMax, float _growthBaseTime, float _witherBaseTime, AnimationCurve _curve, AnimationCurve _changeCurve)
	{
		_animationCurve = _curve;
		_changedTexCurve = _changeCurve;

		_limitGrowthScale = new Vector3(UnityEngine.Random.Range(_randomMin.x, _randomMax.x),
										UnityEngine.Random.Range(_randomMin.y, _randomMax.y),
										UnityEngine.Random.Range(_randomMin.z, _randomMax.z));

		_growthTime = _growthBaseTime;
		_witherTime = _witherBaseTime;

		transform.rotation = Quaternion.Euler(0.0f,
											  UnityEngine.Random.Range(0.0f, 90.0f),
											  0.0f);
	}

	/// <summary>
	/// この関数をStartCoroutineで呼べば、自動的に草が生えます。
	/// </summary>
	/// <returns></returns>
	public IEnumerator Growth()
	{
		if (_isAnimation) yield break;
		_isAnimation = true;

        _coroutine = Growth();
		_tagTransform.tag = "GrownGrass";

        float _startTime = Time.timeSinceLevelLoad;

		while (_isAnimation)
		{
			float _elapsedTime = Time.timeSinceLevelLoad - _startTime;
			float _elapsedTimeRatio = _elapsedTime / _growthTime;
			float _growthRatio = _animationCurve.Evaluate(_elapsedTimeRatio);
			transform.localScale = _limitGrowthScale * _growthRatio;

			if (_elapsedTimeRatio >= 1.0f)
			{
				_isAnimation = false;
				_coroutine = null;
			}
			yield return null;
		}
	}

	public IEnumerator Wither()
	{
        if (_isAnimation) yield break;
        _isAnimation = true;

		_coroutine = Wither();
		_tagTransform.tag = "WitheredGrass";

		float _startTime = Time.timeSinceLevelLoad;

		while (_isAnimation)
		{
			float _elapsedTime = Time.timeSinceLevelLoad - _startTime;
			float _elapsedTimeRatio = 1.0f - (_elapsedTime / _witherTime);
			float _growthRatio = _animationCurve.Evaluate(_elapsedTimeRatio);

			transform.localScale = _limitGrowthScale * _growthRatio;

			if (_elapsedTimeRatio <= 0.0f)
			{
				_isAnimation = false;
				_coroutine = null;
			}
			yield return null;
		}
	}

	public void ForceScaleZero()
	{
		if (_coroutine != null)
		{
			StopCoroutine(_coroutine);
			_isAnimation = false;
		}
		_tagTransform.tag = "WitheredGrass";

		transform.localScale = Vector3.zero;
	}

	public IEnumerator GrowthChangedTexture(Action _act)
	{
		_isAnimation = true;

		if (_coroutine != null)
		{
			StopCoroutine(_coroutine);
		}

		_coroutine = GrowthChangedTexture(_act);

		float _startTime = Time.timeSinceLevelLoad;
		bool _hasAction = false;
		while (_isAnimation)
		{
			float _elapsedTime = Time.timeSinceLevelLoad - _startTime;
			float _elapsedTimeRatio = _elapsedTime / _witherTime;
			float _growthRatio = _changedTexCurve.Evaluate(_elapsedTimeRatio);
			transform.localScale = _limitGrowthScale * _growthRatio;

			if(_elapsedTimeRatio >= 0.5f && !_hasAction)
			{
				_hasAction = true;
				_act();
			}

			if (_elapsedTimeRatio >= 1.0f)
			{
				_isAnimation = false;
				_coroutine = null;
			}
			yield return null;
		}
	}
}
