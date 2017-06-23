using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : MonoBehaviour {
    [SerializeField]
    LayerMask mask;

    [SerializeField]
    LayerMask buildingMask;

    [SerializeField]
    float gravity;

    [SerializeField]
    float speed;

    //[SerializeField]
    //float jumpPower;

    [SerializeField]
    float slopeAngle;

    Animator animator;

    Vector3 characterMoveForward;

    Vector3 centripetalDirection;

    [SerializeField]
    float jpower;//  = 0.49f;

    float velocity = 0;

    //接地中か
    bool canJump;
    public bool CanJump {
        set {
            velocity = jpower/speed;
            canJump = value;
        }
        get {
            return canJump;
        }
    }

    //坂道を滑っているか
    bool isSliding = false;
    public bool IsSliding
    {
        set { isSliding = value; }
        get { return isSliding; }
    }

    bool isPulled = false;
    public bool IsPulled
    {
        set {isPulled = value; }
        get { return isPulled; }
    }

    bool isStop = false;
    public bool IsStop
    {
        set { isStop = value; }
        get { return isStop; }
    }

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        canJump = false;
    }

    // Update is called once per frame
    void Update () {

        //Vector3 slope = 
        Sliding();
        
        if (canJump == false)
        {
            velocity += Physics.gravity.y * Time.fixedDeltaTime;
            if (velocity < -2) velocity = -2;
        }

        //移動している間アニメーションを動かす
        animator.SetBool("IsWalk", (characterMoveForward != Vector3.zero) && !isPulled || StringView.Instance.IsSpin);
        animator.SetBool("IsJump", (!canJump));
        animator.SetBool("IsPulled", (isPulled));

        animator.SetBool("Hit", Physics.Raycast(transform.position + new Vector3(0, 2f, 0), characterMoveForward/*,out hit*/, 5f, buildingMask));

        if (animator.GetCurrentAnimatorStateInfo(0).shortNameHash != Animator.StringToHash("Butukaru"))
        {
            //移動方向を向かせる(移動方向+坂の傾き)
            if (!isPulled)
            {
                transform.LookAt(
                    transform.position +
                    new Vector3(characterMoveForward.x, 0f, characterMoveForward.z) +
                    //new Vector3(slope.x, 0f, slope.z) +
                    new Vector3(centripetalDirection.x, 0f, centripetalDirection.z));
            }
            else
            {
                transform.LookAt((
                    transform.position -
                    new Vector3(characterMoveForward.x, 0f, characterMoveForward.z) -
                    //new Vector3(slope.x, 0f, slope.z) -
                    new Vector3(centripetalDirection.x, 0f, centripetalDirection.z)));
            }
            //RaycastHit hit;

            // 移動方向にスピードを掛けたものに、坂を滑り落ちる速度と向心力を加算
            transform.Translate(((characterMoveForward + new Vector3(/*slope.x*/0, velocity,0/* slope.z*/)) * speed) + centripetalDirection, Space.World);
        }
        else if(Physics.Raycast(transform.position + new Vector3(0, 2f, 0), characterMoveForward/*,out hit*/, 5f, buildingMask))
        {
            transform.Translate(-(((characterMoveForward + new Vector3(/*slope.x*/0, velocity,0/* slope.z*/)) * speed) + centripetalDirection), Space.World);
        }
        centripetalDirection = Vector3.zero;
    }

    /// <summary>
    /// 自分がたっている地面が坂かを調べる
    /// </summary>
    /// <returns>傾斜角度</returns>
    Vector3 Sliding()
    {
        RaycastHit hit;

        //Debug.DrawRay(transform.position + new Vector3(0, 3f, 0), Vector3.down, Color.red);
        if (Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), Vector3.down, out hit, slopeAngle / 10f, mask))
        {
            //ジャンプ中でなければキャラクターを地面に接地する高さにずらす
            if (canJump == true)
            {
                transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
            }

            isSliding = (Vector3.Angle(hit.normal, Vector3.up) > slopeAngle);
        }
        //Debug.Log(Vector3.Angle(hit.normal, Vector3.up));
        //地面の傾斜方向を調べる
        return hit.normal;
    }
    
    /// <summary>
    /// 踏み台になるものに乗っているか調べる
    /// </summary>
    /// <param name="other">当たったオブジェクト</param>
    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Player"))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + new Vector3(0, 5f, 0), Vector3.down, out hit, 6f, mask))
            {
                transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
                canJump = true;

                velocity = 0;
            }
        }
    }
    
    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Player"))
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position + new Vector3(0f,5f,0f), Vector3.down, out hit, 5.5f, mask))
            {
                transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
            }
            else {
                canJump = false;
            }
        }
    }

    /// <summary>
    /// 向心力の計算
    /// </summary>
    public void Centripetal(float distance,Vector3 direction ,Vector3 characterMoveDirection)
    {
        float f = (/*speed * speed*/1) / distance;

        float num = (Vector3.Angle(Vector3.Cross(-characterMoveDirection,Vector3.up), direction * f)-90f)/4.5f*speed;
        centripetalDirection = Vector3.Cross(direction * f,Vector3.up)* num;
    }

    /// <summary>
    /// 移動方向を取得する
    /// </summary>
    /// <param name="moveForward">移動方向</param>
    public void SetCharacterMoveDirection(Vector3 moveForward)
    {
        characterMoveForward = moveForward;
    }
}
