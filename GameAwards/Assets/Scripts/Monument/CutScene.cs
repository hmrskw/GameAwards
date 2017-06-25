using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CutScene : Monument
{
    [System.Serializable]
    public struct CheckPointCut
    {
        public CameraAndMask cutCamera;
        public Monument guids;
    }

    [Space(15)]
    [SerializeField]
    CameraAndMask MainCamera;

    [SerializeField]
    CameraAndMask CutSceneCamera;

    [SerializeField]
    AnimationCurve curve;
    [SerializeField]
    float cameraMoveTime;
    [SerializeField]
    float characterMoveSpeed;

    [SerializeField,Tooltip("カメラの移動先")]
    Transform nextCameraPositionTransform;
    [SerializeField,Tooltip("カメラはこのオブジェクトの方向を見続ける")]
    Transform targetTransform;

    [SerializeField]
    Player p1;
    [SerializeField]
    Player p2;

    [SerializeField]
    GameObject[] playerCharacters;
    [SerializeField]
    GameObject cutSceneCharacters;
    [SerializeField]
    Transform cutSceneCharactersInitPosition;

    [SerializeField]
    CheckPointCut[] CheckPoints;

    [SerializeField]
    ParticleSystem wind;
    void Start()
    {
        openAnimation = GetComponent<Animator>();
        isOn = false;

        StartCoroutine(Wait());
    }

    void StartCutScene()
    {
        if (StringView.Instance.isPlayCutScene == false)
        {
            StartCoroutine(Task_op());
        }
    }

    override protected IEnumerator Wait()
    {
        while (
            Vector3.Distance(playerCharacters[0].transform.position, transform.position) > 30f&&
            Vector3.Distance(playerCharacters[1].transform.position, transform.position) > 30f
            )
        //while (StringView.Instance.OnHitLine(transform.position) == false)
        {
            yield return null;
        }

        StringView.Instance.GrassTextureUpdate(0);
        if (StringView.Instance.isPlayCutScene == false) StartCutScene();
    }

    IEnumerator Task_op()
    {
        yield return StartCoroutine(FadeInFadeOut(MainCamera, CutSceneCamera,1.0f,
            () => {
                for (int i = 0; i < playerCharacters.Length; i++)
                {
                    playerCharacters[i].SetActive(false);
                }
                cutSceneCharacters.SetActive(!cutSceneCharacters.activeInHierarchy);
                if (cutSceneCharacters.activeInHierarchy == true)
                {
                    cutSceneCharacters.transform.position = cutSceneCharactersInitPosition.position;
                    cutSceneCharacters.transform.rotation = cutSceneCharactersInitPosition.rotation;
                }

                StringView.Instance.cutP1 = p1.transform;
                StringView.Instance.cutP2 = p2.transform;
                StringView.Instance.isPlayCutScene = !StringView.Instance.isPlayCutScene;
                if (SoundManager.Instance.IsPlayBGM("asioto") == true)
                {
                    SoundManager.Instance.StopBGM("asioto");
                }
                CutSceneCamera.camera.transform.LookAt(targetTransform);
            }
        ));

        p1.transform.localPosition = new Vector3(-4f, 0f, 0f);
        p2.transform.localPosition = new Vector3(4f, 0f, 0f);
        StartCoroutine(MoveCharacter());
        yield return StartCoroutine(MoveCamera());
        yield return StartCoroutine(FlowerAnim());
        StartCoroutine(RotateCamera(10,2));
        wind.Stop();
        yield return new WaitForSeconds(3f);

        //他の花を映す
        if (CheckPoints.Length > 0)
        {
            yield return StartCoroutine(FadeInFadeOut(CutSceneCamera, CheckPoints[0].cutCamera, 1.0f,()=> {
                if (CheckPoints[0].guids != null) CheckPoints[0].guids.Guid();
            }));

            for (int i = 1; i < CheckPoints.Length; i++)
            {
                yield return new WaitForSeconds(1.0f);
                yield return StartCoroutine(FadeInFadeOut(CheckPoints[i - 1].cutCamera, CheckPoints[i].cutCamera, 1.0f,()=> {
                    if (CheckPoints[i].guids != null) CheckPoints[i].guids.Guid();
                }));
            }

            if (CheckPoints[CheckPoints.Length - 1].guids != null) CheckPoints[CheckPoints.Length - 1].guids.Guid();

            yield return new WaitForSeconds(3.0f);
            yield return StartCoroutine(FadeInFadeOut(CheckPoints[CheckPoints.Length - 1].cutCamera, MainCamera, 3.0f, () => {
                for (int i = 0; i < playerCharacters.Length; i++)
                {
                    playerCharacters[i].SetActive(true);
                }
                cutSceneCharacters.SetActive(!cutSceneCharacters.activeInHierarchy);
                if (cutSceneCharacters.activeInHierarchy == true)
                {
                    cutSceneCharacters.transform.position = cutSceneCharactersInitPosition.position;
                    cutSceneCharacters.transform.rotation = cutSceneCharactersInitPosition.rotation;
                }
                guideObjct.SetActive(false);

                StringView.Instance.cutP1 = p1.transform;
                StringView.Instance.cutP2 = p2.transform;
                StringView.Instance.isPlayCutScene = !StringView.Instance.isPlayCutScene;
            }));
        }
        else
        {
            yield return StartCoroutine(FadeInFadeOut(CutSceneCamera, MainCamera, 1.0f,
                () => {
                    for (int i = 0; i < playerCharacters.Length; i++)
                    {
                        playerCharacters[i].SetActive(true);
                    }
                    cutSceneCharacters.SetActive(!cutSceneCharacters.activeInHierarchy);
                    if (cutSceneCharacters.activeInHierarchy == true)
                    {
                        cutSceneCharacters.transform.position = cutSceneCharactersInitPosition.position;
                        cutSceneCharacters.transform.rotation = cutSceneCharactersInitPosition.rotation;
                    }
                    guideObjct.SetActive(false);

                    StringView.Instance.cutP1 = p1.transform;
                    StringView.Instance.cutP2 = p2.transform;
                    StringView.Instance.isPlayCutScene = !StringView.Instance.isPlayCutScene;
                }
            ));
        }
    }

    IEnumerator RotateCamera(float angle ,float time)
    {
        float startTime = Time.timeSinceLevelLoad;
        float diff = Time.timeSinceLevelLoad - startTime;

        while (diff < (time))
        {
            diff = Time.timeSinceLevelLoad - startTime;

            CutSceneCamera.camera.transform.localRotation = 
                Quaternion.Lerp(CutSceneCamera.camera.transform.localRotation,
                Quaternion.Euler(new Vector3(CutSceneCamera.camera.transform.localRotation.x - angle, CutSceneCamera.camera.transform.localRotation.y + 180, CutSceneCamera.camera.transform.localRotation.z)),
                diff / (time));
            yield return null;
        }
    }
    /*
    IEnumerator FadeOut(CameraAndMask fadeOut, float time)
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

    IEnumerator FadeIn(CameraAndMask fadeIn, float time)
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

    IEnumerator FadeInFadeOut(CameraAndMask fadeOut, CameraAndMask fadeIn,float time,Action func)
    {
        yield return StartCoroutine(FadeOut(fadeOut,time/2f));

        fadeOut.camera.SetActive(false);
        if (func != null)func();
        fadeIn.camera.SetActive(true);

        yield return StartCoroutine(FadeIn(fadeIn, time / 2f));
    }*/

    IEnumerator MoveCamera()
    {
        float startTime = Time.timeSinceLevelLoad;
        Vector3 startPosition = CutSceneCamera.camera.transform.position;
        float diff = Time.timeSinceLevelLoad - startTime;
        float pos;

        while (diff < cameraMoveTime)
        {
            diff = Time.timeSinceLevelLoad - startTime;

            pos = curve.Evaluate(diff / cameraMoveTime);

            CutSceneCamera.camera.transform.LookAt(targetTransform);
            CutSceneCamera.camera.transform.position = Vector3.Lerp(startPosition, nextCameraPositionTransform.position, pos);

            yield return null;
        }
    }

    IEnumerator FlowerAnim()
    {
        //SoundManager.Instance.PlaySE("se object");
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
        particle.Play();
        yield return new WaitForSeconds(0.5f);
        SoundManager.Instance.PlaySE("se object");
    }

    IEnumerator MoveCharacter()
    {
        p1.SetCharacterMoveDirection(new Vector3(characterMoveSpeed, 0, 0));
        p2.SetCharacterMoveDirection(new Vector3(characterMoveSpeed, 0, 0));
        yield return new WaitForSeconds(2f);
        p1.SetCharacterMoveDirection(new Vector3(0, 0, 0));
        p2.SetCharacterMoveDirection(new Vector3(0, 0, 0));
    }
}
