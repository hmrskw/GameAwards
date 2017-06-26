using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndrollController : MonoBehaviour
{
	[SerializeField]
	Transform _rect;

	[SerializeField]
	float _fadeValue;

	[SerializeField]
	float _moveTime;

	[SerializeField]
	AnimationCurve _curve;

	private Vector3 _basePosition;

	private void Start()
	{
		_basePosition = _rect.localPosition;
		StartCoroutine(ScrollStaffRoll());
	}

	IEnumerator ScrollStaffRoll()
	{
		float _startTime = Time.timeSinceLevelLoad;
		float _elapsedTimeRatio = 0.0f;
		Vector3 _position = _basePosition;

		while (_elapsedTimeRatio <= 1.0f)
		{
			float _elapsedTime = Time.timeSinceLevelLoad;
			_elapsedTimeRatio = _elapsedTime / _moveTime;
			float _value = _curve.Evaluate(_elapsedTimeRatio);

			_position.y = _basePosition.y + _fadeValue * _value;
			_rect.localPosition = _position;
			yield return null;
		}

		yield return new WaitForSeconds(2.0f);
		FadeManager.Instance.FadeOutIn(5.0f, 2.0f, new Color(0, 0, 0), () => SceneManager.LoadScene(0));
	}
}
