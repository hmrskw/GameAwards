using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringView : MonoBehaviour {
    static StringView instance;

    public static StringView Instance
    {
        get { return instance; }
    }

    LineRenderer lineRenderer;

    [SerializeField,Tooltip("始点")]
    Transform head;

    [SerializeField, Tooltip("終点")]
    Transform tail;

    int co = 0;
    Vector3 point;

    float coefficient;

    [SerializeField]
    LayerMask[] mask;

	[SerializeField]
	GrassManager _grassManager;

    [SerializeField]
    Material[] mats;

    Ray ray = new Ray();

    RaycastHit hit;

    public bool isSpin = false;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    // Use this for initialization
    void Start () {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        point = Vector3.Lerp(head.position, tail.position, 0.5f);
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
        if (isSpin)
        {
            lineRenderer.material = mats[1];
        }
        else
        {
            lineRenderer.material = mats[0];
        }

        var posList = new List<Vector3>();

        posList.Add(head.position + new Vector3(0, 3, 0));
        float length = 0f;

        co++;

        point = Vector3.Lerp(point, Vector3.Lerp(head.position, tail.position, 0.5f), 0.1f * Vector3.Distance(head.position, tail.position) / (InputController.GetMaxDistanceLength()));

        if (co % 60 == 0)
        {
            co = 0;
        }

        while (length < 1f)
        {
            length += 0.05f;
            {
                posList.Add(
                    B_SplineCurve(
                        head.position + new Vector3(0, 3, 0),
                        tail.position + new Vector3(0, 3, 0),
                        point + new Vector3(0, 3, 0),
                        length
                    )
                );
            }
        }
        
        lineRenderer.positionCount = posList.Count;
        lineRenderer.SetPositions(posList.ToArray());
        OnPassLine();
    }
    
    void OnPassLine()
    {
        float length = 0f;

        while (length < 1f)
        {
            length += 0.01f;
            {
                Vector3 curve =
                    B_SplineCurve(
                        head.position + new Vector3(0, 3, 0),
                        tail.position + new Vector3(0, 3, 0),
                        point + new Vector3(0, 3, 0),
                        length
                    );

                ray.origin = curve;
                ray.direction = -transform.up;

                /*if (Physics.Raycast(ray, out hit, 10.0f,LayerMask.GetMask("Monument")))
                {
                    hit.collider.GetComponent<Monument>().Boot();
                }*/

                if (Physics.Raycast(ray, out hit, 10.0f, LayerMask.GetMask("Grass")))
                {
					int _xIndex = 0;
					int _zIndex = 0;
					_grassManager.SearchDummyPointIndex(hit.point, out _xIndex, out _zIndex);

					if (hit.transform.tag == "WitheredGrass")
					{
						_grassManager.ChangeHasGrown(_xIndex, _zIndex, true);

						var grassComponent = hit.collider.GetComponent<GrassesController>();
                        if (grassComponent != null)
                        {
                            grassComponent.Growth();
                        }
					}
                }
			}
        }
    }

    public bool OnHitLine(Vector3 position)
    {
        float length = 0f;

        while (length < 1f)
        {
            length += 0.1f;
            {
                Vector3 curve =
                    B_SplineCurve(
                        head.position + new Vector3(0, 3, 0),
                        tail.position + new Vector3(0, 3, 0),
                        point + new Vector3(0, 3, 0),
                        length
                    );
                if (Vector3.Distance(curve, position) < 2) return true;
            }
        }
        return false;
    }
}
