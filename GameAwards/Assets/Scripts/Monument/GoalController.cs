using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// *************************************************
/// 制作者 三澤裕樹
/// *************************************************
/// ゴールのカットシーンを制御するクラス
/// *************************************************
/// </summary>
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

    [SerializeField]
    GameObject pulseObj;

    [SerializeField]
    CameraAndMask mainCamera;

    [SerializeField]
    CameraAndMask CutSceneCamera;

    void Start () {
        pulseObj.gameObject.SetActive(false);
        StartCoroutine(Task());
    }

    IEnumerator Task()
    {
        yield return StartCoroutine(Wait());
        windObj.SetActive(false);
        pulseObj.gameObject.SetActive(true);
    }

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
        }
    }    
}
