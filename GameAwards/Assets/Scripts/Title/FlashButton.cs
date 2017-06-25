using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FlashButton : MonoBehaviour
{
	[SerializeField]
	AnimationCurve _curve;

	[SerializeField]
	float _animationTime;

	[SerializeField]
	Image _image;

	private bool _isLooping;

	private Coroutine _coroutine;

	private void Start()
	{
		_isLooping = true;
		_coroutine = StartCoroutine(Flash());
	}

	public void StopAnimation()
	{
		_isLooping = false;
	}

	private IEnumerator Flash()
	{
		while (true)
		{
			float _startTime = Time.timeSinceLevelLoad;
			Color _color = new Color(1, 1, 1, 1);
			float _elapsedTimeRatio = 0.0f;

			while (_elapsedTimeRatio <= 1.0f)
			{
				float _elapsedTime = Time.timeSinceLevelLoad - _startTime;
				_elapsedTimeRatio = _elapsedTime / _animationTime;
				float _curveValue = _curve.Evaluate(_elapsedTimeRatio);

				_color.a = _curveValue;
				_image.color = _color;

				if (!_isLooping)
				{
					_image.color = new Color(1, 1, 1, 1);
					yield break;
				}

				yield return null;
			}
		}
	}
}
