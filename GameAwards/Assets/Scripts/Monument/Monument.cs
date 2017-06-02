using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monument : MonoBehaviour {

    [SerializeField]
    GameObject monument;

    [SerializeField]
    ParticleSystem particle;

    [SerializeField]
    GameObject guideObjct;
    //ParticleSystem guideParticle;

    [SerializeField]
    Monument nextMonument;

    [SerializeField]
    float extendLength;

    Material mat;

    bool isOn;
    public bool IsOn
    {
        private set { isOn = value; }
        get { return isOn; }
    }

    Color objColor;

    void Start ()
    {
        guideObjct.SetActive(false);
        mat = monument.GetComponent<Renderer>().material;
        //objColor = mat.color;
        //mat.color = Color.grey;
        isOn = false;

        StartCoroutine(Wait());
	}

    IEnumerator Wait()
    {
        while (StringView.Instance.OnHitLine(transform.position) == false)
        {
            yield return null;
        }
        StringView.Instance.GrassTextureUpdate(1);
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
        else if (guideObjct.activeInHierarchy == true/*guideParticle.isPlaying*/)
        {
            guideObjct.SetActive(false);
            //guideParticle.Stop();
        }
    }

    public void Guid()
    {
        if(isOn == false && guideObjct.activeInHierarchy == false/*guideParticle.isPlaying == false*/)
        {
            guideObjct.SetActive(true);
            //guideParticle.Play();
        }
    }
}
