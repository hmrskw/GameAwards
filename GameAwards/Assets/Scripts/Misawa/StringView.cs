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
    /// 終点
    public Transform tail;

    int co = 0;
    Vector3 point;

    float coefficient;

    public void AddTop(Vector3 topPos)
    {
        //top.Enqueue(topPos);
    }

    /// <summary>
    /// B-スプライン曲線における 2 次元座標を返します
    /// </summary>
    /// <param name="p1">開始点の座標</param>
    /// <param name="p2">制御点の座標</param>
    /// <param name="p3">終点の座標</param>
    /// <param name="t">重み(0 から 1)</param>
    /// <returns>B-スプライン曲線における 2 次元座標</returns>
    public static Vector3 B_SplineCurve(Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        return new Vector3(
            ((1 - t) * (1 - t) * p1.x + 2 * t * (1 - t) * p3.x + t * t * p2.x),
            ((1 - t) * (1 - t) * p1.y + 2 * t * (1 - t) * p3.y + t * t * p2.y),
            ((1 - t) * (1 - t) * p1.z + 2 * t * (1 - t) * p3.z + t * t * p2.z)
        );
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

    void Update()
    {
        var posList = new List<Vector3>();

        posList.Add(head.position);
        float length = 0f;

        co++;

        //coefficient = (1.5f - (co/20f));

        point = Vector3.Lerp(point, Vector3.Lerp(head.position, tail.position, 0.5f), /*(0.5f - (co / 60f))*/0.005f * Vector2.Distance(head.position, tail.position));

        if (co % 60 == 0)
        {
            co = 0;
            //point = Vector3.Lerp(point, Vector3.Lerp(head.position, tail.position, 0.5f), 0.5f);
        }

        while (length < 1f)
        {
            length += 0.1f;
            {
                posList.Add(
                    B_SplineCurve(
                        head.position,
                        tail.position,
                        point,
                        length
                    )
                );
            }
        }

        lineRenderer.positionCount = posList.Count;
        lineRenderer.SetPositions(posList.ToArray());
    }
    
}
