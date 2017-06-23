using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ******************************************************
/// 制作者：丸本慶大
/// ******************************************************
/// タイトル時に出す世界観設定などのポップアップをコントロールするクラス。
/// </summary>
public class TitlePopupController : MonoBehaviour {
	[SerializeField, Tooltip("背景")]
	Image _background;

	[SerializeField, Tooltip("フレーバーテキスト")]
	List<Image> _flavorText;

	[SerializeField, Tooltip("背景の最終アルファ値(0~255)"), Range(0, 255)]
	float _backgroundAlpha;

	[SerializeField, Tooltip("アルファ値のカーブ")]
	AnimationCurve _curve;

	[SerializeField, Tooltip("フェード時間")]
	float _fadeTime;

	[SerializeField]
	TitleController _titleCtrl;

	void Start ()
	{
		StartCoroutine(FadeInPopup());
	}

	private IEnumerator FadeInPopup()
	{
		float _normalizedAlpha = _backgroundAlpha / 255.0f;
		float _startTime = Time.timeSinceLevelLoad;
		float _elapsedTimeRatio = 0.0f;
		Color _bgColor = _background.color;

		while (_elapsedTimeRatio <= 1.0f)
		{
			float _elapsedTime = Time.timeSinceLevelLoad - _startTime;
			_elapsedTimeRatio = _elapsedTime / _fadeTime;
			float _curveValue = _curve.Evaluate(_elapsedTimeRatio);

			_bgColor.a = _normalizedAlpha * _curveValue;
			_background.color = _bgColor;

			yield return null;
		}

		foreach(var _text in _flavorText)
		{
			_startTime = Time.timeSinceLevelLoad;
			_elapsedTimeRatio = 0.0f;
			Color _textColor = new Color(1, 1, 1, 0);

			while (_elapsedTimeRatio <= 1.0f)
			{
				float _elapsedTime = Time.timeSinceLevelLoad - _startTime;
				_elapsedTimeRatio = _elapsedTime / _fadeTime;
				float _curveValue = _curve.Evaluate(_elapsedTimeRatio);

				_textColor.a = _curveValue;
				_text.color = _textColor;

				yield return null;
			}
		}

		StartCoroutine(_titleCtrl.LoadSceneAsync());
	}
}
