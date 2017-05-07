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

    [SerializeField, Tooltip("カメラが上下に回転する速さ")]
    float tiltSpeed;

    // Update is called once per frame
    void Update () {
        Vector3 camPos = Vector3.Lerp(PlayerCharacter1.transform.position, PlayerCharacter2.transform.position, 0.5f);

        transform.position = new Vector3(camPos.x, camPos.y, camPos.z);

        CameraObjct.transform.localPosition = new Vector3(0, 9, -12);

        float dis = Vector3.Distance(PlayerCharacter1.transform.position, PlayerCharacter2.transform.position);

        if (dis > 5f)
        {
            dis -= 5f;
            CameraObjct.transform.localPosition += new Vector3(0, dis * 0.5f, -dis * 1.5f);
        }

        Check();

        transform.Rotate(0, Input.GetAxis("RotateCameraLeft") * panSpeed, 0, Space.Self);
    }

    void Check()
    {
        RaycastHit hit;

        Debug.DrawRay(transform.position + new Vector3(0, 2f, 0), transform.TransformDirection(Vector3.forward)*5f, Color.blue);

        if (Physics.Raycast(transform.position + new Vector3(0, 2f, 0), transform.TransformDirection(Vector3.forward), out hit, 10f, LayerMask.GetMask("Ground")))
        {
            float angle = Vector3.Angle(hit.normal, transform.rotation.eulerAngles);
            if (angle > 0 && angle < 60 && Mathf.Abs(180 - CameraTiltPivot.transform.rotation.eulerAngles.x) > 180 - angle)
            {
                CameraTiltPivot.transform.Rotate(-tiltSpeed, 0, 0, Space.Self);
            }
        }
        else if (CameraTiltPivot.transform.rotation.eulerAngles.x > 180 || CameraTiltPivot.transform.rotation.eulerAngles.x < 0f)
        {
            CameraTiltPivot.transform.Rotate(tiltSpeed, 0, 0, Space.Self);
        }
    }
}