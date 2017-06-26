using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour {
    [SerializeField]
    Color activeColor;

    [SerializeField]
    Color inactiveColor;

    [SerializeField]
    float fadeTime;
    [SerializeField]
    AnimationCurve curve;

    [SerializeField]
    Monument monument;

    [SerializeField]
    Material mat;

    [SerializeField]
    Material emblemMat;

    [SerializeField]
    GameObject windObj;

    //IEnumerator func;

    // Use this for initialization
    void Start () {
        mat.color = inactiveColor;
    }

    void Update()
    {
        if (monument.IsOn == false && (windObj == null || windObj.activeInHierarchy == false))
        {
            emblemMat.color = new Color(emblemMat.color.r, emblemMat.color.g, emblemMat.color.b, 1);
        }
        else
        {
            emblemMat.color = new Color(emblemMat.color.r, emblemMat.color.g, emblemMat.color.b, 0);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (windObj == null || windObj.activeInHierarchy == false)
        {
            if (monument.IsOn == false && col.gameObject.tag == "Midpoint")
            {
                StartCoroutine(ArrowFadeIn(fadeTime));
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (windObj == null || windObj.activeInHierarchy == false)
        {
            if (col.gameObject.tag == "Midpoint")
            {
                StopCoroutine(ArrowFadeIn(fadeTime));
                StartCoroutine(ArrowFadeOut(fadeTime));
            }
        }
    }
    
    IEnumerator ArrowFadeIn(float time)
    {
        float startTime = Time.timeSinceLevelLoad;
        float diff = Time.timeSinceLevelLoad - startTime;
        float rate;

        while (diff < (time))
        {
            diff = Time.timeSinceLevelLoad - startTime;
            rate = curve.Evaluate(diff / time);

            mat.color = Color.Lerp(inactiveColor,activeColor,rate);
            yield return null;
        }
    }

    IEnumerator ArrowFadeOut(float time)
    {
        float startTime = Time.timeSinceLevelLoad;
        float diff = Time.timeSinceLevelLoad - startTime;
        float rate;
        Color initColor = mat.color;

        while (diff < fadeTime)
        {
            diff = Time.timeSinceLevelLoad - startTime;
            rate = curve.Evaluate(1 - (diff / (fadeTime)));
            mat.color = Color.Lerp(inactiveColor, initColor, rate);
            yield return null;
        }
    }
}
