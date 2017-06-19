using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ********************************************
/// 制作者：丸本慶大
/// 2017/06/18
/// 
/// 脈シェーダをアタッチしたオブジェクトを
/// 一定周期で脈動させる。
/// ********************************************
/// </summary>
public class PulseController : MonoBehaviour
{
	[SerializeField]
	AnimationCurve _curve;

	[SerializeField]
	float _animationTime;

	[SerializeField]
	Vector3 _scalingDiff;
    [HideInInspector]
	public Vector3 baseScale;

	void Start ()
	{
		baseScale = transform.localScale;
		StartCoroutine(Pulse());
	}

	private IEnumerator Pulse()
	{
		while (true)
		{
			float _startTime = Time.timeSinceLevelLoad;
			float _elapsedTimeRatio = 0.0f;

			while(_elapsedTimeRatio < 1.0f)
			{
				float _elapsedTime = Time.timeSinceLevelLoad - _startTime;
				_elapsedTimeRatio = _elapsedTime / _animationTime;

				transform.localScale = baseScale + (_scalingDiff * _curve.Evaluate(_elapsedTimeRatio));
				yield return null;
			}
		}
	}
}
