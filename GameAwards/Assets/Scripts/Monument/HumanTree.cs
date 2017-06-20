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
        Debug.Log("en" + col.gameObject.tag);
        if (StringView.Instance.IsDraw == false && col.gameObject.tag == "Midpoint")
        {
            StringView.Instance.IsDraw = true;
            func = Draw();
            StartCoroutine(func);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (StringView.Instance.IsDraw == true && col.gameObject.tag == "Midpoint")
        {
            StringView.Instance.IsDraw = false;
            StopCoroutine(func);
            func = null;
            func = Draw();
            StartCoroutine(HideText(fadeTime));
        }
    }
}