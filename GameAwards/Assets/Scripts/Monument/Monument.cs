using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monument : MonoBehaviour {

    [SerializeField]
    GameObject monument;

    [SerializeField]
    public ParticleSystem particle;

    [SerializeField]
    protected GameObject guideObjct;
    //ParticleSystem guideParticle;

    [SerializeField]
    protected Monument nextMonument;

    [SerializeField]
    protected float extendLength;

    protected Animator openAnimation;
    //Material mat;

    protected bool isOn;
    public bool IsOn
    {
        private set { isOn = value; }
        get { return isOn; }
    }

    protected float animationStart;
    //Color objColor;

    void Start ()
    {
        if(guideObjct != null)guideObjct.SetActive(false);
        openAnimation = GetComponent<Animator>();
        isOn = false;

        StartCoroutine(Wait());
	}

    virtual protected IEnumerator Wait()
    {
        while (StringView.Instance.OnHitLine(transform.position) == false)
        {
            yield return null;
        }
        StringView.Instance.GrassTextureUpdate(1);
        SoundManager.Instance.PlaySE("se object");
        StartCoroutine(Boot());
    }

    virtual protected IEnumerator Boot() {
        if (isOn == false)
        {
            isOn = true;
            openAnimation.SetTrigger("Open");
            if(guideObjct != null && nextMonument != null) nextMonument.Guid();
            InputController.ExtendMaxDistanceLength(extendLength);
        }
        else if (guideObjct != null && guideObjct.activeInHierarchy == true)
        {
            guideObjct.SetActive(false);
        }

        while (
            openAnimation.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash("New State") ||
            openAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime < (40f / 45f))
        {
            yield return null;
        }
        particle.Play();
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
