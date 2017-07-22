using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// *************************************************
/// 制作者 三澤裕樹
/// *************************************************
/// 特定のカットシーンを制御するクラス
/// *************************************************
/// </summary>
public class Cut5 : Monument {
    [SerializeField]
    CameraAndMask MainCamera;

    [SerializeField]
    CameraAndMask CutSceneCamera;

    [SerializeField]
    AnimationCurve curve;
    [SerializeField]
    float cameraMoveTime;
    
    [SerializeField, Tooltip("カメラはこのオブジェクトの方向を見続ける")]
    Transform cameraTargetTransform;
    [SerializeField, Tooltip("演出で移動する敵")]
    GameObject moveEnemy;
    [SerializeField, Tooltip("演出で移動する敵の移動先")]
    Transform moveEnemyTarget;

    [SerializeField]
    GameObject[] Characters;

    [SerializeField]
    Player p1;
    [SerializeField]
    Player p2;
    [SerializeField]
    int enemyTargetValue;

    [Space(15)]
    [SerializeField]
    CameraAndMask GoalSceneCamera;
    [SerializeField]
    ParticleSystem wind;

    bool isEvent = false;

    void StartCutScene()
    {
        if (StringView.Instance.isPlayCutScene == false)
        {
            StartCoroutine(Task());
        }
    }

    override protected IEnumerator Wait()
    {
        yield return null;
    }

    IEnumerator Task()
    {
        int initialDestroyEnemyCount = StringView.Instance.DestroyEnemyCount;
        yield return StartCoroutine(FadeInFadeOut(MainCamera, CutSceneCamera, 1.0f, ChangePlayer));
        yield return StartCoroutine(MoveCamera());

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(FadeInFadeOut(CutSceneCamera, MainCamera, 1.0f, ChangePlayer));

        while (StringView.Instance.DestroyEnemyCount -initialDestroyEnemyCount < enemyTargetValue)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(FadeInFadeOut(MainCamera, CutSceneCamera, 1.0f, ()=> {
            guideObjct.SetActive(false);
            ChangePlayer();
        }));
        yield return StartCoroutine(FlowerAnim());
        yield return StartCoroutine(FadeInFadeOut(CutSceneCamera, GoalSceneCamera, 1.0f,null));
        SoundManager.Instance.PlaySE("wind");
        wind.Stop();
        yield return new WaitForSeconds(5f);
        yield return StartCoroutine(FadeInFadeOut(GoalSceneCamera, MainCamera, 1.0f, ()=> {
            StringView.Instance.GrassTextureUpdate(1);
            ChangePlayer();
        }));
    }

    void ChangePlayer()
    {
        StringView.Instance.cutP1 = p1.transform;
        StringView.Instance.cutP2 = p2.transform;
        StringView.Instance.isPlayCutScene = !StringView.Instance.isPlayCutScene;

        CutSceneCamera.camera.transform.LookAt(cameraTargetTransform);
    }

    IEnumerator MoveCamera()
    {
        float startTime = Time.timeSinceLevelLoad;
        Vector3 startPosition = moveEnemy.transform.position;
        float diff = Time.timeSinceLevelLoad - startTime;
        float pos;

        while (diff < cameraMoveTime)
        {
            diff = Time.timeSinceLevelLoad - startTime;
            pos = curve.Evaluate(diff / cameraMoveTime);

            CutSceneCamera.camera.transform.LookAt(cameraTargetTransform);
            moveEnemy.transform.position = Vector3.Lerp(startPosition, moveEnemyTarget.position, pos);

            yield return null;
        }
    }

    IEnumerator FlowerAnim()
    {
        Destroy(moveEnemy);
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
            isEvent = false;
            yield return new WaitForSeconds(0.5f);
            openAnimation.SetTrigger("Open");
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

        SoundManager.Instance.PlaySE("se object");
        particle.Play();
    }

    void OnTriggerEnter(Collider col)
    {
        if (isOn == false && isEvent == false && StringView.Instance.isPlayCutScene == false)
        {
            if (col.gameObject.tag == "Player")
            {
                if (SoundManager.Instance.IsPlayBGM("asioto") == true)
                {
                    SoundManager.Instance.StopBGM("asioto");
                }
                isEvent = true;
                StartCutScene();
            }
        }
    }
}
