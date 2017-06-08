using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CutManager : Monument
{
    enum CUT
    {
        Cut1, Cut2, Cut3, Cut4
    }
    [SerializeField]
    CUT cut;

    [System.Serializable]
    public struct ZoomData
    {
        public int startFrame;
        public float frameLength;
        public float targetFoV;
    }

    [System.Serializable]
    public struct CameraAndMask
    {
        public GameObject camera;
        public Image mask;
    }
    [Space(15)]
    [SerializeField]
    CameraAndMask MainCamera;

    [SerializeField]
    CameraAndMask CutSceneCamera;

    //[SerializeField, Tooltip("カメラはこのオブジェクトの方向を見続ける")]
    //Transform targetTransform;

    [SerializeField]
    GameObject playerCharacter;
    [SerializeField]
    GameObject cutSceneP1;
    [SerializeField]
    GameObject cutSceneP2;
    //GameObject cutSceneCharacters;
    [SerializeField]
    Animator cutAnim;
    [SerializeField]
    ZoomData[] zoomData;
    [SerializeField]
    AnimationCurve zoomCurve;

    bool isPlayCutScene = false;
    public bool IsPlayCutScene
    {
        set { isPlayCutScene = value; }
        get { return isPlayCutScene; }
    }

    [SerializeField]
    Camera cutCamera;

    void StartCutScene()
    {
        if (isPlayCutScene == false)
        {
            StartCoroutine(Task());
        }
    }

    override protected IEnumerator Wait()
    {
        while (StringView.Instance.OnHitLine(transform.position) == false)
        {
            yield return null;
        }

        StringView.Instance.GrassTextureUpdate(1);
        SoundManager.Instance.PlaySE("se object");
        if (isPlayCutScene == false) StartCutScene();
    }

    IEnumerator Task()
    {

        yield return StartCoroutine(FadeInFadeOut(MainCamera, CutSceneCamera, 1.0f));
        /*yield return*/
        yield return StartCoroutine(Anim());
        yield return StartCoroutine(FadeInFadeOut(CutSceneCamera, MainCamera, 1.0f));
    }

    IEnumerator FadeInFadeOut(CameraAndMask fadeIn, CameraAndMask fadeOut, float time)
    {
        float startTime = Time.timeSinceLevelLoad;
        float diff = Time.timeSinceLevelLoad - startTime;
        Color maskAlpha = new Color(0, 0, 0, 0);

        while (diff < (time / 2f))
        {
            diff = Time.timeSinceLevelLoad - startTime;
            maskAlpha.a = diff / (time / 2);
            fadeIn.mask.color = maskAlpha;
            yield return null;
        }
        fadeIn.camera.SetActive(false);

        playerCharacter.SetActive(!playerCharacter.activeInHierarchy);
        //cutSceneCharacters.SetActive(!cutSceneCharacters.activeInHierarchy);
        cutSceneP1.SetActive(!cutSceneP1.activeInHierarchy);
        cutSceneP2.SetActive(!cutSceneP2.activeInHierarchy);

        isPlayCutScene = !isPlayCutScene;
        fadeOut.camera.SetActive(true);
        //CutSceneCamera.camera.transform.LookAt(targetTransform);

        startTime = Time.timeSinceLevelLoad;
        diff = Time.timeSinceLevelLoad - startTime;
        while (diff < time / 2)
        {
            diff = Time.timeSinceLevelLoad - startTime;
            maskAlpha.a = 1 - (diff / (time / 2));
            fadeOut.mask.color = maskAlpha;
            yield return null;
        }
    }

    protected override IEnumerator Boot()
    {
        if (isOn == false)
        {
            isOn = true;
            cutAnim.SetTrigger("Open");
            if (guideObjct != null && nextMonument != null) nextMonument.Guid();
            InputController.ExtendMaxDistanceLength(extendLength);
        }
        else if (guideObjct != null && guideObjct.activeInHierarchy == true)
        {
            guideObjct.SetActive(false);
        }
        
        while (
            openAnimation.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash("New State") ||
            openAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime < (40f / 45f))//発生させたいフレーム/アニメーションの総フレーム数
        {
            yield return null;
        }
        particle.Play();
    }

    IEnumerator Zoom(int index)
    {
        float startFov = cutCamera.fieldOfView;
        float startTime = Time.timeSinceLevelLoad;
        float diff = (Time.timeSinceLevelLoad - startTime) * 30f;
        float fov;

        while (diff < zoomData[index].frameLength)
        {
            diff = (Time.timeSinceLevelLoad - startTime) * 30f;
            fov = zoomCurve.Evaluate(diff / zoomData[index].frameLength);
            cutCamera.fieldOfView = Mathf.Lerp(startFov, zoomData[index].targetFoV, fov);

            yield return null;
        }

    }

    IEnumerator Anim()
    {
        yield return StartCoroutine(Boot());

        while (openAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime - animationStart < 1 || particle.isPlaying)
        {
            yield return null;
        }
    }
}
