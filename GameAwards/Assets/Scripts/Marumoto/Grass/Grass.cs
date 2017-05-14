using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
	[SerializeField]
	private AnimationCurve _growthCurve;
	[SerializeField, Tooltip("高さ１の植物が成長しきるまでにかかる時間(秒)")]
	private float _growthBaseTime;
	[SerializeField]
	private Vector3 _randomMin;
	[SerializeField]
	private Vector3 _randomMax;
	[SerializeField]
	private Transform _grassTransform;

	private Vector3 _limitGrowthScale;
	private float _growthTime = 0.0f;

	private void Awake()
	{
		_limitGrowthScale = new Vector3(Random.Range(_randomMin.x, _randomMax.x),
										Random.Range(_randomMin.y, _randomMax.y),
										Random.Range(_randomMin.z, _randomMax.z));

		_growthTime = _growthBaseTime;

		_grassTransform.rotation = Quaternion.Euler(0.0f,
													Random.Range(0.0f, 90.0f),
													0.0f);
	}

	/// <summary>
	/// この関数をStartCoroutineで呼べば、自動的に草が生えます。
	/// </summary>
	/// <returns></returns>
	public IEnumerator Growth()
	{
        float _startTime = Time.timeSinceLevelLoad;
		bool _isGrowing = true;

		while (_isGrowing)
		{
			float _elapsedTime = Time.timeSinceLevelLoad - _startTime;
			float _elapsedTimeRatio = _elapsedTime / _growthTime;
			float _growthRatio = _growthCurve.Evaluate(_elapsedTimeRatio);

			_grassTransform.localScale = _limitGrowthScale * _growthRatio;

			if (_growthRatio >= 1.0f) _isGrowing = false;
			yield return null;
		}
	}
}
