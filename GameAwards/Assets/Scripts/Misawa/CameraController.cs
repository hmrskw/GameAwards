using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField]
    GameObject PlayerCharacter1;

    [SerializeField]
    GameObject PlayerCharacter2;

    [SerializeField]
    GameObject CameraObjct;
    
    bool camMode = false;

    Vector2 camRotate = new Vector2(0, 0);

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

        if (Input.GetButtonDown("CameraModeChange"))
        {
            camMode = !camMode;
            Debug.Log("camMode = " + camMode);
        }

        if (camMode)
        {
            if (Input.GetAxis("LeftVertical") != 0 || Input.GetAxis("LeftHorizontal") != 0)
            {
                camRotate += new Vector2(-Input.GetAxis("LeftVertical"), Input.GetAxis("LeftHorizontal"));
                CameraObjct.transform.localRotation = Quaternion.Euler(new Vector3(10 + camRotate.x, camRotate.y, 0));
            }
        }
        else
        {
            CameraObjct.transform.localRotation = Quaternion.Euler(new Vector3(10, 0, 0));
        }

        transform.Rotate(0, Input.GetAxis("RotateCameraLeft") * 2, 0, Space.Self);
    }
}