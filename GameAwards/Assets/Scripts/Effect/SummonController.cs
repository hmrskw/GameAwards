using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ******************************************************
/// 制作者：丸本慶大
/// ******************************************************
/// エンディングのカットシーンでエネルギーが子供になるときの
/// エフェクトの大きさをコントロールできるようにするもの。
/// </summary>
public class SummonController : MonoBehaviour {
	[SerializeField, Tooltip("スケールのアニメーションカーブ")]
	AnimationCurve _curve;
	[SerializeField, Tooltip("アニメーションにかかる時間(秒)")]
	float _animationTime;
	[SerializeField, Tooltip("スケールの最大値")]
	public Vector3 _limitScale;

	void Start ()
	{
		StartCoroutine(ScaleAnimation());
	}

	private IEnumerator ScaleAnimation()
	{
		float _startTime = Time.timeSinceLevelLoad;
		float _elapsedTimeRatio = 0.0f;

		while(_elapsedTimeRatio <= 1.0f)
		{
			float _elapsedTime = Time.timeSinceLevelLoad - _startTime;
			_elapsedTimeRatio = _elapsedTime / _animationTime;

			transform.localScale = _limitScale * _curve.Evaluate(_elapsedTimeRatio);

			yield return null;
		}
	}
}
