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
        while (FadeManager.Instance.IsFading == true)
        {
            Debug.Log(_rect.localPosition.y);
            yield return null;
        }
        float _startTime = Time.timeSinceLevelLoad;
		float _elapsedTimeRatio = 0.0f;
		Vector3 _position = _basePosition;

		while (_elapsedTimeRatio <= 1.0f)
		{
			float _elapsedTime = Time.timeSinceLevelLoad - _startTime;
			_elapsedTimeRatio = _elapsedTime / _moveTime;
			float _value = _curve.Evaluate(_elapsedTimeRatio);

            Debug.Log(_elapsedTime + "\n" + _elapsedTimeRatio + "\n" + _value);

			_position.y = _basePosition.y + _fadeValue * _value;
			_rect.localPosition = _position;
			yield return null;
		}

		yield return new WaitForSeconds(2.0f);
		FadeManager.Instance.FadeOutIn(5.0f, 2.0f, new Color(0, 0, 0), () => SceneManager.LoadScene(0),true);
	}
}
