using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezCurveTemplate : MonoBehaviour
{
    public static readonly List<Vector2> templateControlPoints = new List<Vector2>()
      {
          new Vector2(0, 0),
          new Vector2(31, 17),
          new Vector2(47, 13),
          new Vector2(45, 3),
          new Vector2(46, -1),
          new Vector2(23, -5),
          new Vector2(15, -18),
          new Vector2(36, -20),
          new Vector2(64, -20),
          new Vector2(85, -18),
          new Vector2(75, -5),
          new Vector2(52, 0),
          new Vector2(55, 5),
          new Vector2(53, 13),
          new Vector2(65, 15),
          new Vector2(100, 0)
      };

    
    public GameObject PointPrefab;
    LineRenderer[] linerenderers = null;

    List<GameObject> goPoints = new List<GameObject>();

    public float LineWidth;
    public float LineWidthBezier;
    public Color LineColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);
    public Color BezierCurveColor = new Color(0.5f, 0.6f, 0.8f, 0.8f);

    private LineRenderer CreateLine()
    {
        GameObject obj = new GameObject();
        LineRenderer lr = obj.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = LineColor;
        lr.endColor = LineColor;
        lr.startWidth = LineWidth;
        lr.endWidth = LineWidth;
        return lr;
    }

    void Start()
    {
        linerenderers = new LineRenderer[2];
        linerenderers[0] = CreateLine();
        linerenderers[1] = CreateLine();

        linerenderers[0].gameObject.name = "LineRenderer_obj_0";
        linerenderers[1].gameObject.name = "LineRenderer_obj_1";

        for (int i = 0; i < templateControlPoints.Count; i++)
        {
            GameObject obj = Instantiate(PointPrefab, templateControlPoints[i], Quaternion.identity);
            obj.name = "ControlPoint_" + i.ToString();
            goPoints.Add(obj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        LineRenderer lineRenderer = linerenderers[0];
        LineRenderer curveRenderer = linerenderers[1];

        List<Vector2> pts = new List<Vector2>();
        for (int i = 0; i < goPoints.Count; i++)
        {
            pts.Add(goPoints[i].transform.position);
        }

        lineRenderer.positionCount = pts.Count;
        for (int i = 0; i < pts.Count; i++)
        {
            lineRenderer.SetPosition(i, pts[i]);
        }

        List<Vector2> curve = BezierCurve.PointList2(pts, 0.01f);
        curveRenderer.startColor = BezierCurveColor;
        curveRenderer.endColor = BezierCurveColor;
        curveRenderer.positionCount = curve.Count;
        curveRenderer.startWidth = LineWidthBezier;
        curveRenderer.endWidth = LineWidthBezier;

        for (int i = 0; i < curve.Count; i++)
        {
            curveRenderer.SetPosition(i, curve[i]);
        }
    }
}
