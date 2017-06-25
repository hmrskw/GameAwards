using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndrollController : MonoBehaviour
{
	[SerializeField]
	RectTransform _rect;

	[SerializeField]
	float _fadeValue;

	[SerializeField]
	float _moveTime;

	[SerializeField]
	AnimationCurve _curve;

	private void Start()
	{
		StartCoroutine(ScrollStaffRoll());
	}

	IEnumerator ScrollStaffRoll()
	{
		float _startTime = Time.timeSinceLevelLoad;
		float _elapsedTimeRatio = 0.0f;
		Vector3 _position = _rect.localPosition;
		float _rectY = _rect.position.y;

        while (SceneManager.GetActiveScene().buildIndex != 2)
        {
            yield return null;
        }
        while (_elapsedTimeRatio <= 1.0f)
		{
			float _elapsedTime = Time.timeSinceLevelLoad;
			_elapsedTimeRatio = _elapsedTime / _moveTime;
			float _value = _curve.Evaluate(_elapsedTimeRatio);

			_position.y = _rectY + _fadeValue * _value;
			_rect.localPosition = _position;
			yield return null;
		}
        SceneManager.LoadScene(0);
        //AsyncOperation _loadOpe = SceneManager.LoadSceneAsync(0, LoadSceneMode.Additive);

        //FadeManager.Instance.FadeScene(2, 3.0f, 6.0f, new Color(0, 0, 0), _loadOpe);
	}
}
