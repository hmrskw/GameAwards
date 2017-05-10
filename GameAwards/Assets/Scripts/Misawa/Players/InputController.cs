using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {
    class PlayerComponents
    {
        //public Rigidbody rigidbody;
        public Player playerModel;
    }

    [SerializeField, Header("プレイヤー")]
    GameObject PlayerCharacter1;

    [SerializeField]
    GameObject PlayerCharacter2;

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
        //PlayerCharacter1Components.rigidbody = PlayerCharacter1.GetComponent<Rigidbody>();
        PlayerCharacter1Components.playerModel = PlayerCharacter1.GetComponent<Player>();

        //PlayerCharacter2Components.rigidbody = PlayerCharacter2.GetComponent<Rigidbody>();
        PlayerCharacter2Components.playerModel = PlayerCharacter2.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update() {
        //スティックの入力を受け取る
        float character1Horizontal = Input.GetAxis("LeftHorizontal");
        float character1Vertical = Input.GetAxis("LeftVertical");
        float character2Horizontal = Input.GetAxis("RightHorizontal");
        float character2Vertical = Input.GetAxis("RightVertical");

        // カメラの方向から、X-Z平面の単位ベクトルを取得
        Vector3 cameraForward = Vector3.Scale(CameraPivot.transform.forward, new Vector3(1, 0, 1)).normalized;

        //キャラクターの移動方向を計算
        Vector3 character1MoveDirection = cameraForward * character1Vertical + CameraPivot.transform.right * character1Horizontal;
        Vector3 character2MoveDirection = cameraForward * character2Vertical + CameraPivot.transform.right * character2Horizontal;

        //糸の上限値以上離れようとしたら移動方向を制限する
        if (Vector3.Distance(
            Vector3.Scale(PlayerCharacter1.transform.position, new Vector3(1, 0, 1)),
            Vector3.Scale(PlayerCharacter2.transform.position, new Vector3(1, 0, 1))) > maxDistanceLength)
        {
            if (PlayerCharacter1.transform.position.x - PlayerCharacter2.transform.position.x > 0)
                CalculateMoveDirection(ref character1MoveDirection.x, ref character2MoveDirection.x);
            else
                CalculateMoveDirection(ref character2MoveDirection.x, ref character1MoveDirection.x);

            if (PlayerCharacter1.transform.position.z - PlayerCharacter2.transform.position.z > 0)
                CalculateMoveDirection(ref character1MoveDirection.z, ref character2MoveDirection.z);
            else
                CalculateMoveDirection(ref character2MoveDirection.z, ref character1MoveDirection.z);
        }

        //各キャラを移動
        PlayerCharacter1Components.playerModel.SetCharacterMoveDirection(character1MoveDirection);
        PlayerCharacter2Components.playerModel.SetCharacterMoveDirection(character2MoveDirection);

        //ジャンプ
        if (Input.GetButton("LeftJump") && PlayerCharacter1Components.playerModel.CanJump && PlayerCharacter1.transform.position.y - PlayerCharacter2.transform.position.y < maxDistanceLength)
        {
            PlayerCharacter1Components.playerModel.CanJump = false;
            //PlayerCharacter1Components.rigidbody.AddForce(new Vector3(0, jumpPower, 0));
        }
        if (Input.GetButton("RightJump") && PlayerCharacter2Components.playerModel.CanJump && PlayerCharacter2.transform.position.y - PlayerCharacter1.transform.position.y < maxDistanceLength)
        {
            PlayerCharacter2Components.playerModel.CanJump = false;
            //PlayerCharacter2Components.rigidbody.AddForce(new Vector3(0, jumpPower, 0));
        }
    }

    /// <summary>
    /// 2体のキャラがこれ以上離れられないようにする
    /// </summary>
    /// <param name="character1MoveDirection">一体目のキャラの移動方向</param>
    /// <param name="character2MoveDirection">二体目のキャラの移動方向</param>
    void CalculateMoveDirection(ref float character1MoveDirection, ref float character2MoveDirection)
    {
        float moveVector = character2MoveDirection - character1MoveDirection;

        if (moveVector < 0f)
        {
            float moveForward = (character1MoveDirection + character2MoveDirection) / 3f;
            character1MoveDirection = moveForward;
            character2MoveDirection = moveForward;
        }
    }
    
    public static float GetMaxDistanceLength()
    {
        return maxDistanceLength;
    }

    /// <summary>
    /// 離れられる距離の上限を伸ばす
    /// </summary>
    /// <param name="extendLength">増加量</param>
    public static void ExtendMaxDistanceLength(float extendLength)
    {
        maxDistanceLength += extendLength;
    }
}
