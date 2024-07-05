using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bezier_viz : MonoBehaviour
{
    
    public List<Vector2> controlPoints = new List<Vector2>()
  {
    new Vector2(-5.0f, -5.0f),
    new Vector2( 0.0f,  2.0f),
    new Vector2( 5.0f, -2.0f)
  };

    
    public GameObject PointPrefab;
    LineRenderer[] mLineRenderers = null;


    List<GameObject> mPointGameObjects = new List<GameObject>();

    // Store the properties of the line.
    public float LineWidth;
    public float LineWidthBezier;
    public Color LineColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);
    public Color BezierCurveColor = new Color(0.5f, 0.6f, 0.8f, 0.8f);

    // Let's create a function to create the line renderer.
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

    // Start is called before the first frame update
    void Start()
    {
        // Here we will create the actual lines.
        mLineRenderers = new LineRenderer[2];
        mLineRenderers[0] = CreateLine();
        mLineRenderers[1] = CreateLine();

        // Set the name of these lines to distinguish.
        mLineRenderers[0].gameObject.name = "LineRenderer_obj_0";
        mLineRenderers[1].gameObject.name = "LineRenderer_obj_1";

        // Now create the instances of the control points.
        for (int i = 0; i < controlPoints.Count; i++)
        {
            GameObject obj = Instantiate(PointPrefab, controlPoints[i], Quaternion.identity);
            obj.name = "ControlPoint_" + i.ToString();
            mPointGameObjects.Add(obj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // We will now draw the lines every frame.
        LineRenderer lineRenderer = mLineRenderers[0];
        LineRenderer curveRenderer = mLineRenderers[1];

        List<Vector2> pts = new List<Vector2>();
        for (int i = 0; i < mPointGameObjects.Count; i++)
        {
            pts.Add(mPointGameObjects[i].transform.position);
        }

        // set the lineRenderer for showing the straight lines between
        // the control points.
        lineRenderer.positionCount = pts.Count;
        for (int i = 0; i < pts.Count; i++)
        {
            lineRenderer.SetPosition(i, pts[i]);
        }

        // We can now see the straight lines connecting the control points.
        // We will now proceed to draw the curve based on the bezier points.
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

    // Now let's add functionality to add new control points
    // by double clicking on the screen.
    private void OnGUI()
    {
        Event e = Event.current;
        if (e.isMouse)
        {
            if (e.clickCount == 2 && e.button == 0)
            {
                Vector2 rayPos = new Vector2(
                  Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
                  Camera.main.ScreenToWorldPoint(Input.mousePosition).y);

                InsertNewControlPoint(rayPos);
            }
        }
    }

    void InsertNewControlPoint(Vector2 p)
    {
        if (mPointGameObjects.Count >= 18)
        {
            Debug.Log("Cannot create any more control points. Max is 18");
            return;
        }

        GameObject obj = Instantiate(PointPrefab, p, Quaternion.identity);
        obj.name = "ControlPoint_" + mPointGameObjects.Count.ToString();
        mPointGameObjects.Add(obj);
    }
}
