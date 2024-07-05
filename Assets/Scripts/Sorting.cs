using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Sorting 
{

    private List<SpriteRenderer> renderers = new List<SpriteRenderer>();

    public Sorting() { }

    public void Clear()
    {
        renderers.Clear();
    }

    public void Add(SpriteRenderer sprite)
    {
        renderers.Add(sprite);
        SetRenderOrder(sprite, renderers.Count);
    }

    public void Remove(SpriteRenderer sprite)
    {
        renderers.Remove(sprite);
        for (int i = 0; i < renderers.Count; i++)
        {
            SetRenderOrder(renderers[i], i+1);
        }
    }

    public void BringToTop(SpriteRenderer sprite)
    {
        Remove(sprite);
        Add(sprite);
    }
    private void SetRenderOrder(SpriteRenderer sprite, int n)
    {
        sprite.sortingOrder = n;
        Vector3 vector = sprite.transform.position;
        vector.z = -n / 10.0f;
        sprite.transform.position = vector;
    }
}
