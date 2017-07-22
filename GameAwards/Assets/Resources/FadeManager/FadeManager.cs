using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// *************************************************
/// 制作者 丸本慶大
/// 2017/06/18
/// *************************************************
/// DontDestroyOnLoadでシーン間フェードを管理するクラス。
/// *************************************************
/// </summary>
public class FadeManager : MonoBehaviour {
	private static FadeManager _instance;
	public static FadeManager Instance
	{
		get { return _instance; }
	}
	[SerializeField]
	AnimationCurve _fadeoutCurve;

	[SerializeField]
	AnimationCurve _fadeinCurve;

	[SerializeField]
	Image _fadeImage;

	public bool IsFading { get; private set; }

	/// <summary>
	/// シーンを跨いでコルーチンを動かしたいのでラッピングする。
	/// 
	/// 直接コルーチンを呼ぶと、そのシーンが無効になった段階でコルーチンも止まるため、
	/// コルーチンの呼び出し元を、DontDestroyOnLoadのオブジェクトにする対策。
	/// </summary>
	/// <param name="_unloadIndex">Unloadしたいシーンのインデックス</param>
	/// <param name="_fadeoutTime">フェードアウトにかかる時間(秒)</param>
	/// <param name="_fadeInTime">フェードインにかかる時間(秒)</param>
	/// <param name="_maskColor">フェード時のマスク画像の色</param>
	/// <param name="_nextOpe">次のシーンのAsyncOperation</param>
	public void FadeScene(int _unloadIndex, float _fadeoutTime, float _fadeInTime, Color _maskColor, AsyncOperation _nextOpe)
	{
		StartCoroutine(FadeSceneImpl(_unloadIndex, _fadeoutTime, _fadeInTime, _maskColor, _nextOpe));
	}

	/// <summary>
	/// フェードアウトとフェードインをする関数。
	/// こちらもシーンをまたぐことを想定し、ラッピング関数です。
	/// 
	/// フェードアウトとフェードインの間にアクションを渡すことで何か処理させることもできます。
	/// </summary>
	/// <param name="_fadeoutTime">フェードアウト時間(s)</param>
	/// <param name="_fadeinTime">フェードイン時間(s)</param>
	/// <param name="_maskColor">マスク画像の色</param>
	/// <param name="_act">何かしたい場合はここにアクションを渡す</param>
	public void FadeOutIn(float _fadeoutTime, float _fadeinTime, Color _maskColor, Action _act = null, bool _bgmFadeOut = false)
	{
		StartCoroutine(FadeOutInImpl(_fadeoutTime, _fadeinTime, _maskColor, _bgmFadeOut, _act));
	}

	private IEnumerator FadeSceneImpl(int _unloadSceneIndex, float _fadeoutTime, float _fadeinTime, Color _maskColor, AsyncOperation _nextOpe)
	{
		if (IsFading) yield break;
		IsFading = true;
		_fadeImage.gameObject.SetActive(true);
        StartCoroutine(SoundManager.Instance.BGMFadeOut(_fadeoutTime));
		yield return StartCoroutine(Fade(_fadeoutTime, _maskColor, _fadeoutCurve));
		//SoundManager.Instance.StopAll();

		if (_nextOpe != null)
		{
			if (!_nextOpe.allowSceneActivation)
			{
				_nextOpe.allowSceneActivation = true;
			}
		}
		var _unloadOpe = SceneManager.UnloadSceneAsync(_unloadSceneIndex);
		yield return _unloadOpe;
		
		yield return StartCoroutine(Fade(_fadeinTime, _maskColor, _fadeinCurve));

		_fadeImage.gameObject.SetActive(false);
		IsFading = false;
	}

	private IEnumerator FadeOutInImpl(float _fadeoutTime, float _fadeinTime, Color _maskColor, bool _bgmFadeOut, Action _act = null)
	{
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (IsFading) yield break;
		IsFading = true;
        if (_bgmFadeOut == true)
        {
            StartCoroutine(SoundManager.Instance.BGMFadeOut(_fadeoutTime));
        }
        _fadeImage.gameObject.SetActive(true);

		yield return StartCoroutine(Fade(_fadeoutTime, _maskColor, _fadeoutCurve));
		//SoundManager.Instance.StopAll();

		if (_act != null)
		{
			_act();
		}

        while (SceneManager.GetActiveScene().buildIndex == sceneIndex)
        {
            yield return null;
        }

        yield return StartCoroutine(Fade(_fadeinTime, _maskColor, _fadeinCurve));
		_fadeImage.gameObject.SetActive(false);
		IsFading = false;
	}

	private IEnumerator Fade(float _fadeTime, Color _maskColor, AnimationCurve _curve)
	{
		float _startTime = Time.timeSinceLevelLoad;
		float _elapsedTimeRatio = 0.0f;
		_maskColor.a = _curve.Evaluate(0.0f);
		Color _color = _maskColor;

		while (_elapsedTimeRatio < 1.0f)
		{
			float _elapsedTime = Time.timeSinceLevelLoad - _startTime;
			_elapsedTimeRatio = _elapsedTime / _fadeTime;
            _color.a = _curve.Evaluate(_elapsedTimeRatio);
			_fadeImage.color = _color;

			yield return null;
		}
	}

	private void Awake()
	{
		if(_instance == null)
		{
			_instance = this;
		}

		IsFading = false;
		DontDestroyOnLoad(transform.gameObject);
	}
}
