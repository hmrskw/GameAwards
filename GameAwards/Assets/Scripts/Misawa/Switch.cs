using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour {

    bool isOn;
    public bool IsOn
        {get{ return isOn; }}

    int onCharacterNum;


    //[SerializeField]
    Material mat;


	// Use this for initialization
	void Start ()
    {
        mat = GetComponent<Renderer>().material;
        mat.color = Color.grey;
        onCharacterNum = 0;
        isOn = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            onCharacterNum++;
            mat.color = Color.green;
            isOn = true;
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            onCharacterNum--;
            if (onCharacterNum == 0)
            {
                mat.color = Color.grey;
                isOn = false;
            }
        }
    }
}
