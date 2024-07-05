using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Movement : MonoBehaviour
{
    public Tile tile { get; set; }
    private Vector3 MouseOffset = new Vector3(0.0f, 0.0f, 0.0f);

    public delegate void DelegateOnTilePlace(Movement movement);
    public DelegateOnTilePlace OnTilePlace;

    private Vector3 CorrectPos()
    {
        return new Vector3(tile.pixelX * 100f, tile.pixelY * 100, 0f);
    }
    private SpriteRenderer spriteRenderer;


    private void OnMouseDown()
    {
        if (!GameApp.Instance.MovementEnabled) { return; }
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        MouseOffset = transform.position - Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
        Tile.tilesorting.BringToTop(spriteRenderer);
    }

    private void OnMouseDrag()
    {
        if(!GameApp.Instance.MovementEnabled) { return; }
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        Vector3 currScreen = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
        Vector3 currpos = Camera.main.ScreenToWorldPoint(currScreen) + MouseOffset;
        transform.position = currpos;
    }

    private void OnMouseUp()
    {
        if (!GameApp.Instance.MovementEnabled) { return; }
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        float distance = (transform.position - CorrectPos()).magnitude;
        if (distance < 20.0f)
        {
            transform.position = CorrectPos();
            OnTilePlace?.Invoke(this);
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
