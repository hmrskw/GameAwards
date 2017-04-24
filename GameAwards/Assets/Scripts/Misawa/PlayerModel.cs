using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour {
    [SerializeField]
    LayerMask mask;

    [SerializeField]
    float gravity;

    [SerializeField]
    float slideSpeed;

    [SerializeField]
    Animator animator;

    //接地中か
    bool canJump;
    public bool CanJump {
        set { canJump = value; }
        get { return canJump; }
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
        canJump = true;	
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit;

        Debug.DrawRay(transform.position, new Vector3(0,-0.5f,0), new Color(1f, 1f, 1f));

        if (Physics.Raycast(transform.position + new Vector3(0,0.1f,0), Vector3.down, out hit, 0.5f, mask))
        {
            //ジャンプ中でなければキャラクターを地面に接地する高さにずらす
            if(canJump == true)
            {
                transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
            }

            if (Vector3.Angle(hit.normal, Vector3.up) > 20f)
                isSliding = true;
            else
                isSliding = false;

            if (isSliding)
            {
                //地面の傾斜方向を調べる
                Vector3 hitNormal = hit.normal;

                transform.Translate(new Vector3(hitNormal.x * slideSpeed, 0, hitNormal.z * slideSpeed),Space.World);
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Player"))
        {
            canJump = true;
        }
    }


}
