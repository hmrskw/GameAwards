using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// *************************************************
/// 制作者 三澤裕樹
/// *************************************************
/// カメラの挙動を制御するクラス
/// *************************************************
/// </summary>
public class CameraController : MonoBehaviour {

    [SerializeField]
    GameObject PlayerCharacter1;

    [SerializeField]
    GameObject PlayerCharacter2;

    [SerializeField]
    GameObject CameraTiltPivot;

    [SerializeField]
    GameObject CameraObjct;

    [SerializeField,Tooltip("カメラが左右に回転する速さ")]
    float panSpeed;

    RaycastHit hit;

    float time = 0.1f;

    Vector3 endPosition;

    [SerializeField]
    AnimationCurve curve;

    [SerializeField]
    UIController uiController;

    void Start()
    {
        endPosition = transform.position;
        StartCoroutine(MoveCamera());
    }

    void OnEnable()
    {
        endPosition = transform.position;
        StartCoroutine(MoveCamera());
    }

    void Update()
    {
        {
            Vector3 camPos = Vector3.Lerp(PlayerCharacter1.transform.position, PlayerCharacter2.transform.position, 0.5f);

            endPosition = new Vector3(camPos.x, camPos.y, camPos.z);

            CameraObjct.transform.localPosition = new Vector3(0, 12, -30);

            float dis = Vector3.Distance(PlayerCharacter1.transform.position, PlayerCharacter2.transform.position);

            if (dis > 7f)
            {
                dis -= 7f;
                CameraObjct.transform.localPosition += new Vector3(0, dis * 0.5f, -dis * 1.5f);
            }

            transform.Rotate(0, Input.GetAxis("RotateCameraLeft") * -panSpeed, 0, Space.Self);
        }
    }

    IEnumerator MoveCamera()
    {
        float startTime = Time.timeSinceLevelLoad; ;
        Vector3 startPosition = transform.position;
        float diff = Time.timeSinceLevelLoad - startTime;
        float pos;

        while (true)
        {
            diff = Time.timeSinceLevelLoad - startTime;

            startTime = Time.timeSinceLevelLoad;
            startPosition = transform.position;
            
            pos = curve.Evaluate(diff / time);

            transform.position = Vector3.Lerp (startPosition, endPosition, pos);

            yield return null;
        }
    }
}