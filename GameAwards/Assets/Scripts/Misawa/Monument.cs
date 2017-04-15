using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monument : MonoBehaviour {

    [SerializeField]
    GameObject monument;

    [SerializeField]
    ParticleSystem particle;

    Material mat;

    bool isOn;

    Color objColor;

    void Start ()
    {
        mat = monument.GetComponent<Renderer>().material;
        objColor = mat.color;
        mat.color = Color.grey;
        isOn = false;
	}
	
    public void Boot() {
        if (isOn == false)
        {
            isOn = true;
            mat.color = objColor;
            particle.Play();
        }
    }
}
