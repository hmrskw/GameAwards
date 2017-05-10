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

    AudioSource source;

    // Use this for initialization
    void Start () {
        targetPosition = targets.GetComponentsInChildren<Transform>();
        source = obj.GetComponent<AudioSource>(); 
        StartCoroutine(Amplitude());
        StartCoroutine(Move());
    }

    /*public void Absorbed()
    {
        InputController.ExtendMaxDistanceLength(5);
        StopAllCoroutines();
        StartCoroutine(Del());
    }*/

    IEnumerator Del()
    {
        while(source.volume > 0)
        {
            source.volume -= 0.1f;
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
        while (StringView.Instance.OnHitLine(obj.transform.position) == false)
        {
            if (Vector3.Distance(obj.transform.position, targetPosition[tartgetID].position) < 5) {
                tartgetID = Random.Range(0, targetPosition.Length);
                obj.transform.LookAt(targetPosition[tartgetID]);
            }
            obj.transform.Translate(0, 0, 0.1f, Space.Self);
            yield return null;
        }
        InputController.ExtendMaxDistanceLength(extendLength);
        StartCoroutine(Del());
    }
}
