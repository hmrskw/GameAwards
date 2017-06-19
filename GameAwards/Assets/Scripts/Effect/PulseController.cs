using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ********************************************
/// 制作者：丸本慶大
/// 2017/06/18
/// 
/// 脈シェーダをアタッチしたオブジェクトを
/// 一定周期で脈動させるためのコード。
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

	[SerializeField]
	MeshRenderer _renderer;

    [HideInInspector]
	public Vector3 baseScale;

	void Start ()
	{
		baseScale = transform.localScale;
		StartCoroutine(Pulse());
	}

	private IEnumerator Pulse()
	{
		MaterialPropertyBlock _mat = new MaterialPropertyBlock();
		while (true)
		{
			float _startTime = Time.timeSinceLevelLoad;
			float _elapsedTimeRatio = 0.0f;

			while(_elapsedTimeRatio < 1.0f)
			{
				float _elapsedTime = Time.timeSinceLevelLoad - _startTime;
				_elapsedTimeRatio = _elapsedTime / _animationTime;
				float _curveValue = _curve.Evaluate(_elapsedTimeRatio);

				transform.localScale = baseScale + (_scalingDiff * _curveValue);

				_mat.SetFloat("_InputAlpha", _curveValue);
				_renderer.SetPropertyBlock(_mat);
				yield return null;
			}
		}
	}
}
