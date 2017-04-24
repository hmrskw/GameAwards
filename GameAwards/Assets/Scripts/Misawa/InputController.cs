using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {
    class PlayerComponents
    {
        public Rigidbody rigidbody;
        public PlayerModel playerModel;
        public Animator animator;
    }

    [SerializeField]
    GameObject PlayerCharacter1;

    [SerializeField]
    GameObject PlayerCharacter2;

    /*
    [SerializeField]
    GameObject CameraObjct;
    */
    [SerializeField]
    GameObject CameraPivot;

    [SerializeField,Range(1,10)]
    float speed;

    [SerializeField, Space(15)]
    float jumpPower;

    static float maxDistanceLength = 10;

    PlayerComponents PlayerCharacter1Components = new PlayerComponents();
    PlayerComponents PlayerCharacter2Components = new PlayerComponents();

    //bool camMode = false;

    void Awake()
    {
        Application.targetFrameRate = 30;
    }

    // Use this for initialization
    void Start () {
        PlayerCharacter1Components.rigidbody = PlayerCharacter1.GetComponent<Rigidbody>();
        PlayerCharacter1Components.playerModel = PlayerCharacter1.GetComponent<PlayerModel>();
        PlayerCharacter1Components.animator = PlayerCharacter1.GetComponent<Animator>();

        PlayerCharacter2Components.rigidbody = PlayerCharacter2.GetComponent<Rigidbody>();
        PlayerCharacter2Components.playerModel = PlayerCharacter2.GetComponent<PlayerModel>();
        PlayerCharacter2Components.animator = PlayerCharacter2.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
        //if (camMode == false)
        {

            float character1Horizontal = Input.GetAxis("LeftHorizontal") * 0.1f;
            float character1Vertical = Input.GetAxis("LeftVertical") * 0.1f;
            float character2Horizontal = Input.GetAxis("RightHorizontal") * 0.1f;
            float character2Vertical = Input.GetAxis("RightVertical") * 0.1f;

            PlayerCharacter1Components.animator.SetBool("IsWalk", (character1Horizontal != 0 || character1Vertical != 0));
            PlayerCharacter2Components.animator.SetBool("IsWalk", (character2Horizontal != 0 || character2Vertical != 0));

            // カメラの方向から、X-Z平面の単位ベクトルを取得
            Vector3 cameraForward = Vector3.Scale(CameraPivot.transform.forward, new Vector3(1, 0, 1)).normalized;

            Vector3 character1moveForward = cameraForward * character1Vertical + CameraPivot.transform.right * character1Horizontal;
            Vector3 character2moveForward = cameraForward * character2Vertical + CameraPivot.transform.right * character2Horizontal;

            if (Vector3.Distance(
                Vector3.Scale(PlayerCharacter1.transform.position, new Vector3(1, 0, 1)),
                Vector3.Scale(PlayerCharacter2.transform.position, new Vector3(1, 0, 1))) > maxDistanceLength)
            {
                if (PlayerCharacter1.transform.position.x - PlayerCharacter2.transform.position.x > 0)

               {
                    Vector3 moveVector = character2moveForward - character1moveForward;

                    if (moveVector.x < 0)
                    {
                        float h = (character1moveForward.x + character2moveForward.x) / 3;
                        character1moveForward.x = h;
                        character2moveForward.x = h;
                    }
                }
                else
                {
                    Vector3 moveVector = character1moveForward - character2moveForward;

                    if (moveVector.x < 0)
                    {
                        float h = (character1moveForward.x + character2moveForward.x) / 3;
                        character1moveForward.x = h;
                        character2moveForward.x = h;
                    }
                }

                if (PlayerCharacter1.transform.position.z - PlayerCharacter2.transform.position.z > 0)
                {
                    Vector3 moveVector = character2moveForward - character1moveForward;

                    if (moveVector.z < 0f)
                    {
                        float v = (character1moveForward.z + character2moveForward.z) / 3f;
                        character1moveForward.z = v;
                        character2moveForward.z = v;
                    }
                }
                else
                {
                    Vector3 moveVector = character1moveForward - character2moveForward;

                    if (moveVector.z < 0f)
                    {
                        float v = (character1moveForward.z + character2moveForward.z) / 3f;
                        character1moveForward.z = v;
                        character2moveForward.z = v;
                    }
                }
            }

            PlayerCharacter1.transform.LookAt(PlayerCharacter1.transform.position + new Vector3(character1moveForward.x, 0, character1moveForward.z));
            PlayerCharacter2.transform.LookAt(PlayerCharacter2.transform.position + new Vector3(character2moveForward.x, 0, character2moveForward.z));

            // 移動方向にスピードを掛ける。ジャンプや落下がある場合は、別途Y軸方向の速度ベクトルを足す。
            PlayerCharacter1.transform.Translate(character1moveForward * speed, Space.World);
            PlayerCharacter2.transform.Translate(character2moveForward * speed, Space.World);
        }

        if (Input.GetButton("LeftJump") && PlayerCharacter1Components.playerModel.CanJump && PlayerCharacter1.transform.position.y - PlayerCharacter2.transform.position.y < maxDistanceLength)
        {
            PlayerCharacter1Components.playerModel.CanJump = false;
            PlayerCharacter1Components.rigidbody.AddForce(new Vector3(0, jumpPower, 0));
        }
        if (Input.GetButton("RightJump") && PlayerCharacter2Components.playerModel.CanJump && PlayerCharacter2.transform.position.y - PlayerCharacter1.transform.position.y < maxDistanceLength)
        {
            PlayerCharacter2Components.playerModel.CanJump = false;
            PlayerCharacter2Components.rigidbody.AddForce(new Vector3(0, jumpPower, 0));
        }

        //CameraController();
    }

    /*
    Vector2 camRotate = new Vector2(0,0);
    private void CameraController()
    {
        Vector3 camPos = Vector3.Lerp(PlayerCharacter1.transform.position, PlayerCharacter2.transform.position, 0.5f);

        CameraPivot.transform.position = new Vector3(camPos.x, camPos.y, camPos.z);
        CameraObjct.transform.localPosition = new Vector3(0,9,-12);

        float dis = Vector3.Distance(PlayerCharacter1.transform.position, PlayerCharacter2.transform.position);

        if (dis > 5f)
        {
            dis -= 5f;
            CameraObjct.transform.localPosition += new Vector3(0, dis * 0.5f , -dis * 1.5f);
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
            CameraObjct.transform.localRotation = Quaternion.Euler(new Vector3(10,0,0));
        }

        CameraPivot.transform.Rotate(0, Input.GetAxis("RotateCameraLeft") * 2, 0, Space.Self);
    }
    */
    public static float GetMaxDistanceLength()
    {
        return maxDistanceLength;
    }

    public static void ExtendMaxDistanceLength(float extendLength)
    {
        maxDistanceLength += extendLength;
    }
}
