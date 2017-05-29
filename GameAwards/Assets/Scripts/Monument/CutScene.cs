using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CutScene : MonoBehaviour
{

    /*[System.Serializable]
    public class CameraAction
    {
        public IEnumerable action;
        public AnimationCurve curve;
        public float time;
    }*/
    [System.Serializable]
    public struct CameraAndMask
    {
        public GameObject camera;
        public Image mask;
    }
    
    [SerializeField]
    CameraAndMask MainCamera;

    [SerializeField]
    CameraAndMask CutSceneCamera;

    [SerializeField]
    AnimationCurve curve;
    [SerializeField]
    float moveTime;
    [SerializeField]
    Transform nextCameraPositionTransform;
    [SerializeField]
    Transform targetTransform;
    [SerializeField]
    ParticleSystem pop;

    //[SerializeField]
    //CameraAction[] cameraAction;

    int actionIndex = 0;

    bool isFade = false;

    Transform initCameraTransform;

    // Use this for initialization
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCutScene();
        }
    }

    public void StartCutScene()
    {
        if (isFade == false)
        {
            StartCoroutine(Task());
        }
    }

    IEnumerator Task()
    {
        isFade = true;
        yield return StartCoroutine(FadeInFadeOut(MainCamera, CutSceneCamera,1.0f));
        yield return StartCoroutine(MoveCamera());
        //yield return new WaitForSeconds(1f);
        yield return StartCoroutine(FlowerAnim());
        yield return StartCoroutine(FadeInFadeOut(CutSceneCamera, MainCamera, 1.0f));
        isFade = false;
        //StartCoroutine(MoveCharacter());
        //yield return StartCoroutine(MoveCamera());
        //yield return StartCoroutine(Flower());
    }

    IEnumerator FadeInFadeOut(CameraAndMask fadeIn, CameraAndMask fadeOut,float time)
    {
        float startTime = Time.timeSinceLevelLoad;
        float diff = Time.timeSinceLevelLoad - startTime;
        Color maskAlpha = new Color(0,0,0,0);

        while (diff < (time/2f))
        {
            diff = Time.timeSinceLevelLoad - startTime;
            maskAlpha.a = diff / (time/2);
            fadeIn.mask.color = maskAlpha;
            yield return null;
        }
        fadeIn.camera.SetActive(false);
        fadeOut.camera.SetActive(true);

        startTime = Time.timeSinceLevelLoad;
        diff = Time.timeSinceLevelLoad - startTime;
        while (diff < time/2)
        {
            diff = Time.timeSinceLevelLoad - startTime;
            maskAlpha.a = 1 - (diff / (time / 2));
            fadeOut.mask.color = maskAlpha;
            yield return null;
        }
    }

    IEnumerator MoveCharacter()
    {
        yield return null;
    }

    IEnumerator MoveCamera()
    {
        initCameraTransform = CutSceneCamera.camera.transform;
        float startTime = Time.timeSinceLevelLoad;
        Vector3 startPosition = CutSceneCamera.camera.transform.position;
        float diff = Time.timeSinceLevelLoad - startTime;
        float pos;

        while (diff < moveTime)
        {
            diff = Time.timeSinceLevelLoad - startTime;

            pos = curve.Evaluate(diff / moveTime);

            CutSceneCamera.camera.transform.LookAt(targetTransform);
            CutSceneCamera.camera.transform.position = Vector3.Lerp(startPosition, nextCameraPositionTransform.position, pos);

            yield return null;
        }
    }

    IEnumerator FlowerAnim()
    {
        pop.Play();
        while (pop.isPlaying)
        {
            yield return null;
        }
    }
}
