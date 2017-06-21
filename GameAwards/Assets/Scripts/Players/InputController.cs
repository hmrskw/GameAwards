using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {
    class PlayerComponents
    {
        //public Rigidbody rigidbody;
        public Player playerModel;
    }
    enum PulledCharacter
    {
        NONE,CHARACTER1, CHARACTER2
    }

    PulledCharacter pulledCharacter = PulledCharacter.NONE;

    [SerializeField, Header("プレイヤー")]
    GameObject PlayerCharacter1;

    [SerializeField]
    GameObject PlayerCharacter2;

    [SerializeField]
    GameObject CameraPivot;

    static float maxDistanceLength = 10;

    PlayerComponents PlayerCharacter1Components = new PlayerComponents();
    PlayerComponents PlayerCharacter2Components = new PlayerComponents();

    void Awake()
    {
        Application.targetFrameRate = 30;
    }

    // Use this for initialization
    void Start () {
        PlayerCharacter1Components.playerModel = PlayerCharacter1.GetComponent<Player>();

        PlayerCharacter2Components.playerModel = PlayerCharacter2.GetComponent<Player>();

        SoundManager.Instance.PlayBGM("kankyou hiru");
    }

    // Update is called once per frame
    void Update() {
        //スティックの入力を受け取る
        var character1Horizontal = Input.GetAxis("LeftHorizontal");
        var character1Vertical = Input.GetAxis("LeftVertical");
        var character2Horizontal = Input.GetAxis("RightHorizontal");
        var character2Vertical = Input.GetAxis("RightVertical");

        pulledCharacter = PulledCharacter.NONE;
        StringView.Instance.IsSpin = false;

        // カメラの方向から、X-Z平面の単位ベクトルを取得
        Vector3 cameraForward = Vector3.Scale(CameraPivot.transform.forward, new Vector3(1, 0, 1)).normalized;

        //キャラクターの移動方向を計算
        Vector3 character1MoveDirection = cameraForward * character1Vertical + CameraPivot.transform.right * character1Horizontal;
        Vector3 character2MoveDirection = cameraForward * character2Vertical + CameraPivot.transform.right * character2Horizontal;

        var distance = Vector3.Distance(
            Vector3.Scale(PlayerCharacter1.transform.position, new Vector3(1, 0, 1)),
            Vector3.Scale(PlayerCharacter2.transform.position, new Vector3(1, 0, 1)));

        PlayerCharacter1Components.playerModel.IsStop = false;
        PlayerCharacter2Components.playerModel.IsStop = false;

        if (distance > maxDistanceLength - 4f)
        {
            var LinputMoveDirection = character1MoveDirection;
            var RinputMoveDirection = character2MoveDirection;

            //TEST:糸を伸ばした状態で回転すると糸に特殊な判定
            var inputDot = Vector3.Dot(LinputMoveDirection, RinputMoveDirection);

            if (inputDot < -0.5f)
            {
                StringView.Instance.IsSpin = true;

                var heading = PlayerCharacter2.transform.position - PlayerCharacter1.transform.position;
                var dis = heading.magnitude;
                var direction = heading / dis;
                PlayerCharacter1Components.playerModel.Centripetal(distance, direction, RinputMoveDirection);

                heading = PlayerCharacter1.transform.position - PlayerCharacter2.transform.position;
                dis = heading.magnitude;
                direction = heading / dis;
                PlayerCharacter2Components.playerModel.Centripetal(distance, direction, LinputMoveDirection);
            }
        }

        //糸の上限値以上離れようとしたら移動方向を制御する
        if (distance > maxDistanceLength)
        {
            var LinputMoveDirection = character1MoveDirection;
            var RinputMoveDirection = character2MoveDirection;
            
            //移動量を各キャラの平均にする
            if (PlayerCharacter1.transform.position.x - PlayerCharacter2.transform.position.x > 0)
                CalculateMoveDirection(ref character1MoveDirection.x, ref character2MoveDirection.x);
            else
                CalculateMoveDirection(ref character2MoveDirection.x, ref character1MoveDirection.x);

            if (PlayerCharacter1.transform.position.z - PlayerCharacter2.transform.position.z > 0)
                CalculateMoveDirection(ref character1MoveDirection.z, ref character2MoveDirection.z);
            else
                CalculateMoveDirection(ref character2MoveDirection.z, ref character1MoveDirection.z);

            if (LinputMoveDirection.x == 0 && LinputMoveDirection.z == 0)
            {
                if (character1MoveDirection.x != 0f || character1MoveDirection.z != 0f)
                    pulledCharacter = PulledCharacter.CHARACTER1;
            }
            else if (RinputMoveDirection.x == 0 && RinputMoveDirection.z == 0)
            {
                if (character2MoveDirection.x != 0f || character2MoveDirection.z != 0f)
                    pulledCharacter = PulledCharacter.CHARACTER2;
            }

            //向心力を使う
            if (pulledCharacter == PulledCharacter.CHARACTER1){
                var heading = PlayerCharacter2.transform.position - PlayerCharacter1.transform.position;
                var dis = heading.magnitude;
                var direction = heading / dis;
                PlayerCharacter1Components.playerModel.Centripetal(distance,direction, character2MoveDirection);
            }
            else if (pulledCharacter == PulledCharacter.CHARACTER2)
            {
                var heading = PlayerCharacter1.transform.position - PlayerCharacter2.transform.position;
                var dis = heading.magnitude;
                var direction = heading / dis;
                PlayerCharacter2Components.playerModel.Centripetal(distance, direction, character1MoveDirection);
            }
        }

        PlayerCharacter1Components.playerModel.IsPulled = (pulledCharacter == PulledCharacter.CHARACTER1);
        PlayerCharacter2Components.playerModel.IsPulled = (pulledCharacter == PulledCharacter.CHARACTER2);

        //各キャラを移動
        PlayerCharacter1Components.playerModel.SetCharacterMoveDirection(character1MoveDirection);
        PlayerCharacter2Components.playerModel.SetCharacterMoveDirection(character2MoveDirection);

        if (StringView.Instance.isPlayCutScene == false &&
            (PlayerCharacter1Components.playerModel.CanJump == true && PlayerCharacter2Components.playerModel.CanJump == true) &&
            (character1MoveDirection != Vector3.zero || character2MoveDirection != Vector3.zero))
        {
            if (SoundManager.Instance.IsPlayBGM("asioto") == false)
            {
                SoundManager.Instance.PlayBGM("asioto");
            }
        }
        else
        {
            if (SoundManager.Instance.IsPlayBGM("asioto") == true)
            {
                SoundManager.Instance.StopBGM("asioto");
            }
        }

        //ジャンプ
        if (Input.GetButton("LeftJump") && PlayerCharacter1Components.playerModel.CanJump &&
            PlayerCharacter1.transform.position.y - PlayerCharacter2.transform.position.y < maxDistanceLength)
        {
            PlayerCharacter1Components.playerModel.CanJump = false;
            SoundManager.Instance.PlaySE("jump");
        }
        if (Input.GetButton("RightJump") && PlayerCharacter2Components.playerModel.CanJump &&
            PlayerCharacter2.transform.position.y - PlayerCharacter1.transform.position.y < maxDistanceLength)
        {
            PlayerCharacter2Components.playerModel.CanJump = false;
            SoundManager.Instance.PlaySE("jump");
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
            float moveForward = (character1MoveDirection + character2MoveDirection) / 2f;
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
