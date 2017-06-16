using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// タイトルシーン管理クラス。
/// BuildSettingsのインデックス１がMainのシーンになるようにお願いします。
/// </summary>
public class TitleController : MonoBehaviour {
	[SerializeField, Tooltip("何かボタンが押されたときに表示する世界観のポップアップ")]
	GameObject _Popup;

	[SerializeField, Tooltip("GameMainに移動する時に押すボタンを表示したUI")]
	GameObject _button;

	[SerializeField, Tooltip("タイトルロゴとPushAnyButtonの文字")]
	GameObject _logos;

	[SerializeField, Tooltip("フェードアウト用のマスク画像")]
	Image _fadeMaskImage;

	[SerializeField, Tooltip("フェードのアニメーション用")]
	AnimationCurve _fadeCurve;

	[SerializeField, Tooltip("フェードアウトに掛かる時間(0秒だとゼロ除算でエラー)")]
	float _fadeTime;

	private bool _hasPushedAnyButtonAtFirst = false;
	private bool _isEnableGameStartButton = false;

	private void Update ()
	{
		if (!_hasPushedAnyButtonAtFirst)
		{
			if (Input.anyKeyDown)
			{
				StartCoroutine(DisplayPopup());
			}
		}

		if (_isEnableGameStartButton)
		{
			if (Input.GetButtonDown("Submit"))
			{
				StartCoroutine(GoToGamemain());
			}
		}
	}

	private IEnumerator DisplayPopup()
	{
		_logos.SetActive(false);
		_Popup.SetActive(true);

		yield return new WaitForSeconds(3.0f);

		_isEnableGameStartButton = true;
		_button.SetActive(true);
	}

	private IEnumerator GoToGamemain()
	{
		yield return StartCoroutine(FadeOutScreen());
		SceneManager.LoadScene(1);
	}

	private IEnumerator FadeOutScreen()
	{
		Color _color = new Color(0, 0, 0, 0);
		float _startTime = Time.timeSinceLevelLoad;
		float _curveValue = 0.0f;

		while (_curveValue < 1.0f)
		{
			float _elapsedTime = Time.timeSinceLevelLoad - _startTime;
			float _elapsedTimeRatio = _elapsedTime / _fadeTime;
			_curveValue = _fadeCurve.Evaluate(_elapsedTimeRatio);

			_color.a = _curveValue;
			_fadeMaskImage.color = _color;
			yield return null;
		}
	}
}
