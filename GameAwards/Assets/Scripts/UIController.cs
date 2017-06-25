﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour {

    [System.Serializable]
    public struct ControllerUI
    {
        public Image controller;
        public Sprite[] sprites;
        //public Image word;
    };

    [System.Serializable]
    public struct UI {
        public Image[] back;
        public ControllerUI[] controllerUI;
    };

    [SerializeField]
    UI wordUI;

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

    public bool isDrawUI;

    bool isWaitJump = false;

    void Start()
    {
        for (int j = 0; j < moveAndRotateUI.Length; j++)
        {
            for (int i = 0; i < moveAndRotateUI[j].back.Length; i++)
            {
                moveAndRotateUI[j].back[i].color = new Color(moveAndRotateUI[j].back[i].color.r, moveAndRotateUI[j].back[i].color.g, moveAndRotateUI[j].back[i].color.b, 0);
            }
            for (int i = 0; i < moveAndRotateUI[j].controllerUI.Length; i++)
            {
                moveAndRotateUI[j].controllerUI[i].controller.color = 
                    new Color(
                        moveAndRotateUI[j].controllerUI[i].controller.color.r,
                        moveAndRotateUI[j].controllerUI[i].controller.color.g,
                        moveAndRotateUI[j].controllerUI[i].controller.color.b,
                        0);
            }
        }

        for (int i = 0; i < jumpUI.back.Length; i++)
        {
            jumpUI.back[i].color = new Color(jumpUI.back[i].color.r, jumpUI.back[i].color.g, jumpUI.back[i].color.b, 0);
        }
        for (int i = 0; i < jumpUI.controllerUI.Length; i++)
        {
            jumpUI.controllerUI[i].controller.color = 
                new Color(
                    jumpUI.controllerUI[i].controller.color.r,
                    jumpUI.controllerUI[i].controller.color.g,
                    jumpUI.controllerUI[i].controller.color.b,
                    0);
        }

        for (int i = 0; i < wordUI.back.Length; i++)
        {
            wordUI.back[i].color = new Color(wordUI.back[i].color.r, wordUI.back[i].color.g, wordUI.back[i].color.b, 1);
        }
        for (int i = 0; i < wordUI.controllerUI.Length; i++)
        {
            wordUI.controllerUI[i].controller.color = 
                new Color(
                    wordUI.controllerUI[i].controller.color.r,
                    wordUI.controllerUI[i].controller.color.g,
                    wordUI.controllerUI[i].controller.color.b,
                    1);
        }
        isDrawUI = true;
        StartCoroutine(DrawUI());
    }
    
    IEnumerator DrawUI()
    {
        while (SceneManager.GetActiveScene().buildIndex != 1)
        {
            yield return null;
        }
        yield return new WaitForSeconds(drawTime);
        yield return StartCoroutine(UIFadeOut(wordUI.controllerUI, wordUI.back, 1));

        isDrawUI = false;

        for (int i = 0; i < moveAndRotateUI.Length; i++) {
            yield return StartCoroutine(UIFadeIn(moveAndRotateUI[i].controllerUI, moveAndRotateUI[i].back, 1));

            //なんだこのコード・・・
            for (int j = 0; j < drawTime / 0.25f; j++)
            {
                for (int k = 0; k < moveAndRotateUI[i].controllerUI.Length; k++)
                {
                    moveAndRotateUI[i].controllerUI[k].controller.sprite = moveAndRotateUI[i].controllerUI[k].sprites[j % moveAndRotateUI[i].controllerUI[k].sprites.Length];
                }
                yield return new WaitForSeconds(0.25f);
            }

            yield return StartCoroutine(UIFadeOut(moveAndRotateUI[i].controllerUI, moveAndRotateUI[i].back, 1));
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
        yield return StartCoroutine(UIFadeIn(jumpUI.controllerUI, jumpUI.back, 2));
        StartCoroutine(WaitJump());
        while (isWaitJump == false)
        {
            //なんだこのコード・・・
            for (int j = 0; j < drawTime / 0.25f; j++)
            {
                for (int k = 0; k < jumpUI.controllerUI.Length; k++)
                {
                    jumpUI.controllerUI[k].controller.sprite = jumpUI.controllerUI[k].sprites[j % jumpUI.controllerUI[k].sprites.Length];
                }
                yield return new WaitForSeconds(0.25f);
            }
        }
        yield return new WaitForSeconds(drawTime/2);
        yield return StartCoroutine(UIFadeOut(jumpUI.controllerUI, jumpUI.back, 2));
    }

    IEnumerator WaitJump() {
        while (isWaitJump == false) {
            isWaitJump = Input.GetButtonDown("LeftJump") || Input.GetButtonDown("RightJump");
            yield return null;
        }
    }
    
    IEnumerator UIFadeIn(ControllerUI[] controllerImages, Image[] backImages, float fadeTime)
    {
        float startTime = Time.timeSinceLevelLoad;
        float diff = Time.timeSinceLevelLoad - startTime;
        Color[] controllersImageAlpha = new Color[controllerImages.Length];
        for (int i = 0; i < controllerImages.Length; i++)
        {
            controllersImageAlpha[i] = controllerImages[i].controller.color;
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
                controllerImages[i].controller.color = controllersImageAlpha[i];
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

    IEnumerator UIFadeOut(ControllerUI[] controllerImages, Image[] backImages, float fadeTime)
    {
        float startTime = Time.timeSinceLevelLoad;
        float diff = Time.timeSinceLevelLoad - startTime;
        Color[] controllersImageAlpha = new Color[controllerImages.Length];
        for (int i = 0; i < controllerImages.Length; i++)
        {
            controllersImageAlpha[i] = controllerImages[i].controller.color;
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
                controllerImages[i].controller.color = controllersImageAlpha[i];
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
