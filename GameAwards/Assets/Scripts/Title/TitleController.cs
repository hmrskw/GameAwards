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

	[SerializeField, Tooltip("プログレスバー(Slider)")]
	Slider _slider;

	[SerializeField, Tooltip("プログレスバー(Text)")]
	Text _text;

	private bool _isPushedAnyButtonOnce = false;
	private bool _isEnableGameStartButton = false;
	private bool _isPushedGameStartOnce = false;

	AsyncOperation _ope;
	private bool _operationCompleted = false;

	/// <summary>
	/// ロードしたシーンを有効化。
	/// </summary>
	public void ActivateScene()
	{
		if (_ope == null) return;
		_ope.allowSceneActivation = true;
	}

	private void Update ()
	{
		if (_isPushedGameStartOnce) return;

		if (!_isPushedAnyButtonOnce)
		{
			if (Input.anyKeyDown)
			{
				_isPushedAnyButtonOnce = true;
				StartCoroutine(DisplayPopup());
				StartCoroutine(LoadSceneAsync());
			}
		}

		if (_isEnableGameStartButton && _operationCompleted)
		{
			if (Input.GetButtonDown("Submit"))
			{
				_isPushedGameStartOnce = true;
				StartCoroutine(GoToGamemain());
			}
		}
	}

	private IEnumerator DisplayPopup()
	{
		_logos.SetActive(false);
		_Popup.SetActive(true);

		yield return new WaitUntil(() => _operationCompleted);
		Debug.Log("Complete!");
		_isEnableGameStartButton = true;
		_button.SetActive(true);
	}

	private IEnumerator GoToGamemain()
	{
		yield return StartCoroutine(FadeOutScreen());
		SceneManager.UnloadSceneAsync(0);
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

	private IEnumerator LoadSceneAsync()
	{
		_ope = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
		_ope.allowSceneActivation = false;

		while(_ope.progress < 1.0f)
		{
			_slider.value = _ope.progress;
			_text.text = (int)(_ope.progress * 100) + "%";
			yield return null;
		}

		_slider.value = 1.0f;
		_text.text = 100 + "%";

		_operationCompleted = true;
	}
}
