using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {
    class PlayerComponents
    {
        public Rigidbody rigidbody;
        public PlayerModel playerModel;
    }

    [SerializeField]
    GameObject PlayerCharacter1;

    [SerializeField]
    GameObject PlayerCharacter2;

    [SerializeField]
    GameObject CameraObjct;
    [SerializeField]
    GameObject CameraPivot;

    [SerializeField,Range(1,10)]
    float speed;

    [SerializeField, Space(15)]
    float jumpPower;

    static float maxDistanceLength = 10;

    PlayerComponents PlayerCharacter1Components = new PlayerComponents();
    PlayerComponents PlayerCharacter2Components = new PlayerComponents();

    void Awake()
    {
        Application.targetFrameRate = 30;
    }

    // Use this for initialization
    void Start () {
        PlayerCharacter1Components.rigidbody = PlayerCharacter1.GetComponent<Rigidbody>();
        PlayerCharacter1Components.playerModel = PlayerCharacter1.GetComponent<PlayerModel>();

        PlayerCharacter2Components.rigidbody = PlayerCharacter2.GetComponent<Rigidbody>();
        PlayerCharacter2Components.playerModel = PlayerCharacter2.GetComponent<PlayerModel>();
    }

    // Update is called once per frame
    void Update () {
        float lh, lv, rh, rv;
        lh = Input.GetAxis("LeftHorizontal") * 0.1f;
        lv = Input.GetAxis("LeftVertical") * 0.1f;
        rh = Input.GetAxis("RightHorizontal") * 0.1f;
        rv = Input.GetAxis("RightVertical") * 0.1f;

        // カメラの方向から、X-Z平面の単位ベクトルを取得
        Vector3 cameraForward = Vector3.Scale(CameraPivot.transform.forward, new Vector3(1, 0, 1)).normalized;

        if (lh != 0 || lv != 0)
        {

            // 方向キーの入力値とカメラの向きから、移動方向を決定
            Vector3 moveForward = cameraForward * lv + CameraPivot.transform.right * lh;

            if (Vector3.Distance(
                PlayerCharacter1.transform.position + /*new Vector3(lh * 0.1f * speed, 0, lv * 0.1f * speed)*/(moveForward * speed),
                PlayerCharacter2.transform.position) > maxDistanceLength)
            {
                lh = 0;
                lv = 0;
            }

            // 移動方向にスピードを掛ける。ジャンプや落下がある場合は、別途Y軸方向の速度ベクトルを足す。
            PlayerCharacter1.transform.Translate(moveForward * speed);
            /*
            // キャラクターの向きを進行方向に
            if (moveForward != Vector3.zero)
            {
                PlayerCharacter1.transform.rotation = Quaternion.LookRotation(moveForward);
            }*/

            //PlayerCharacter1.transform.Translate(new Vector3(lh * 0.1f * speed, 0, lv * 0.1f * speed));
        }
        if (rh != 0 || rv != 0)
        {
            // 方向キーの入力値とカメラの向きから、移動方向を決定
            Vector3 moveForward = cameraForward * rv + CameraPivot.transform.right * rh;

            if (Vector3.Distance(
                PlayerCharacter1.transform.position,
                PlayerCharacter2.transform.position + (moveForward * speed))/*new Vector3(rh * 0.1f * speed, 0, rv * 0.1f * speed)*/ > maxDistanceLength)
            {
                rh = 0;
                rv = 0;
            }

            // 移動方向にスピードを掛ける。ジャンプや落下がある場合は、別途Y軸方向の速度ベクトルを足す。
            PlayerCharacter2.transform.Translate(moveForward * speed);
            /*
            // キャラクターの向きを進行方向に
            if (moveForward != Vector3.zero)
            {
                PlayerCharacter2.transform.rotation = Quaternion.LookRotation(moveForward);
            }*/

            //PlayerCharacter2.transform.Translate(new Vector3(rh * 0.1f * speed, 0, rv * 0.1f * speed));
        }

        if (Input.GetButton("LeftJump") && PlayerCharacter1Components.playerModel.CanJump)
        {
            PlayerCharacter1Components.playerModel.CanJump = false;
            PlayerCharacter1Components.rigidbody.AddForce(new Vector3(0, jumpPower, 0));
        }
        if (Input.GetButton("RightJump") && PlayerCharacter2Components.playerModel.CanJump)
        {
            PlayerCharacter2Components.playerModel.CanJump = false;
            PlayerCharacter2Components.rigidbody.AddForce(new Vector3(0, jumpPower, 0));
        }

        CameraController();
    }

    private void CameraController()
    {
        //Vector3 camPos = Vector3.Min(PlayerCharacter1.transform.position, PlayerCharacter2.transform.position);
        //camPos.x = Mathf.Lerp(PlayerCharacter1.transform.position.x, PlayerCharacter2.transform.position.x, 0.5f);

        Vector3 camPos = Vector3.Lerp(PlayerCharacter1.transform.position, PlayerCharacter2.transform.position, 0.5f);

        CameraPivot.transform.position = new Vector3(camPos.x, camPos.y, camPos.z);
        CameraObjct.transform.localPosition = new Vector3(/*CameraPivot.transform.position.x, CameraPivot.transform.position.y + 10, CameraPivot.transform.position.z - 9*/0,10,-9);

        float dis = Vector3.Distance(PlayerCharacter1.transform.position, PlayerCharacter2.transform.position);

        if (dis > 5f)
        {
            dis -= 5f;
            CameraObjct.transform.localPosition += new Vector3(0, dis * 1.5f , -dis * 1.5f);
        }

        //if (Input.GetAxis("RotateCameraLeft"))
            CameraPivot.transform.Rotate(0, Input.GetAxis("RotateCameraLeft") * 2, 0, Space.Self);
        /*if (Input.GetButton("RotateCameraRight"))
            CameraPivot.transform.Rotate(0, -2, 0, Space.Self);
            */
    }

    public static float GetMaxDistanceLength()
    {
        return maxDistanceLength;
    }

    public static void ExtendMaxDistanceLength(float extendLength)
    {
        maxDistanceLength += extendLength;
    }
}
