using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesGen : MonoBehaviour
{
    public string imageFilename;
    private Texture2D mTextureOriginal;

    private Tile tiles = null;
    private Sprite sprite = null;


    void Start()
    {
        CreateBaseTexture();

    }

    void CreateBaseTexture()
    {
        mTextureOriginal = SpriteUtil.LoadTexture(imageFilename);
        Debug.Log(imageFilename);
        if (!mTextureOriginal.isReadable)
        {
            Debug.Log("Texture is nor readable");
            return;
        }

        SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        sprite = SpriteUtil.CreateSpriteFromTexture2D(
          mTextureOriginal,
          0,
          0,
          mTextureOriginal.width,
          mTextureOriginal.height);
        spriteRenderer.sprite = sprite;
    }

    private (Tile.CurveType, UnityEngine.Color) GetRendomType()
    {
        Tile.CurveType type;
        UnityEngine.Color color;
        float rand = UnityEngine.Random.Range(0f, 1f);

        if (rand < 0.5f)
        {
            type = Tile.CurveType.Positive;
            color = UnityEngine.Color.blue;
        }
        else
        {
            type = Tile.CurveType.Negative;
            color = UnityEngine.Color.red;
        }
        return (type, color);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TestRandomCurves();
            tiles.HideAllCurves();


        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            TestTileFloodFill();
        }
    }

    void TestRandomCurves()
    {
        if (tiles != null)
        {
            tiles.DestroyAllCurves();
            tiles = null;
        }

        Tile tile = new Tile(mTextureOriginal);
        tiles = tile;

        var tmp = GetRendomType();
        this.tiles.Draw(Tile.Directions.Up, tmp.Item1, tmp.Item2);
        tmp = GetRendomType();
        this.tiles.Draw(Tile.Directions.Down, tmp.Item1, tmp.Item2);
        tmp = GetRendomType();
        this.tiles.Draw(Tile.Directions.Left, tmp.Item1, tmp.Item2);
        tmp = GetRendomType();
        this.tiles.Draw(Tile.Directions.Right, tmp.Item1, tmp.Item2);

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;

    }

    void TestTileFloodFill()
    {
        if (tiles != null)
        {
            tiles.DestroyAllCurves();
            tiles = null;
        }

        tiles = new Tile(mTextureOriginal);


        var type_color = GetRendomType();
        tiles.Draw(Tile.Directions.Up, type_color.Item1, type_color.Item2);
        tiles.SetCurveType(Tile.Directions.Up, type_color.Item1);

        type_color = GetRendomType();
        tiles.Draw(Tile.Directions.Right, type_color.Item1, type_color.Item2);
        tiles.SetCurveType(Tile.Directions.Right, type_color.Item1);

        type_color = GetRendomType();
        tiles.Draw(Tile.Directions.Down, type_color.Item1, type_color.Item2);
        tiles.SetCurveType(Tile.Directions.Down, type_color.Item1);

        type_color = GetRendomType();
        tiles.Draw(Tile.Directions.Left, type_color.Item1, type_color.Item2);
        tiles.SetCurveType(Tile.Directions.Left, type_color.Item1);

        tiles.Apply();


        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite sprite = SpriteUtil.CreateSpriteFromTexture2D(
          tiles.finaltexture,
          0,
          0,
          tiles.finaltexture.width,
          tiles.finaltexture.height);

        spriteRenderer.sprite = sprite;
    }
}
