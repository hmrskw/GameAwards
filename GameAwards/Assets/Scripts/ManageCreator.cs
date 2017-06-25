using UnityEngine;
using System.Collections;

public class ManageCreator : MonoBehaviour {

    //Awakeより前に自動でマネージャーを作成する
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreatorManager()
    {
        Cursor.visible = false;
        GameObject SoundManager = Instantiate((GameObject)Resources.Load("SoundManager/SoundManager"));
        DontDestroyOnLoad(SoundManager);
        GameObject FadeManager = Instantiate((GameObject)Resources.Load("FadeManager/FadeManager"));
    }
}
