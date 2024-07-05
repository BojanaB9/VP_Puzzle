using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Tile;

public class Tile
{
    public enum Directions { Up, Down, Left, Right }

    public enum CurveType { Positive, Negative, Edge }

    //public Vector2Int curveOffset = new Vector2Int(20, 20);

    public static int tileSize = 100;
    public static int padding = 20;

    private Dictionary<(Directions, CurveType), LineRenderer> lineRender = new Dictionary<(Directions, CurveType), LineRenderer>();

    public static List<Vector2> bezCurve = BezierCurve.PointList2(BezCurveTemplate.templateControlPoints, 0.001f);

    public Texture2D ogtexture;

    public Texture2D finaltexture { get; private set; }

    public static readonly Color transparent = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    public bool[,] isVisited;

    public Stack<Vector2Int> visitedPixels_stack = new Stack<Vector2Int>();

    public int pixelX = 0;
    public int pixelY = 0;

    public static Sorting tilesorting = new Sorting();

    private CurveType[] curveTypes = new CurveType[4]
    {
        CurveType.Edge, CurveType.Edge, CurveType.Edge, CurveType.Edge,
    };

    public void SetCurveType(Directions dir, CurveType type)
    {
        curveTypes[(int)dir] = type;
    }

    public CurveType GetCurveType(Directions dir)
    {
        return curveTypes[(int)dir];
    }

    public Tile(Texture2D texture)
    {
        ogtexture = texture;
        int sizeWithpadding = 2 * padding + tileSize;

        finaltexture = new Texture2D(sizeWithpadding, sizeWithpadding, TextureFormat.ARGB32, false);
        for (int i = 0; i < sizeWithpadding; ++i)
        {
            for (int j = 0; j < sizeWithpadding; ++j)
            {
                finaltexture.SetPixel(i, j, transparent);
            }
        }


    }
    public void Apply()
    {
        FloodFillInit();
        FloodFill();
        finaltexture.Apply();
    }
    void FloodFillInit()
    {
        //int padding = mOffset.x;
        int tileSizeWithPadding = 2 * padding + tileSize;

        isVisited = new bool[tileSizeWithPadding, tileSizeWithPadding];
        for (int i = 0; i < tileSizeWithPadding; ++i)
        {
            for (int j = 0; j < tileSizeWithPadding; ++j)
            {
                isVisited[i, j] = false;
            }
        }

        List<Vector2> pts = new List<Vector2>();
        for (int i = 0; i < curveTypes.Length; ++i)
        {
            pts.AddRange(MakeCurve((Directions)i, curveTypes[i]));
        }


        for (int i = 0; i < pts.Count; ++i)
        {
            isVisited[(int)pts[i].x, (int)pts[i].y] = true;
        }

        Vector2Int start = new Vector2Int(tileSizeWithPadding / 2, tileSizeWithPadding / 2);

        isVisited[start.x, start.y] = true;
        visitedPixels_stack.Push(start);
    }

    void Fill(int x, int y)
    {
        Color c = ogtexture.GetPixel(x + pixelX * tileSize, y + pixelY * tileSize);
        c.a = 1.0f;
        finaltexture.SetPixel(x, y, c);
    }

    void FloodFill()
    {
        //int padding = mOffset.x;
        int width_height = padding * 2 + tileSize;

        while (visitedPixels_stack.Count > 0)
        {
            Vector2Int pixelCoordinate = visitedPixels_stack.Pop();

            int xx = pixelCoordinate.x;
            int yy = pixelCoordinate.y;

            Fill(pixelCoordinate.x, pixelCoordinate.y);



            // Right
            int x = xx + 1;
            int y = yy;

            if (x < width_height)
            {
                Color c = finaltexture.GetPixel(x, y);
                if (!isVisited[x, y])
                {
                    isVisited[x, y] = true;
                    visitedPixels_stack.Push(new Vector2Int(x, y));
                }
            }

            // Left
            x = xx - 1;
            y = yy;
            if (x > 0)
            {
                Color c = finaltexture.GetPixel(x, y);
                if (!isVisited[x, y])
                {
                    isVisited[x, y] = true;
                    visitedPixels_stack.Push(new Vector2Int(x, y));
                }
            }
            // Up
            x = xx;
            y = yy + 1;

            if (y < width_height)
            {
                Color c = finaltexture.GetPixel(x, y);
                if (!isVisited[x, y])
                {
                    isVisited[x, y] = true;
                    visitedPixels_stack.Push(new Vector2Int(x, y));
                }
            }

            // Down
            x = xx;
            y = yy - 1;

            if (y >= 0)
            {
                Color c = finaltexture.GetPixel(x, y);
                if (!isVisited[x, y])
                {
                    isVisited[x, y] = true;
                    visitedPixels_stack.Push(new Vector2Int(x, y));
                }
            }


        }
    }
    public static LineRenderer CreateLR(UnityEngine.Color color, float width = 1.0f)
    {
        GameObject gameObject = new GameObject();
        LineRenderer lr = gameObject.AddComponent<LineRenderer>();

        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = width;
        lr.endWidth = width;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        return lr;
    }

    public static void TranslatePts(List<Vector2> vectorsList, Vector2 offset)
    {
        for (int i = 0; i < vectorsList.Count; i++)
        {
            vectorsList[i] += offset;
        }
    }

    public static void InvertYCoordinate(List<Vector2> vectorsList)
    {
        for (int i = 0; i < vectorsList.Count; i++)
        {
            vectorsList[i] = new Vector2(vectorsList[i].x, -vectorsList[i].y);
        }
    }

    public static void SwapAxis(List<Vector2> vectorsList)
    {
        for (int i = 0; i < vectorsList.Count; i++)
        {
            vectorsList[i] = new Vector2(vectorsList[i].y, vectorsList[i].x);
        }
    }

    public List<Vector2> MakeCurve(Directions nasoka, CurveType type)
    {
        int offX = padding;
        int offY = padding;
        int width = tileSize;
        int height = tileSize;

        List<Vector2> curvePts = new List<Vector2>(bezCurve);
        switch (nasoka)
        {
            case Directions.Up:
                switch (type)
                {
                    case CurveType.Positive:
                        TranslatePts(curvePts, new Vector2(offX, offY + height));
                        break;
                    case CurveType.Negative:
                        InvertYCoordinate(curvePts);
                        TranslatePts(curvePts, new Vector2(offX, offY + height));
                        break;
                    default:
                        curvePts.Clear();
                        for (int i = 0; i < 100; i++)
                        {
                            curvePts.Add(new Vector2(i + offX, offY + height));
                        }
                        break;
                }
                break;
            case Directions.Down:
                switch (type)
                {
                    case CurveType.Positive:
                        InvertYCoordinate(curvePts);
                        TranslatePts(curvePts, new Vector2(offX, offY));
                        break;
                    case CurveType.Negative:
                        TranslatePts(curvePts, new Vector2(offX, offY));
                        break;
                    default:
                        curvePts.Clear();
                        for (int i = 0; i < 100; i++)
                        {
                            curvePts.Add(new Vector2(i + offX, offY));
                        }
                        break;
                }
                break;
            case Directions.Left:
                switch (type)
                {
                    case CurveType.Positive:
                        InvertYCoordinate(curvePts);
                        SwapAxis(curvePts);
                        TranslatePts(curvePts, new Vector2(offX, offY));
                        break;
                    case CurveType.Negative:
                        SwapAxis(curvePts);
                        TranslatePts(curvePts, new Vector2(offX, offY));
                        break;
                    default:
                        curvePts.Clear();
                        for (int i = 0; i < 100; i++)
                        {
                            curvePts.Add(new Vector2(offX, offY + i));
                        }
                        break;
                }
                break;
            case Directions.Right:
                switch (type)
                {
                    case CurveType.Positive:
                        SwapAxis(curvePts);
                        TranslatePts(curvePts, new Vector2(offX + width, offY));
                        break;
                    case CurveType.Negative:
                        InvertYCoordinate(curvePts);
                        SwapAxis(curvePts);
                        TranslatePts(curvePts, new Vector2(offX + width, offY));
                        break;
                    default:
                        curvePts.Clear();
                        for (int i = 0; i < 100; i++)
                        {
                            curvePts.Add(new Vector2(offX + width, offY + i));
                        }
                        break;
                }
                break;
        }
        return curvePts;
    }
    public void HideAllCurves()
    {
        foreach (var item in lineRender)
        {
            item.Value.gameObject.SetActive(false);
        }

    }
    public void DestroyAllCurves()
    {
        foreach (var item in lineRender)
        {
            GameObject.Destroy(item.Value.gameObject);
        }

        lineRender.Clear();
    }

    public void Draw(Directions direction, CurveType type, UnityEngine.Color color)
    {
        if (!lineRender.ContainsKey((direction, type)))
        {
            lineRender.Add((direction, type), CreateLR(color));
        }
        LineRenderer lineRenderer = lineRender[(direction, type)];
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        //clarity for unity editor BELOW, DELETE LATER MAYBE???
        lineRenderer.gameObject.name = "Linerender_" + direction.ToString() + "_" + type.ToString();
        List<Vector2> listOfPts = MakeCurve(direction, type);

        lineRenderer.positionCount = listOfPts.Count;
        for (int i = 0; i < listOfPts.Count; i++)
        {
            lineRenderer.SetPosition(i, listOfPts[i]);
        }
    }
}