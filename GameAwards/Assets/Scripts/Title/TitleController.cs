using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// タイトルシーン管理クラス。
/// BuildSettingsのインデックス１がMainのシーンになるようにお願いします。
/// </summary>
public class TitleController : MonoBehaviour {
	private bool _progressLoading;
	private bool _canChange;
	private AsyncOperation _nextSceneOpe;

	private void Awake ()
	{
		_progressLoading = false;
		_canChange = false;
	}

	void Update ()
	{
		if (!_progressLoading)
		{
			if (Input.anyKeyDown)
			{
				LoadSceneAsyncBackground(1);
			}
		}

		if (_nextSceneOpe != null)
		{
			if (_nextSceneOpe.isDone)
			{
				_nextSceneOpe.allowSceneActivation = false;
			}
		}

		if (_canChange)
		{

		}

		if (Input.GetKeyDown(KeyCode.G))
		{
			MoveNextScene(0);
		}
	}

	void LoadSceneAsyncBackground(int _sceneIndex)
	{
		_progressLoading = true;
		_nextSceneOpe = SceneManager.LoadSceneAsync(_sceneIndex, LoadSceneMode.Additive);
		_nextSceneOpe.allowSceneActivation = false;
	}

	/// <summary>
	/// 現在のシーンを無効化し
	/// ロードしたシーンを有効化
	/// </summary>
	public void MoveNextScene(int _sceneIndex)
	{
		SceneManager.UnloadSceneAsync(_sceneIndex);
		_nextSceneOpe.allowSceneActivation = true;
	}
}
