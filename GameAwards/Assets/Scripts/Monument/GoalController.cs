using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GoalController : MonoBehaviour
{
    [System.Serializable]
    public struct CameraAndMask
    {
        public GameObject camera;
        public Image mask;
    }

    [SerializeField]
    Monument[] checkPoints;

    [SerializeField]
    GameObject windObj;

    [SerializeField]
    ParticleSystem windParticle;

    [SerializeField]
    GameObject pulseObj;

    [SerializeField]
    CameraAndMask mainCamera;

    [SerializeField]
    CameraAndMask CutSceneCamera;

    bool open = false;

    // Use this for initialization
    void Start () {
        pulseObj.gameObject.SetActive(false);
        StartCoroutine(Task());
    }

    IEnumerator Task()
    {
        yield return StartCoroutine(Wait());
        while (StringView.Instance.isPlayCutScene)
        {
            yield return null;
        }
        yield return StartCoroutine(FadeOut(mainCamera,1));
        mainCamera.camera.SetActive(false);
        CutSceneCamera.camera.SetActive(true);
        yield return StartCoroutine(FadeIn(CutSceneCamera, 1));

        windParticle.Stop();
        pulseObj.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        windObj.SetActive(false);

        yield return StartCoroutine(FadeOut(CutSceneCamera, 1));
        CutSceneCamera.camera.SetActive(false);
        mainCamera.camera.SetActive(true);
        yield return StartCoroutine(FadeIn(mainCamera, 1));
    }

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


    IEnumerator Wait()
    {
        while (open == false) {
            for (int i = 0; i < checkPoints.Length; i++)
            {
                if (i == 0) open = checkPoints[i].IsOn;
                else
                {
                    open &= checkPoints[i].IsOn;
                }
                if (open == false) break;
            }
            //if (open == true && windObj.activeInHierarchy == true) windObj.SetActive(false);
            yield return null;
        }
    }    
}
