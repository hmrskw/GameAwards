﻿using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// *************************************************
/// 制作者 三澤裕樹
/// *************************************************
/// 道中に配置されている花の開花処理
/// *************************************************
/// </summary>
[System.Serializable]
public class Monument : MonoBehaviour {

    [System.Serializable]
    public struct CameraAndMask
    {
        public GameObject camera;
        public Image mask;
    }

    [SerializeField]
    protected GameObject monument;

    [SerializeField]
    public ParticleSystem particle;

    [SerializeField]
    protected GameObject guideObjct;

    [SerializeField]
    protected Monument nextMonument;

    [SerializeField]
    protected float extendLength;

    protected Animator openAnimation;

    protected bool isOn;
    public bool IsOn
    {
        private set { isOn = value; }
        get { return isOn; }
    }

    protected float animationStart;

    void Start ()
    {
        if(guideObjct != null)guideObjct.SetActive(false);
        openAnimation = GetComponent<Animator>();
        isOn = false;

        StartCoroutine(Wait());
	}

    virtual protected IEnumerator Wait()
    {
        while (StringView.Instance.OnHitLine(transform.position) == false)
        {
            yield return null;
        }
        StringView.Instance.GrassTextureUpdate(0);
        SoundManager.Instance.PlaySE("se object");
        StartCoroutine(Boot());
    }

    virtual protected IEnumerator Boot() {
        if (isOn == false)
        {
            isOn = true;
            StringView.Instance.OpenFlowerCount = 1;
            particle.Play();
            yield return new WaitForSeconds(0.5f);
            openAnimation.SetTrigger("Open");
            if(guideObjct != null && nextMonument != null) nextMonument.Guid();
            InputController.ExtendMaxDistanceLength(extendLength);
        }
        else if (guideObjct != null && guideObjct.activeInHierarchy == true)
        {
            guideObjct.SetActive(false);
        }
    }

    public void Guid()
    {
        if(isOn == false && guideObjct.activeInHierarchy == false)
        {
            guideObjct.SetActive(true);
        }
    }

    protected IEnumerator FadeOut(CameraAndMask fadeOut, float time)
    {
        float startTime = Time.timeSinceLevelLoad;
        float diff = Time.timeSinceLevelLoad - startTime;
        Color maskAlpha = new Color(0, 0, 0, 0);

        while (diff < (time))
        {
            diff = Time.timeSinceLevelLoad - startTime;
            maskAlpha.a = diff / (time);
            fadeOut.mask.color = maskAlpha;
            yield return null;
        }
    }

    protected IEnumerator FadeIn(CameraAndMask fadeIn, float time)
    {
        float startTime = Time.timeSinceLevelLoad;
        float diff = Time.timeSinceLevelLoad - startTime;
        Color maskAlpha = new Color(0, 0, 0, 1);

        while (diff < time)
        {
            diff = Time.timeSinceLevelLoad - startTime;
            maskAlpha.a = 1 - (diff / (time));
            fadeIn.mask.color = maskAlpha;
            yield return null;
        }
    }

    protected IEnumerator FadeInFadeOut(CameraAndMask fadeOut, CameraAndMask fadeIn, float time, Action func)
    {
        yield return StartCoroutine(FadeOut(fadeOut, time / 2f));

        fadeOut.camera.SetActive(false);
        if (func != null) func();
        fadeIn.camera.SetActive(true);

        yield return StartCoroutine(FadeIn(fadeIn, time / 2f));
    }

}
