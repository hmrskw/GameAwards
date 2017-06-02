using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CutScene : Monument
{
    
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
    float cameraMoveTime;
    [SerializeField]
    float characterMoveSpeed;

    [SerializeField,Tooltip("カメラの移動先")]
    Transform nextCameraPositionTransform;
    [SerializeField,Tooltip("カメラはこのオブジェクトの方向を見続ける")]
    Transform targetTransform;
    //[SerializeField]
    //ParticleSystem pop;
    [SerializeField]
    Player p1;
    [SerializeField]
    Player p2;

    [SerializeField]
    GameObject playerCharacter;
    [SerializeField]
    GameObject cutSceneCharacter;

    //[SerializeField]
    //CameraAction[] cameraAction;

    int actionIndex = 0;



    bool isPlayCutScene = false;
    public bool IsPlayCutScene
    {
        set { isPlayCutScene = value; }
        get { return isPlayCutScene; }
    }

    Transform initCameraTransform;

    // Use this for initialization
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCutScene();
        }
        Debug.Log(isPlayCutScene);
    }

    public void StartCutScene()
    {
        if (isPlayCutScene == false)
        {
            StartCoroutine(Task());
        }
    }

    IEnumerator Task()
    {
        yield return StartCoroutine(FadeInFadeOut(MainCamera, CutSceneCamera,1.0f));
        yield return StartCoroutine(MoveCharacter());
        yield return StartCoroutine(MoveCamera());
        yield return StartCoroutine(FlowerAnim());
        yield return StartCoroutine(FadeInFadeOut(CutSceneCamera, MainCamera, 1.0f));
        //isPlayCutScene = false;
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
        playerCharacter.SetActive(!playerCharacter.activeInHierarchy);
        cutSceneCharacter.SetActive(!cutSceneCharacter.activeInHierarchy);
        fadeIn.camera.SetActive(false);
        isPlayCutScene = !isPlayCutScene;
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
        //float startTime = Time.timeSinceLevelLoad;
        //float diff = Time.timeSinceLevelLoad - startTime;
        //while (diff < moveTime)
        {
            //diff = Time.timeSinceLevelLoad - startTime;
            p1.SetCharacterMoveDirection(new Vector3(0, 0, -characterMoveSpeed));
            p2.SetCharacterMoveDirection(new Vector3(0, 0, -characterMoveSpeed));
            yield return null;
        }
    }

    IEnumerator MoveCamera()
    {
        initCameraTransform = CutSceneCamera.camera.transform;
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
        //pop.Play();
        //while (pop.isPlaying)
        {
            yield return null;
        }
    }
}
