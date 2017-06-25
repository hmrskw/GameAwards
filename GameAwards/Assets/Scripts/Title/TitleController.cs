using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// ******************************************************
/// 制作者：丸本慶大
/// ******************************************************
/// タイトルシーン管理クラス。
/// BuildSettingsのインデックス１がMainのシーンになるようにお願いします。
/// </summary>
public class TitleController : MonoBehaviour {
	[SerializeField, Tooltip("何かボタンが押されたときに表示する世界観のポップアップ")]
	GameObject _Popup;

	[SerializeField, Tooltip("GameMainに移動する時に押すボタンを表示したUI")]
	GameObject _button;

	[SerializeField, Tooltip("タイトルロゴとPushAnyButtonの文字")]
	Image _logos;

	[SerializeField]
	GameObject _selectMenues;

	[SerializeField, Tooltip("フェードのアニメーション用")]
	AnimationCurve _fadeCurve;

	[SerializeField, Tooltip("フェードアウトに掛かる時間(0秒だとゼロ除算でエラー)")]
	float _fadeTime;

	[SerializeField]
	Image _fadeMask;

	[SerializeField, Tooltip("プログレスバー(Slider)")]
	Slider _slider;

	[SerializeField, Tooltip("プログレスバー(Text)")]
	Text _text;

	[SerializeField]
	FlashButton _pushAnyButton;

	private bool _isPushedAnyButtonOnce = false;
	private bool _isEnableGameStartButton = false;
	private bool _isPushedGameStartOnce = false;
	private bool _hasSelected = false;

	private AsyncOperation _loadOpe;
	private bool _operationCompleted = false;

	/// <summary>
	/// 非同期でシーンを読み込みます。
	/// 今回はプログレスバーの処理もあります。
	/// </summary>
	/// <returns></returns>
	public IEnumerator LoadSceneAsync()
	{
		_loadOpe = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);

		while (!_loadOpe.isDone)
		{
			_slider.value = _loadOpe.progress;
			_text.text = (int)(_loadOpe.progress * 100) + "%";
			yield return null;
		}

		_slider.value = 1.0f;
		_text.text = 100 + "%";

		_operationCompleted = true;
	}

	private void Start()
	{
		SoundManager.Instance.PlayBGM("TitleBGM");
	}

	private void Update ()
	{
		if (_isPushedGameStartOnce) return;

		if (!_hasSelected && _isPushedAnyButtonOnce)
		{
			if (Input.GetButtonDown("Submit"))
			{
				SoundManager.Instance.PlaySE("se object");
				StartCoroutine(DisplayPopup());
				_hasSelected = true;
			}
			else if (Input.GetButtonDown("Cancel"))
			{
				SoundManager.Instance.PlaySE("se object");
				StartCoroutine(FadeOutGameExit());
				_hasSelected = true;
			}
		}

		if (!_isPushedAnyButtonOnce)
		{
			if (Input.anyKeyDown)
			{
				SoundManager.Instance.PlaySE("se object");
				_pushAnyButton.StopAnimation();
				_logos.enabled = false;
				_selectMenues.SetActive(true);
				_isPushedAnyButtonOnce = true;
			}
		}

		if (_isEnableGameStartButton && _operationCompleted)
		{
			if (Input.GetButtonDown("Submit"))
			{
				_isPushedGameStartOnce = true;
				FadeManager.Instance.FadeScene(0, 3.0f, 6.0f, new Color(0, 0, 0), _loadOpe);
			}
		}
	}

	private IEnumerator DisplayPopup()
	{
		_Popup.SetActive(true);

		yield return new WaitUntil(() => _operationCompleted);

		_isEnableGameStartButton = true;
		_button.SetActive(true);
	}

	private IEnumerator FadeOutGameExit()
	{
		Color _color = new Color(0, 0, 0, 1);
		float _startTime = Time.timeSinceLevelLoad;
		float _elapsedTimeRatio = 0.0f;

		while (_elapsedTimeRatio <= 1.0f)
		{
			float _elapsedTime = Time.timeSinceLevelLoad - _startTime;
			_elapsedTimeRatio = _elapsedTime / _fadeTime;
			float _curveValue = _fadeCurve.Evaluate(_elapsedTimeRatio);

			_color.a = _curveValue;
			_fadeMask.color = _color;
			yield return null;
		}

		Application.Quit();
	}
}
