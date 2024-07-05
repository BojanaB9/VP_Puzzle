using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BoardGen : MonoBehaviour
{
    private string imagename;
    Sprite baseSprite;
    Sprite baseTransparent;

    GameObject goOpaque;
    GameObject goTransparent;

    public float ghostTrans = 0.1f;
    public int numTileX { get; private set; }
    public int numTileY { get; private set; }

    Tile[,] mTiles = null;
    GameObject[,] mTileGameObjects = null;

    public Transform parentForTiles = null;

    public Menu menu = null;
    private List<Rect> tileRegions = new List<Rect>();
    private List<Coroutine> activecoroutines = new List<Coroutine>();
    public List<string> jigsawNames = new List<string>();

    Sprite LoadBase()
    {

        Texture2D texture = SpriteUtil.LoadTexture(imagename);

        Texture2D newtex = new Texture2D(
        texture.width,
        texture.height,
        TextureFormat.ARGB32, false);
        for (int x = 0; x < newtex.width; ++x)
        {
            for (int y = 0; y < newtex.height; ++y)
            {
                newtex.SetPixel(x, y, Color.white);
            }
        }

        for (int x = 0; x < texture.width; ++x)
        {
            for (int y = 0; y < texture.height; ++y)
            {
                Color color = texture.GetPixel(x, y);
                color.a = 1.0f;
                newtex.SetPixel(x, y, color);
            }
        }
        newtex.Apply();
        Sprite sprite = SpriteUtil.CreateSpriteFromTexture2D(
            newtex,
            0,
            0,
            newtex.width, newtex.height);
        return sprite;
    }
    // Start is called before the first frame update
    void Start()
    {
        imagename = GameApp.Instance.getJigsaw();
        baseSprite = LoadBase();
        goOpaque = new GameObject();
        goOpaque.name = imagename + "_opaque";
        goOpaque.AddComponent<SpriteRenderer>().sprite = baseSprite;
        goOpaque.GetComponent<SpriteRenderer>().sortingLayerName = "opaque";

        baseTransparent = createTransView(baseSprite.texture);
        goTransparent = new GameObject();
        goTransparent.name = imagename + "_transparent";
        goTransparent.AddComponent<SpriteRenderer>().sprite = baseTransparent;
        goTransparent.GetComponent<SpriteRenderer>().sortingLayerName = "transparent";

        goOpaque.gameObject.SetActive(false);
        setCameraPos();
        CreateJigsawTiles();
    }

    Sprite createTransView(Texture2D texture)
    {
        Texture2D newtext = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
        for (int i = 0; i < newtext.width; i++)
        {
            for (int j = 0; j < newtext.height; j++)
            {
                Color color = texture.GetPixel(i, j);
                if (i < (newtext.width) && j < (newtext.height))
                {
                    color.a = ghostTrans;
                }
                newtext.SetPixel(i, j, color);
            }
        }
        newtext.Apply();
        Sprite sprite = SpriteUtil.CreateSpriteFromTexture2D(
            newtext, 0, 0, newtext.width, newtext.height);

        return sprite;
    }
    void setCameraPos()
    {
        Camera.main.transform.position = new Vector3(baseSprite.texture.width / 2,
          baseSprite.texture.height / 2, -10.0f);
        Camera.main.orthographicSize = baseSprite.texture.width / 2;
    }
    // Update is called once per frame
    void Update()
    {

    }
    public static GameObject CreateGameObjectFromTile(Tile tile)
    {
        GameObject obj = new GameObject();

        obj.name = "TileGameObe_" + tile.pixelX + "_" + tile.pixelY.ToString();

        obj.transform.position = new Vector3(tile.pixelX * Tile.tileSize, tile.pixelY * Tile.tileSize, 0.0f);

        SpriteRenderer spriteRenderer = obj.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = SpriteUtil.CreateSpriteFromTexture2D(
          tile.finaltexture,
          0,
          0,
          Tile.padding * 2 + Tile.tileSize,
          Tile.padding * 2 + Tile.tileSize);

        BoxCollider2D box = obj.AddComponent<BoxCollider2D>();

        Movement tileMovement = obj.AddComponent<Movement>();
        tileMovement.tile = tile;

        return obj;
    }

    void CreateJigsawTiles()
    {
        Texture2D baseTexture = baseSprite.texture;
        numTileX = baseTexture.width / Tile.tileSize;
        numTileY = baseTexture.height / Tile.tileSize;

        mTiles = new Tile[numTileX, numTileY];
        mTileGameObjects = new GameObject[numTileX, numTileY];

        for (int i = 0; i < numTileX; i++)
        {
            for (int j = 0; j < numTileY; j++)
            {
                mTiles[i, j] = Cut(i, j, baseTexture);
                mTileGameObjects[i, j] = CreateGameObjectFromTile(mTiles[i, j]);
                if (parentForTiles != null)
                {
                    mTileGameObjects[i, j].transform.SetParent(parentForTiles);
                }
            }
        }
        menu.EnableBottomPanel(true);
        menu.btnPlay = ShuffleTiles;
        
    }
    Tile Cut(int i, int j, Texture2D baseTexture)
    {
        Tile tile = new Tile(baseTexture);
        tile.pixelX = i;
        tile.pixelY = j;

        // Left side tiles.
        if (i == 0)
        {
            tile.SetCurveType(Tile.Directions.Left, Tile.CurveType.Edge);
        }
        else
        {
            // We have to create a tile that has LEFT direction opposite curve type.
            Tile leftTile = mTiles[i - 1, j];
            Tile.CurveType rightOp = leftTile.GetCurveType(Tile.Directions.Right);
            tile.SetCurveType(Tile.Directions.Left, rightOp == Tile.CurveType.Negative ?
              Tile.CurveType.Positive : Tile.CurveType.Negative);
        }

        // down
        // side tiles
        if (j == 0)
        {
            tile.SetCurveType(Tile.Directions.Down, Tile.CurveType.Edge);
        }
        else
        {
            Tile downTile = mTiles[i, j - 1];
            Tile.CurveType upOp = downTile.GetCurveType(Tile.Directions.Up);
            tile.SetCurveType(Tile.Directions.Down, upOp == Tile.CurveType.Negative ?
              Tile.CurveType.Positive : Tile.CurveType.Negative);
        }

        // Right side tiles.
        if (i == numTileX - 1)
        {
            tile.SetCurveType(Tile.Directions.Right, Tile.CurveType.Edge);
        }
        else
        {
            float toss = UnityEngine.Random.Range(0f, 1f);
            if (toss < 0.5f)
            {
                tile.SetCurveType(Tile.Directions.Right, Tile.CurveType.Positive);
            }
            else
            {
                tile.SetCurveType(Tile.Directions.Right, Tile.CurveType.Negative);
            }
        }

        // Up side tile.
        if (j == numTileY - 1)
        {
            tile.SetCurveType(Tile.Directions.Up, Tile.CurveType.Edge);
        }
        else
        {
            float toss = UnityEngine.Random.Range(0f, 1f);
            if (toss < 0.5f)
            {
                tile.SetCurveType(Tile.Directions.Up, Tile.CurveType.Positive);
            }
            else
            {
                tile.SetCurveType(Tile.Directions.Up, Tile.CurveType.Negative);
            }
        }

        tile.Apply();
        return tile;
    }

    #region Shuffle codes

    private IEnumerator moveOverSeconds(GameObject movableObject, Vector3 end, float seconds)
    {
        float timepassed = 0.0f;
        Vector3 start = movableObject.transform.position;
        while (timepassed < seconds)
        {
            movableObject.transform.position = Vector3.Lerp(
                start, end, (timepassed/seconds));
            timepassed += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
        movableObject.transform.position = end;
    }

    void Shuffle(GameObject goObj)
    {
        if(tileRegions.Count == 0)
        {
            tileRegions.Add(new Rect(-300.0f, -100.0f, 50.0f, numTileY * Tile.tileSize));
            tileRegions.Add(new Rect(numTileX * Tile.tileSize, -100.0f, 50.0f, numTileY * Tile.tileSize));
        }

        int regionID = Random.Range(0, tileRegions.Count);
        float x = Random.Range(tileRegions[regionID].xMin, tileRegions[regionID].xMax);
        float y = Random.Range(tileRegions[regionID].yMin, tileRegions[regionID].yMax);

        Vector3 position = new Vector3(x, y, 0f);
        Coroutine coroutine = StartCoroutine(moveOverSeconds(goObj, position, 1.0f));
        activecoroutines.Add(coroutine);
    }

    

    public void ShuffleTiles()
    {
        for (int i = 0; i < numTileX; i++)
        {
            for (int j = 0; j < numTileY; j++)
            {
                Shuffle(mTileGameObjects[i, j]);
                //yield return null;
            }
        }

        activecoroutines.Clear();
        menu.EnableBottomPanel(false);
        menu.EnableTopPanel(true);
        GameApp.Instance.MovementEnabled = true;

        StartTimer();
        for (int i = 0; i < numTileX; i++)
        {
            for (int j = 0; j < numTileY; j++)
            {
                Movement m = mTileGameObjects[i, j].GetComponent<Movement>();
                m.OnTilePlace += onTileinPlace;
                SpriteRenderer renderer = m.gameObject.GetComponent<SpriteRenderer>();
                Tile.tilesorting.BringToTop(renderer);
                //yield return null;
            }
        }

        menu.setTotalTiles(numTileX * numTileY);

    }

    void StartTimer()
    {
        StartCoroutine(TimerTick());       
    }

    IEnumerator TimerTick()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            GameApp.Instance.SecondsSinceStart += 1;

            menu.UpdateTime(GameApp.Instance.SecondsSinceStart);
        }
    }

    public void StopTimer()
    {
        StopCoroutine(TimerTick());
    }
    #endregion

    void onTileinPlace(Movement movement)
    {
        GameApp.Instance.TotalCorrectTiles += 1;
        movement.enabled = false;
        Destroy(movement);

        SpriteRenderer sr = movement.gameObject.GetComponent<SpriteRenderer>();
        Tile.tilesorting.Remove(sr);

        sr.sortingLayerName = "RightPlace";

        if(GameApp.Instance.TotalCorrectTiles == mTileGameObjects.Length)
        {
            //Debug.Log("Congrats lol");
            menu.EnableTopPanel(false);
            menu.EnableCompletionpanel(true);

            GameApp.Instance.TotalCorrectTiles = 0;
            GameApp.Instance.SecondsSinceStart = 0;
        }
        menu.setTotalTiles(GameApp.Instance.TotalCorrectTiles);
    }
}