using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WillOTheWisp : MonoBehaviour {
    [SerializeField]
    GameObject targets;

    Transform[] targetPosition;

    [SerializeField]
    GameObject obj;

    [SerializeField]
    float extendLength;

    [SerializeField]
    GrassManager _grassManager;

	[SerializeField]
	ParticleSystem _deathSmoke;

    [SerializeField]
    ParticleSystem smoke;

    AudioSource source;

    // Use this for initialization
    void Start () {
        if (targets != null)
        {
            targetPosition = targets.GetComponentsInChildren<Transform>();
        }

        source = obj.GetComponent<AudioSource>(); 
        StartCoroutine(Amplitude());
        if (targets != null)
        {
            StartCoroutine(Move());
        }
        else {
            StartCoroutine(Wait());
        }
    }

    IEnumerator Wait()
    {
        while (StringView.Instance.OnHitLine(obj.transform.position) == false)
        {
            yield return null;
        }
        StartCoroutine(Del());
    }
    IEnumerator Del()
    {
		_deathSmoke.Play();
        SoundManager.Instance.PlaySE("shoumetu");
        while(_deathSmoke.isPlaying)
        {
            source.volume -= 0.07f;
            obj.transform.localScale = new Vector3(2 * source.volume, 2 * source.volume, 2 * source.volume);
            yield return null;
        }
        Destroy(gameObject);
    }

    IEnumerator Amplitude()
    {
        float time = 0f;
        while (true)
        {
            obj.transform.position += new Vector3(Mathf.Cos(time) * 0.05f, Mathf.Sin(time)*0.1f, 0);
            time += 0.1f;
            if (Mathf.Sin(time) == 0) time = 0;
            yield return null;
        }
    }

    IEnumerator Move()
    {
        int tartgetID = Random.Range(0, targetPosition.Length);
        obj.transform.LookAt(targetPosition[tartgetID]);

        Ray ray = new Ray(obj.transform.position, -transform.up);
        RaycastHit hit;

        while (StringView.Instance.OnHitLine(obj.transform.position) == false)
        {
            if (Vector3.Distance(obj.transform.position, targetPosition[tartgetID].position) < 5) {
                tartgetID = Random.Range(0, targetPosition.Length);
                obj.transform.LookAt(targetPosition[tartgetID]);
            }

            obj.transform.Translate(0, 0, 0.5f, Space.Self);
            ray = new Ray(obj.transform.position, -transform.up);
            
            //草を枯らす
            if (Physics.Raycast(ray, out hit, 10.0f, LayerMask.GetMask("Grass")))
            {
                if (hit.transform.tag == "GrownGrass")
                {
                    int _xIndex = 0;
                    int _zIndex = 0;
                    _grassManager.SearchDummyPointIndex(hit.point, out _xIndex, out _zIndex);
                    _grassManager.ChangeHasGrown(_xIndex, _zIndex, false);

                    var grassComponent = hit.collider.GetComponent<GrassesController>();
                    if (grassComponent != null)
                    {
                        grassComponent.Wither();
                    }
                    smoke.Play();
                }
            }
            yield return null;
        }

        InputController.ExtendMaxDistanceLength(extendLength);
        StartCoroutine(Del());
    }
}
