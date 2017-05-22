using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monument : MonoBehaviour {

    [SerializeField]
    GameObject monument;

    [SerializeField]
    ParticleSystem particle;
    [SerializeField]
    ParticleSystem guideParticle;

    [SerializeField]
    Monument nextMonument;

    [SerializeField]
    float extendLength;

    Material mat;

    bool isOn;

    Color objColor;

    void Start ()
    {
        mat = monument.GetComponent<Renderer>().material;
        objColor = mat.color;
        mat.color = Color.grey;
        isOn = false;

        StartCoroutine(Wait());
	}

    IEnumerator Wait()
    {
        while (StringView.Instance.OnHitLine(transform.position) == false)
        {
            yield return null;
        }
        SoundManager.Instance.PlaySE("se object");
        Boot();
    }
	
    public void Boot() {
        if (isOn == false)
        {
            isOn = true;
            mat.color = objColor;
            particle.Play();
            if(nextMonument != null) nextMonument.Guid();
            InputController.ExtendMaxDistanceLength(extendLength);
        }
        else if (guideParticle.isPlaying)
        {
            guideParticle.Stop();
        }
    }

    public void Guid()
    {
        if(isOn == false && guideParticle.isPlaying == false)
        {
            guideParticle.Play();
        }
    }
}
