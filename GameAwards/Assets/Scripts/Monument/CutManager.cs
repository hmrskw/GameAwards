using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CutManager : Monument
{
    public enum CUT
    {
        Cut2, Cut3, Cut4
    }
    [SerializeField]
    public CUT cut;

    [System.Serializable]
    struct PaticlesTimeing
    {
        public float totalFrame;
        public float playParticleFrame;
        public ParticleSystem[] particle;
    }

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
    CameraAndMask[] CutSceneCamera;

    [SerializeField]
    Transform targetTransform;
    [SerializeField]
    GameObject playerCharacter;
    [SerializeField]
    GameObject cutSceneP1;
    [SerializeField]
    GameObject cutSceneP2;
    [SerializeField]
    Transform P1Pivot;
    [SerializeField]
    Transform P2Pivot;

    [SerializeField]
    Animator cutAnim;
    [SerializeField]
    ZoomData[] zoomData;
    [SerializeField]
    AnimationCurve zoomCurve;
    [SerializeField]
    float totalFrame;
    [SerializeField]
    float playParticleFrame;
    [SerializeField]
    PaticlesTimeing[] pt;
    [SerializeField]
    Camera cutCamera;

    int cameraIndex = 0;

    void StartCutScene()
    {
        if (StringView.Instance.isPlayCutScene == false)
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
        if (StringView.Instance.isPlayCutScene == false) StartCutScene();
    }

    IEnumerator Task()
    {
        yield return StartCoroutine(FadeInFadeOut(MainCamera, CutSceneCamera[cameraIndex], 1.0f));
        yield return StartCoroutine(Anim());
        yield return StartCoroutine(FadeInFadeOut(CutSceneCamera[cameraIndex], MainCamera, 1.0f));
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
        cutSceneP1.SetActive(!cutSceneP1.activeInHierarchy);
        cutSceneP2.SetActive(!cutSceneP2.activeInHierarchy);

        StringView.Instance.cutP1 = P1Pivot.transform;
        StringView.Instance.cutP2 = P2Pivot.transform;

        StringView.Instance.isPlayCutScene = !StringView.Instance.isPlayCutScene;
        fadeOut.camera.SetActive(true);

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
        int beforeAnimHash = cutAnim.GetCurrentAnimatorStateInfo(0).shortNameHash;
        if (isOn == false)
        {
            isOn = true;
            cutAnim.SetTrigger(cut.ToString());
            if (guideObjct != null && nextMonument != null) nextMonument.Guid();
            InputController.ExtendMaxDistanceLength(extendLength);
        }
        else if (guideObjct != null && guideObjct.activeInHierarchy == true)
        {
            guideObjct.SetActive(false);
        }

        bool cutChange = true;
        if (cutChange)
        {

            for (int i = 0; i < CutSceneCamera.Length - 1; i++)
            {
                while (
                    cutAnim.GetCurrentAnimatorStateInfo(0).shortNameHash == beforeAnimHash ||
                    cutAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)//発生させたいフレーム/アニメーションの総フレーム数
                {
                    if (cut.ToString() == "Cut3")
                    {
                        CutSceneCamera[cameraIndex].camera.transform.LookAt(targetTransform);
                    }
                    for (int j = 0; j < pt.Length; j++) {
                        if (cutAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < (pt[j].playParticleFrame / pt[j].totalFrame))
                        {
                            for (int k = 0; k < pt[j].particle.Length; k++)
                            {
                                pt[j].particle[k].Play();
                            }
                        }
                    }
                    yield return null;
                }

                beforeAnimHash = cutAnim.GetCurrentAnimatorStateInfo(0).shortNameHash;

                if (cameraIndex < CutSceneCamera.Length - 1)
                {
                    CutSceneCamera[cameraIndex].camera.SetActive(false);
                    cameraIndex++;
                    CutSceneCamera[cameraIndex].camera.SetActive(true);
                    cutAnim.SetTrigger("Next");
                }
            }

            while (
                cutAnim.GetCurrentAnimatorStateInfo(0).shortNameHash == beforeAnimHash ||
                cutAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < (playParticleFrame / totalFrame))//発生させたいフレーム/アニメーションの総フレーム数
            {
                CutSceneCamera[cameraIndex].camera.transform.LookAt(targetTransform);

                for (int j = 0; j < pt.Length; j++)
                {
                    if (cutAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < (pt[j].playParticleFrame / pt[j].totalFrame))
                    {
                        for (int k = 0; k < pt[j].particle.Length; k++)
                        {
                            pt[j].particle[k].Play();
                        }
                    }
                }
                yield return null;
            }
        }
        else
        {
            while (
                cutAnim.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash("New State") ||
                cutAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < (40f / 45f))//発生させたいフレーム/アニメーションの総フレーム数
            {

                yield return null;
            }
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
