using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //[SerializeField, Tooltip("カメラが上下に回転する速さ")]
    //float tiltSpeed;

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

    // Update is called once per frame
    void Update()
    {
        //if (uiController.isDrawUI == false)
        {
            Vector3 camPos = Vector3.Lerp(PlayerCharacter1.transform.position, PlayerCharacter2.transform.position, 0.5f);

            //transform.position = new Vector3(camPos.x, camPos.y, camPos.z);
            endPosition = new Vector3(camPos.x, camPos.y, camPos.z);

            CameraObjct.transform.localPosition = new Vector3(0, 12, -30);

            float dis = Vector3.Distance(PlayerCharacter1.transform.position, PlayerCharacter2.transform.position);

            if (dis > 7f)
            {
                dis -= 7f;
                CameraObjct.transform.localPosition += new Vector3(0, dis * 0.5f, -dis * 1.5f);
            }

            //Check();

            transform.Rotate(0, Input.GetAxis("RotateCameraLeft") * panSpeed, 0, Space.Self);
        }
    }

    /*void Check()
    {
        if (Physics.Raycast(transform.position + new Vector3(0, 2f, 0), transform.TransformDirection(Vector3.forward), out hit, 10f, LayerMask.GetMask("Ground")))
        {
            float angle = Vector3.Angle(hit.normal, transform.rotation.eulerAngles);
            if (angle > 0 && angle < 60 && Mathf.Abs(180 - CameraTiltPivot.transform.rotation.eulerAngles.x) > 180 - angle/2)
            {
                CameraTiltPivot.transform.Rotate(-tiltSpeed, 0, 0, Space.Self);
            }
        }
        else if(Physics.Raycast(transform.position + new Vector3(0, 2f, 0), transform.TransformDirection(Vector3.back), out hit, 10f, LayerMask.GetMask("Ground")))
        {
            float angle = Vector3.Angle(hit.normal, transform.rotation.eulerAngles);
            if (angle > 0 && angle < 60 && Mathf.Abs(180 - CameraTiltPivot.transform.rotation.eulerAngles.x) >= 180 - angle / 2)
            {
                CameraTiltPivot.transform.Rotate(tiltSpeed, 0, 0, Space.Self);
            }
        }
        else if (CameraTiltPivot.transform.rotation.eulerAngles.x > 180 || CameraTiltPivot.transform.rotation.eulerAngles.x < -1f)
        {
            CameraTiltPivot.transform.Rotate(tiltSpeed, 0, 0, Space.Self);
        }
        else if (CameraTiltPivot.transform.rotation.eulerAngles.x > 1f)
        {
            CameraTiltPivot.transform.Rotate(-tiltSpeed, 0, 0, Space.Self);
        }
    }*/

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