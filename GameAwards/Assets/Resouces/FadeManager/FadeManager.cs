using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour {
	private FadeManager _instance;
	public FadeManager Instance
	{
		get { return _instance; }
	}

	[SerializeField]
	Image _fadeImage;

	public bool isFading { get; private set; }

	public IEnumerator FadeScene(int _sceneIndex, float _fadeinTime, float _fadeoutTime)
	{
		yield return null;
	}

	private IEnumerator FadeIn(float _fadeinTime)
	{
		yield return null;
	}

	private IEnumerator FadeOut(float _fadeoutTime)
	{
		yield return null;
	}

	private void Awake()
	{
		if(_instance == null)
		{
			_instance = this;
		}

		isFading = false;
		DontDestroyOnLoad(transform.gameObject);
	}
}
