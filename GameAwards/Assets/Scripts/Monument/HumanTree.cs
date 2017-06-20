using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HumanTree : MonoBehaviour
{
    [SerializeField]
    string[] flavorText;
    [SerializeField]
    float drawTime;
    [SerializeField]
    Text textUI;
    [SerializeField]
    float fadeTime;

    bool isDraw = false;

    IEnumerator func = null;
    
    IEnumerator Draw()
    {
        for (int i = 0; i < flavorText.Length; i++)
        {
            textUI.text = flavorText[i];
            yield return StartCoroutine(ShowText(fadeTime));
            yield return new WaitForSeconds(drawTime);
            yield return StartCoroutine(HideText(fadeTime));
        }
    }

    IEnumerator ShowText(float fadeTime)
    {
        float startTime = Time.timeSinceLevelLoad;
        float diff = Time.timeSinceLevelLoad - startTime;
        Color alpha = textUI.color;

        while (diff < (fadeTime))
        {
            diff = Time.timeSinceLevelLoad - startTime;
            alpha.a = diff / (fadeTime);
            textUI.color = alpha;
            yield return null;
        }
    }

    IEnumerator HideText(float fadeTime)
    {
        float startTime = Time.timeSinceLevelLoad;
        float diff = Time.timeSinceLevelLoad - startTime;
        Color alpha = textUI.color;

        while (diff < fadeTime)
        {
            diff = Time.timeSinceLevelLoad - startTime;
            alpha.a = 1 - (diff / (fadeTime));
            textUI.color = alpha;
            yield return null;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (isDraw == false && col.gameObject.tag == "Player")
        {
            isDraw = true;
            func = Draw();
            StartCoroutine(func);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (isDraw == true && col.gameObject.tag == "Player")
        {
            isDraw = false;
            StopCoroutine(func);
            func = null;
            StartCoroutine(HideText(fadeTime));
        }
    }
}