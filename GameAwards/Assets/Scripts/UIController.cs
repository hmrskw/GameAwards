using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour {

    [System.Serializable]
    public struct UI {
        public Image[] back;
        public Image[] controller;
        //public Image word;
    };

    [SerializeField]
    UI[] ui;

    [SerializeField]
    AnimationCurve curve;

    void Start()
    {
        for (int j = 0; j < ui.Length; j++)
        {
            for (int i = 0; i < ui[j].back.Length; i++)
            {
                ui[j].back[i].color = new Color(ui[j].back[i].color.r, ui[j].back[i].color.g, ui[j].back[i].color.b, 0);
            }
            for (int i = 0; i < ui[j].controller.Length; i++)
            {
                ui[j].controller[i].color = new Color(ui[j].controller[i].color.r, ui[j].controller[i].color.g, ui[j].controller[i].color.b, 0);
            }
        }
        StartCoroutine(DrawUI());
    }
    
    IEnumerator DrawUI()
    {
        while (SceneManager.GetActiveScene().buildIndex != 1)
        {
            yield return null;
        }
        for (int i = 0; i < ui.Length; i++) {
            yield return StartCoroutine(UIFadeIn(ui[i].controller, ui[i].back, 2));
            yield return new WaitForSeconds(10f);
            yield return StartCoroutine(UIFadeOut(ui[i].controller, ui[i].back, 2));
        }
    }

    IEnumerator UIFadeIn(Image[] controllerImages, Image[] backImages, float fadeTime)
    {
        float startTime = Time.timeSinceLevelLoad;
        float diff = Time.timeSinceLevelLoad - startTime;
        Color[] controllersImageAlpha = new Color[controllerImages.Length];
        for (int i = 0; i < controllerImages.Length; i++)
        {
            controllersImageAlpha[i] = controllerImages[i].color;
        }
        Color[] backImagesAlpha = new Color[backImages.Length];
        for (int i = 0; i < backImages.Length; i++)
        {
            backImagesAlpha[i] = backImages[i].color;
        }
        float rate;

        while (diff < (fadeTime))
        {
            diff = Time.timeSinceLevelLoad - startTime;
            rate = curve.Evaluate(diff / fadeTime);
            for (int i = 0; i < controllerImages.Length; i++)
            {
                controllersImageAlpha[i].a = Mathf.Lerp(controllersImageAlpha[i].a, 1, rate);
                //alpha.a = diff / (fadeTime);
                controllerImages[i].color = controllersImageAlpha[i];
            }
            for (int i = 0; i < backImages.Length; i++)
            {
                backImagesAlpha[i].a = Mathf.Lerp(backImagesAlpha[i].a, 1, rate);
                //alpha.a = diff / (fadeTime);
                backImages[i].color = backImagesAlpha[i];
            }
            /*
            for (int i = 0; i < Images.Length; i++)
            {
                alpha[i].a = Mathf.Lerp(alpha[i].a, 1, rate);
                //alpha.a = diff / (fadeTime);
                Images[i].color = alpha[i];
            }*/
            yield return null;
        }
    }

    IEnumerator UIFadeOut(Image[] controllerImages, Image[] backImages, float fadeTime)
    {
        float startTime = Time.timeSinceLevelLoad;
        float diff = Time.timeSinceLevelLoad - startTime;
        Color[] controllersImageAlpha = new Color[controllerImages.Length];
        for (int i = 0; i < controllerImages.Length; i++)
        {
            controllersImageAlpha[i] = controllerImages[i].color;
        }
        Color[] backImagesAlpha = new Color[backImages.Length];
        for (int i = 0; i < backImages.Length; i++)
        {
            backImagesAlpha[i] = backImages[i].color;
        }
        float rate;

        while (diff < fadeTime)
        {
            diff = Time.timeSinceLevelLoad - startTime;
            rate = curve.Evaluate((diff / (fadeTime)));

            for (int i = 0; i < controllerImages.Length; i++)
            {
                controllersImageAlpha[i].a = Mathf.Lerp(controllersImageAlpha[i].a, 0, rate);
                //alpha.a = diff / (fadeTime);
                controllerImages[i].color = controllersImageAlpha[i];
            }
            for (int i = 0; i < backImages.Length; i++)
            {
                backImagesAlpha[i].a = Mathf.Lerp(backImagesAlpha[i].a, 0, rate);
                //alpha.a = diff / (fadeTime);
                backImages[i].color = backImagesAlpha[i];
            }
            yield return null;
        }
    }
}
