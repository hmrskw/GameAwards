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
    UI[] moveAndRotateUI;

    [SerializeField]
    UI jumpUI;

    [SerializeField]
    Transform[] characters;

    [SerializeField]
    Transform[] highFlowers;

    [SerializeField]
    float drawJumpUIDistance;

    [SerializeField]
    AnimationCurve curve;

    [SerializeField]
    float drawTime;

    void Start()
    {
        for (int j = 0; j < moveAndRotateUI.Length; j++)
        {
            for (int i = 0; i < moveAndRotateUI[j].back.Length; i++)
            {
                moveAndRotateUI[j].back[i].color = new Color(moveAndRotateUI[j].back[i].color.r, moveAndRotateUI[j].back[i].color.g, moveAndRotateUI[j].back[i].color.b, 0);
            }
            for (int i = 0; i < moveAndRotateUI[j].controller.Length; i++)
            {
                moveAndRotateUI[j].controller[i].color = new Color(moveAndRotateUI[j].controller[i].color.r, moveAndRotateUI[j].controller[i].color.g, moveAndRotateUI[j].controller[i].color.b, 0);
            }
        }

        for (int i = 0; i < jumpUI.back.Length; i++)
        {
            jumpUI.back[i].color = new Color(jumpUI.back[i].color.r, jumpUI.back[i].color.g, jumpUI.back[i].color.b, 0);
        }
        for (int i = 0; i < jumpUI.controller.Length; i++)
        {
            jumpUI.controller[i].color = new Color(jumpUI.controller[i].color.r, jumpUI.controller[i].color.g, jumpUI.controller[i].color.b, 0);
        }

        StartCoroutine(DrawUI());
    }
    
    IEnumerator DrawUI()
    {
        while (SceneManager.GetActiveScene().buildIndex != 1)
        {
            yield return null;
        }

        for (int i = 0; i < moveAndRotateUI.Length; i++) {
            yield return StartCoroutine(UIFadeIn(moveAndRotateUI[i].controller, moveAndRotateUI[i].back, 1));
            yield return new WaitForSeconds(drawTime);
            yield return StartCoroutine(UIFadeOut(moveAndRotateUI[i].controller, moveAndRotateUI[i].back, 1));
        }


        float dis = Vector2.Distance(new Vector2(highFlowers[0].position.x, highFlowers[0].position.z), new Vector2(characters[0].position.x, characters[0].position.z));
        bool inLength = false;

        for (int i = 0; i < characters.Length; i++)
        {
            for (int j = 0; j < highFlowers.Length; j++)
            {
                dis = Vector2.Distance(new Vector2(highFlowers[j].position.x, highFlowers[j].position.z), new Vector2(characters[i].position.x, characters[i].position.z));
                inLength |= (dis < drawJumpUIDistance);
            }
        }

        while (inLength == false)
        {
            for(int i = 0;i<characters.Length ; i++)
            {
                for (int j = 0; j < highFlowers.Length; j++)
                {
                    dis = Vector2.Distance(new Vector2(highFlowers[j].position.x, highFlowers[j].position.z), new Vector2(characters[i].position.x, characters[i].position.z));
                    inLength |= (dis < drawJumpUIDistance);
                }
            }
            yield return null;
        }
        yield return StartCoroutine(UIFadeIn(jumpUI.controller, jumpUI.back, 2));
        while (Input.GetButtonDown("LeftJump") || Input.GetButtonDown("RightJump"))
        {
            yield return null;
        }
        yield return new WaitForSeconds(drawTime/2);
        yield return StartCoroutine(UIFadeOut(jumpUI.controller, jumpUI.back, 2));
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
