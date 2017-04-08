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

    [SerializeField, Space(15)]
    float jumpPower;

    [SerializeField]
    StringView sv;

    PlayerComponents PlayerCharacter1Components = new PlayerComponents();
    PlayerComponents PlayerCharacter2Components = new PlayerComponents();

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
        lh = Input.GetAxis("LeftHorizontal");
        lv = Input.GetAxis("LeftVertical");
        rh = Input.GetAxis("RightHorizontal");
        rv = Input.GetAxis("RightVertical");

        if (lh != 0 || lv != 0)
        {
            if (Vector3.Distance(
                PlayerCharacter1.transform.position + new Vector3(lh * 0.1f, 0, lv * 0.1f),
                PlayerCharacter2.transform.position) > 10)
            {
                lh = 0;
                lv = 0;
            }
            PlayerCharacter1.transform.Translate(new Vector3(lh * 0.1f, 0, lv * 0.1f));
        }
        if (rh != 0 || rv != 0)
        {
            if (Vector3.Distance(
                PlayerCharacter1.transform.position,
                PlayerCharacter2.transform.position + new Vector3(rh * 0.1f, 0, rv * 0.1f)) > 10)
            {
                rh = 0;
                rv = 0;
            }
            PlayerCharacter2.transform.Translate(new Vector3(rh * 0.1f, 0, rv * 0.1f));
        }
        //sv.drawLine();

        if (Input.GetButtonDown("LeftJump") && PlayerCharacter1Components.playerModel.CanJump)
        {
            PlayerCharacter1Components.playerModel.CanJump = false;
            PlayerCharacter1Components.rigidbody.AddForce(new Vector3(0, jumpPower, 0));
        }
        if (Input.GetButtonDown("RightJump") && PlayerCharacter2Components.playerModel.CanJump)
        {
            PlayerCharacter2Components.playerModel.CanJump = false;
            PlayerCharacter2Components.rigidbody.AddForce(new Vector3(0, jumpPower, 0));
        }

        CameraController();
    }

    private void CameraController()
    {
        Vector3 camPos = Vector3.Lerp(PlayerCharacter1.transform.position, PlayerCharacter2.transform.position, 0.5f);
        CameraObjct.transform.position = new Vector3(camPos.x, 10, camPos.z - 9);

        Vector2 pc1Pos = new Vector2(PlayerCharacter1.transform.position.x, PlayerCharacter1.transform.position.z);
        Vector2 pc2Pos = new Vector2(PlayerCharacter2.transform.position.x, PlayerCharacter2.transform.position.z);
        float dis = Vector2.Distance(pc1Pos,pc2Pos);
        if (dis > 10f)
        {
            dis -= 10f;
            CameraObjct.transform.position += new Vector3(0, dis / 2, -dis / 2);
        }
    }
}
