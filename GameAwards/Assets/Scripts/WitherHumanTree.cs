using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitherHumanTree : MonoBehaviour {

    [SerializeField]
    Monument monument;

    [SerializeField]
    Texture growTexture;
    
    [SerializeField]
    Texture witherTexture;

    [SerializeField]
    Material mat;

    // Use this for initialization
    IEnumerator Start () {
        mat.mainTexture = growTexture;
        while (monument.IsOn == false || StringView.Instance.isPlayCutScene == true) {
            yield return null;
        }
        mat.mainTexture = witherTexture;
	}
}
