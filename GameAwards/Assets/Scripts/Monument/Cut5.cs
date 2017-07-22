﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cut5 : Monument {
    [SerializeField]
    CameraAndMask MainCamera;

    [SerializeField]
    CameraAndMask CutSceneCamera;

    [SerializeField]
    AnimationCurve curve;
    [SerializeField]
    float cameraMoveTime;
    
    [SerializeField, Tooltip("カメラはこのオブジェクトの方向を見続ける")]
    Transform cameraTargetTransform;
    [SerializeField, Tooltip("演出で移動する敵")]
    GameObject moveEnemy;
    [SerializeField, Tooltip("演出で移動する敵の移動先")]
    Transform moveEnemyTarget;

    [SerializeField]
    GameObject[] Characters;

    [SerializeField]
    Player p1;
    [SerializeField]
    Player p2;
    [SerializeField]
    int enemyTargetValue;

    [Space(15)]
    [SerializeField]
    CameraAndMask GoalSceneCamera;
    [SerializeField]
    ParticleSystem wind;

    bool isEvent = false;

    void StartCutScene()
    {
        if (StringView.Instance.isPlayCutScene == false)
        {
            StartCoroutine(Task());
        }
    }

    override protected IEnumerator Wait()
    {
        yield return null;
        //while (StringView.Instance.OnHitLine(transform.position) == false)
        //{
        //    yield return null;
        //}
        //if (SoundManager.Instance.IsPlayBGM("asioto") == true)
        //{
        //    SoundManager.Instance.StopBGM("asioto");
        //}

        //StringView.Instance.GrassTextureUpdate(0);
        //if (StringView.Instance.isPlayCutScene == false) StartCutScene();
    }

    IEnumerator Task()
    {
        int initialDestroyEnemyCount = StringView.Instance.DestroyEnemyCount;
        yield return StartCoroutine(FadeInFadeOut(MainCamera, CutSceneCamera, 1.0f, ChangePlayer));
        //wall.SetActive(true);
        yield return StartCoroutine(MoveCamera());

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(FadeInFadeOut(CutSceneCamera, MainCamera, 1.0f, ChangePlayer));

        while (StringView.Instance.DestroyEnemyCount -initialDestroyEnemyCount < enemyTargetValue)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(FadeInFadeOut(MainCamera, CutSceneCamera, 1.0f, ()=> {
            guideObjct.SetActive(false);
            ChangePlayer();
        }));
        //isOn = false;
        //yield return StartCoroutine(MoveCamera());
        yield return StartCoroutine(FlowerAnim());
        //wall.SetActive(false);
        yield return StartCoroutine(FadeInFadeOut(CutSceneCamera, GoalSceneCamera, 1.0f,null));
        SoundManager.Instance.PlaySE("wind");
        wind.Stop();
        yield return new WaitForSeconds(5f);
        yield return StartCoroutine(FadeInFadeOut(GoalSceneCamera, MainCamera, 1.0f, ()=> {
            StringView.Instance.GrassTextureUpdate(1);
            ChangePlayer();
        }));
    }

    void ChangePlayer()
    {
        StringView.Instance.cutP1 = p1.transform;
        StringView.Instance.cutP2 = p2.transform;
        StringView.Instance.isPlayCutScene = !StringView.Instance.isPlayCutScene;

        CutSceneCamera.camera.transform.LookAt(cameraTargetTransform);
    }

    /*IEnumerator FadeInFadeOut(CameraAndMask fadeIn, CameraAndMask fadeOut, float time)
    {
        float startTime = Time.timeSinceLevelLoad;
        float diff = Time.timeSinceLevelLoad - startTime;
        Color maskAlpha = new Color(0, 0, 0, 0);

        while (diff < (time / 2f))
        {
            diff = Time.timeSinceLevelLoad - startTime;
            maskAlpha.a = diff / (time / 2f);
            fadeIn.mask.color = maskAlpha;
            yield return null;
        }
        fadeIn.camera.SetActive(false);

        StringView.Instance.cutP1 = p1.transform;
        StringView.Instance.cutP2 = p2.transform;
        StringView.Instance.isPlayCutScene = !StringView.Instance.isPlayCutScene;

        guideObjct.SetActive(false);
        fadeOut.camera.SetActive(true);
        CutSceneCamera.camera.transform.LookAt(cameraTargetTransform);

        startTime = Time.timeSinceLevelLoad;
        diff = Time.timeSinceLevelLoad - startTime;
        while (diff < time / 2f)
        {
            diff = Time.timeSinceLevelLoad - startTime;
            maskAlpha.a = 1 - (diff / (time / 2f));
            fadeOut.mask.color = maskAlpha;
            yield return null;
        }
    }*/

    IEnumerator MoveCamera()
    {
        float startTime = Time.timeSinceLevelLoad;
        Vector3 startPosition = moveEnemy.transform.position;
        float diff = Time.timeSinceLevelLoad - startTime;
        float pos;

        while (diff < cameraMoveTime)
        {
            diff = Time.timeSinceLevelLoad - startTime;
            pos = curve.Evaluate(diff / cameraMoveTime);

            CutSceneCamera.camera.transform.LookAt(cameraTargetTransform);
            moveEnemy.transform.position = Vector3.Lerp(startPosition, moveEnemyTarget.position, pos);

            yield return null;
        }
    }

    IEnumerator FlowerAnim()
    {
        Destroy(moveEnemy);
        yield return StartCoroutine(Boot());

        while (openAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime - animationStart < 1 || particle.isPlaying)
        {
            yield return null;
        }
    }

    override protected IEnumerator Boot()
    {
        if (isOn == false)
        {
            isOn = true;
            isEvent = false;
            yield return new WaitForSeconds(0.5f);
            openAnimation.SetTrigger("Open");
            //if (guideObjct != null && nextMonument != null) nextMonument.Guid();
            InputController.ExtendMaxDistanceLength(extendLength);
        }
        else if (guideObjct != null && guideObjct.activeInHierarchy == true)
        {
            guideObjct.SetActive(false);
        }

        while (
            openAnimation.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash("New State") ||
            openAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime < (30f / 45f))
        {
            yield return null;
        }

        SoundManager.Instance.PlaySE("se object");
        particle.Play();
    }

    void OnTriggerEnter(Collider col)
    {
        if (isOn == false && isEvent == false && StringView.Instance.isPlayCutScene == false)
        {
            if (col.gameObject.tag == "Player")
            {
                if (SoundManager.Instance.IsPlayBGM("asioto") == true)
                {
                    SoundManager.Instance.StopBGM("asioto");
                }
                //isOn = true;
                isEvent = true;
                StartCutScene();
            }
        }
    }
}
