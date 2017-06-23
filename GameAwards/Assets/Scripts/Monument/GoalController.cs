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
    ParticleSystem[] checkPointParticles;

    [SerializeField]
    GameObject windObj;

    //[SerializeField]
    //ParticleSystem windParticle;

    [SerializeField]
    GameObject pulseObj;

    [SerializeField]
    CameraAndMask mainCamera;

    [SerializeField]
    CameraAndMask CutSceneCamera;

    // Use this for initialization
    void Start () {
        pulseObj.gameObject.SetActive(false);
        StartCoroutine(Task());
    }

    IEnumerator Task()
    {
        yield return StartCoroutine(Wait());
        windObj.SetActive(false);
        pulseObj.gameObject.SetActive(true);
        /*
        while (StringView.Instance.isPlayCutScene)
        {
            yield return null;
        }
        yield return StartCoroutine(FadeOut(mainCamera,1));
        mainCamera.camera.SetActive(false);
        CutSceneCamera.camera.SetActive(true);
        yield return StartCoroutine(FadeIn(CutSceneCamera, 1));

        //windParticle.Stop();
        //yield return new WaitForSeconds(5f);

        yield return StartCoroutine(FadeOut(CutSceneCamera, 1));
        CutSceneCamera.camera.SetActive(false);
        mainCamera.camera.SetActive(true);
        yield return StartCoroutine(FadeIn(mainCamera, 1));
        */
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
    */

    IEnumerator Wait()
    {
        bool open = false;
        bool isNotPlayParticles = false;

        while (open == false) {
            open = checkPoints[0].IsOn;
            for (int i = 0; i < checkPoints.Length; i++)
            {
                open &= checkPoints[i].IsOn;
                if (open == false) break;
            }
            yield return null;
        }
        while (isNotPlayParticles == false)
        {
            isNotPlayParticles = !checkPointParticles[0].isPlaying;
            for (int i = 0; i < checkPoints.Length; i++)
            {
                isNotPlayParticles &= !checkPointParticles[i].isPlaying;
                if (isNotPlayParticles == false) break;
            }
            yield return null;
            /*
            for (int i = 0; i < checkPoints.Length; i++) {
                isPlayParticles &= checkPointParticles[i].isPlaying;
            }*/
        }
    }    
}
