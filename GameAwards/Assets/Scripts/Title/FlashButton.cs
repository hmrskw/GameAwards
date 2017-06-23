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

	public Coroutine _coroutine { get; private set; }

	private void Start()
	{
		_coroutine = StartCoroutine(Flash());
	}

	private IEnumerator Flash()
	{
		float _startTime = Time.timeSinceLevelLoad;
		Color _color = new Color(1, 1, 1, 1);

		while (true)
		{
			float _elapsedTime = Time.timeSinceLevelLoad - _startTime;
			float _elapsedTimeRatio = (_elapsedTime / _animationTime) % 1.00f;
			float _curveValue = _curve.Evaluate(_elapsedTimeRatio);

			_color.a = _curveValue;
			_image.color = _color;

			yield return null;
		}
	}
}
