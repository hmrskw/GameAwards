using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringView : MonoBehaviour {
    [SerializeField]
    GameObject p1;

    [SerializeField]
    GameObject p2;

    LineRenderer line;

    // Use this for initialization
    void Start () {
	    line = gameObject.GetComponent<LineRenderer>();
        point = Vector3.Lerp(head.position, tail.position, 0.5f);

    }
    /*
    // Update is called once per frame
    void Update()
    {

        line.SetPosition(0, p1.transform.position);
        line.SetPosition(1, p2.transform.position);
    }

    */
    [Range(0, 1)]
    public float t;
    /// LineRenderer
    public LineRenderer lineRenderer;
    /// 始点
    public Transform head;
    /// 制御点
    //public Transform top;
    /// 終点
    public Transform tail;

    //public Queue<Vector3> top = new Queue<Vector3>();

    public void AddTop(Vector3 topPos)
    {
        //top.Enqueue(topPos);
    }

    // Update is called once per frame
    Vector3 BezierCurve(Vector3 pt1, Vector3 pt2, Vector3 ctrlPt, float t)
    {
        if (t > 1.0f)
            t = 1.0f;

        Vector3 result = new Vector3();
        float cmp = 1.0f - t;
        result.x = cmp * cmp * pt1.x + 2 * cmp * t * ctrlPt.x + t * t * pt2.x;
        result.y = cmp * cmp * pt1.y + 2 * cmp * t * ctrlPt.y + t * t * pt2.y;
        result.z = cmp * cmp * pt1.z + 2 * cmp * t * ctrlPt.z + t * t * pt2.z;
        return result;
    }
    int co = 0;
    Vector3 point;

    void Update()
    {
        var posList = new List<Vector3>();

        posList.Add(head.position);
        float length = 0f;

        co++;

        if (co % 10 == 0)
        {
            point = Vector3.Lerp(point, Vector3.Lerp(head.position, tail.position, 0.5f), 0.5f);
        }

        while (length < 1f)
        {
            length += 0.1f;
            //if (top.Count > 0)
            {
                posList.Add(
                    BezierCurve(
                        head.position,
                        tail.position,
                        point,
                        length
                    )
                );
            }
            //else
            //{
            //    posList.Add(
            //        BezierCurve(
            //            head.position,
            //            tail.position,
            //            Vector3.Lerp(head.position, tail.position, 0.5f),
            //            length
            //        )
            //    );
            //}
        }

        lineRenderer.positionCount = posList.Count;
        lineRenderer.SetPositions(posList.ToArray());
    }
    
}
