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
    GameObject playerCharacter;
    [SerializeField]
    GameObject cutSceneCharacters;
    [SerializeField]
    Transform cutSceneCharactersInitPosition;

    bool isPlayCutScene = false;
    public bool IsPlayCutScene
    {
        set { isPlayCutScene = value; }
        get { return isPlayCutScene; }
    }

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
        
        yield return StartCoroutine(FadeInFadeOut(MainCamera, CutSceneCamera,1.0f));
        /*yield return*/
        p1.transform.localPosition = new Vector3(-4f, 0f, 0f);
        p2.transform.localPosition = new Vector3(4f, 0f, 0f);
        StartCoroutine(MoveCharacter());
        yield return StartCoroutine(MoveCamera());
        yield return StartCoroutine(FlowerAnim());
        yield return StartCoroutine(FadeInFadeOut(CutSceneCamera, MainCamera, 1.0f));
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

        playerCharacter.SetActive(!playerCharacter.activeInHierarchy);

        cutSceneCharacters.SetActive(!cutSceneCharacters.activeInHierarchy);
        if(cutSceneCharacters.activeInHierarchy == true)
        {
            cutSceneCharacters.transform.position = cutSceneCharactersInitPosition.position;
            cutSceneCharacters.transform.rotation = cutSceneCharactersInitPosition.rotation;
        }

        isPlayCutScene = !isPlayCutScene;
        fadeOut.camera.SetActive(true);
        CutSceneCamera.camera.transform.LookAt(targetTransform);

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
        yield return StartCoroutine(Boot());

        while (openAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime - animationStart < 1 || particle.isPlaying)
        {
            yield return null;
        }
    }

    IEnumerator MoveCharacter()
    {
        p1.SetCharacterMoveDirection(new Vector3(0, 0, -characterMoveSpeed));
        p2.SetCharacterMoveDirection(new Vector3(0, 0, -characterMoveSpeed));
        yield return new WaitForSeconds(2f);
        p1.SetCharacterMoveDirection(new Vector3(0, 0, 0));
        p2.SetCharacterMoveDirection(new Vector3(0, 0, 0));
    }
}
