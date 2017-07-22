using UnityEngine;
using System.Collections;
/// <summary>
/// *************************************************
/// 制作者 三澤裕樹
/// *************************************************
/// Awakeより前に自動でマネージャーを作成するクラス
/// *************************************************
/// </summary>
public class ManageCreator : MonoBehaviour {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreatorManager()
    {
        Cursor.visible = false;
        GameObject SoundManager = Instantiate((GameObject)Resources.Load("SoundManager/SoundManager"));
        DontDestroyOnLoad(SoundManager);
        GameObject FadeManager = Instantiate((GameObject)Resources.Load("FadeManager/FadeManager"));
    }
}
