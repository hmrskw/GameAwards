using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monument : MonoBehaviour {

    [SerializeField]
    Switch switchA;
    [SerializeField]
    Switch switchB;

    //[SerializeField,Space(15)]
    Material mat;

    [SerializeField]
    ParticleSystem particle;

    bool isOn;

    Color objColor;

    // Use this for initialization
    void Start ()
    {
        mat = GetComponent<Renderer>().material;
        objColor = mat.color;
        mat.color = Color.grey;
        isOn = false;
	}
	
	// Update is called once per frame
	void Update () {
        if(isOn == false && switchA.IsOn && switchB.IsOn)
        {
            isOn = true;
            mat.color = objColor;
            particle.Play();
        }
	}
}
