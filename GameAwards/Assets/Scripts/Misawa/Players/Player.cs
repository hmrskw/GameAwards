using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : MonoBehaviour {
    [SerializeField]
    LayerMask mask;

    [SerializeField]
    float gravity;

    [SerializeField]
    float speed;

    [SerializeField]
    float jumpPower;

    [SerializeField]
    float slopeAngle;

    Animator animator;

    Vector3 characterMoveForward;

    [SerializeField]
    Material mat;


    [SerializeField]
    float jpower = 0.49f;

    float velocity = 0;

    //接地中か
    bool canJump;
    public bool CanJump {
        set {
            velocity = jpower;
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

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        canJump = false;
    }

    // Update is called once per frame
    void Update () {

        Vector3 slope = Sliding();

        
        if (canJump == false)
        {
            velocity += Physics.gravity.y * Time.fixedDeltaTime;
            if (velocity < -2) velocity = -2;
        }

        //DEBUG:キャラのステートに合わせて色を変える
        if (isSliding)
        {
            mat.color = Color.red;
        }
        else if (canJump == false)
        {
            mat.color = Color.blue;
        }
        else
        {
            slope = Vector3.zero;
            mat.color = Color.gray;
        }
        //移動している間アニメーションを動かす
        animator.SetBool("IsWalk", (characterMoveForward != Vector3.zero));
        animator.SetBool("IsJump", (!canJump));

        //移動方向を向かせる(移動方向+坂の傾き)
        transform.LookAt(transform.position + new Vector3(characterMoveForward.x, 0f, characterMoveForward.z) + (new Vector3(slope.x, 0f, slope.z) * speed / 10f));
        //Debug.Log(((characterMoveForward + new Vector3(slope.x, velocity, slope.z)) * speed));
        // 移動方向にスピードを掛けたものに、坂を滑り落ちる速度を加算
        transform.Translate(((characterMoveForward + new Vector3(slope.x , velocity, slope.z)) * speed), Space.World);
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
            }
            canJump = true;
            
            velocity = 0;
            
        }
    }

    /*void OnTriggerEnter(Collider col)
    {
        Debug.Log("hit");

        velocity = 0;
        canJump = true;

        Vector3 hitPoint = col.ClosestPointOnBounds(transform.position);

        Debug.Log(hitPoint + " === " + transform.position);
        Vector3 a = hitPoint - transform.position;
        Debug.Log("a = " + a);

        a = a.normalized;

        Debug.Log("a.normalized = " + a);

        Debug.Log("point" + hitPoint + "a" + a);
        Plane p = new Plane(a,hitPoint);
        
        float dis = p.GetDistanceToPoint(transform.position);

        Debug.Log(dis);

        transform.position += a * dis;
    }*/
    
    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Player"))
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position+new Vector3(0, 5f, 0), Vector3.down, out hit, 6f, mask) == false)
                canJump = false;
        }
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
