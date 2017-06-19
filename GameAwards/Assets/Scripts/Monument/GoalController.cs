using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalController : Monument {

    [SerializeField]
    Monument[] checkPoints;

    [SerializeField]
    GameObject windObj;

    [SerializeField]
    CameraAndMask mainCamera;

    [SerializeField]
    CameraAndMask CutSceneCamera;

    bool open;

    // Use this for initialization
    void Start () {
        StartCoroutine(Task());
    }

    IEnumerator Task()
    {
        yield return StartCoroutine(Wait());
        yield return StartCoroutine(WhiteOut(mainCamera,2f));
        SceneManager.LoadScene(0);

    }

    IEnumerator WhiteOut(CameraAndMask fadeIn, float time)
    {
        float startTime = Time.timeSinceLevelLoad;
        float diff = Time.timeSinceLevelLoad - startTime;
        Color maskAlpha = new Color(1, 1, 1, 0);

        while (diff < (time / 2f))
        {
            diff = Time.timeSinceLevelLoad - startTime;
            maskAlpha.a = diff / (time / 2);
            fadeIn.mask.color = maskAlpha;
            yield return null;
        }
        //fadeIn.camera.SetActive(false);
    }

    override protected IEnumerator Wait()
    {
        while (StringView.Instance.OnHitLine(transform.position) == false) {
            for (int i = 0; i < checkPoints.Length; i++)
            {
                if (i == 0) open = checkPoints[i].IsOn;
                else
                {
                    open &= checkPoints[i].IsOn;
                }
                if (open == false) break;
            }
            if (open == true && windObj.activeInHierarchy == true) windObj.SetActive(false);
            yield return null;
        }
    }    
}
