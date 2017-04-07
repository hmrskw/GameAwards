using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour {

    bool canJump;
    public bool CanJump {
        set { canJump = value; }
        get { return canJump; }
    }

	// Use this for initialization
	void Start () {
        canJump = true;	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Player"))
        {
            canJump = true;
        }
    }
}
