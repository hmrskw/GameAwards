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

    [SerializeField,Range(1,10)]
    float speed;

    [SerializeField, Space(15)]
    float jumpPower;

    static float maxDistanceLength = 10;

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
                PlayerCharacter1.transform.position + new Vector3(lh * 0.1f * speed, 0, lv * 0.1f * speed),
                PlayerCharacter2.transform.position) > maxDistanceLength)
            {
                lh = 0;
                lv = 0;
            }
            PlayerCharacter1.transform.Translate(new Vector3(lh * 0.1f * speed, 0, lv * 0.1f * speed));
        }
        if (rh != 0 || rv != 0)
        {
            if (Vector3.Distance(
                PlayerCharacter1.transform.position,
                PlayerCharacter2.transform.position + new Vector3(rh * 0.1f * speed, 0, rv * 0.1f * speed)) > maxDistanceLength)
            {
                rh = 0;
                rv = 0;
            }
            PlayerCharacter2.transform.Translate(new Vector3(rh * 0.1f * speed, 0, rv * 0.1f * speed));
        }

        if (Input.GetAxis("LeftJump") > 0.5 && PlayerCharacter1Components.playerModel.CanJump)
        {
            PlayerCharacter1Components.playerModel.CanJump = false;
            PlayerCharacter1Components.rigidbody.AddForce(new Vector3(0, jumpPower, 0));
        }
        if (Input.GetAxis("RightJump") < -0.5 && PlayerCharacter2Components.playerModel.CanJump)
        {
            PlayerCharacter2Components.playerModel.CanJump = false;
            PlayerCharacter2Components.rigidbody.AddForce(new Vector3(0, jumpPower, 0));
        }

        CameraController();
    }

    private void CameraController()
    {
        Vector3 camPos = Vector3.Min(PlayerCharacter1.transform.position, PlayerCharacter2.transform.position);
        camPos.x = Mathf.Lerp(PlayerCharacter1.transform.position.x, PlayerCharacter2.transform.position.x, 0.5f);

        CameraObjct.transform.position = new Vector3(camPos.x, camPos.y + 10, camPos.z - 9);

        float dis = Vector3.Distance(PlayerCharacter1.transform.position, PlayerCharacter2.transform.position);

        if (dis > 5f)
        {
            dis -= 5f;
            CameraObjct.transform.position += new Vector3(0, dis / 1f , -dis / 1.5f);
        }
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
