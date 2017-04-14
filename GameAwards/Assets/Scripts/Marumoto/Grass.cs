using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
	[SerializeField]
	private AnimationCurve _curve;
	[SerializeField, Tooltip("成長しきるまでにかかる時間(秒)")]
	private float _growthTime;
	[SerializeField]
	private Vector3 _randomMin;
	[SerializeField]
	private Vector3 _randomMax;

	private Vector3 _limitGrowthScale;

	private void Awake()
	{
		_limitGrowthScale = new Vector3(Random.Range(_randomMin.x, _randomMax.x),
										Random.Range(_randomMin.y, _randomMax.y),
										Random.Range(_randomMin.z, _randomMax.z));
	}

	/// <summary>
	/// この関数をStartCoroutineで呼べば、自動的に草が生えます。
	/// </summary>
	/// <returns></returns>
	public IEnumerator Growth()
	{

		float _startTime = Time.timeSinceLevelLoad;
		bool _isGrowing = true;

		//TODO:UniRxで書き換え。
		while (_isGrowing)
		{
			float _elapsedTime = Time.timeSinceLevelLoad - _startTime;
			float _elapsedTimeRatio = _elapsedTime / _growthTime;
			float _growthRatio = _curve.Evaluate(_elapsedTimeRatio);

			transform.localScale = _limitGrowthScale * _growthRatio;

			if (_growthRatio >= 1.0f) _isGrowing = false;
			yield return null;
		}

		Destroy(this);
	}
}
