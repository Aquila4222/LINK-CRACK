using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundOutline : MonoBehaviour
{
    public float lineWidth;

    public Color LineColor;
    
    public Color InnerColor;
    
    private SpriteRenderer spriteRenderer;
    
    

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        GameObject obj = new GameObject();
        obj.name = "GroundInner";
        SpriteRenderer inner = obj.AddComponent<SpriteRenderer>();
        inner.sprite = spriteRenderer.sprite;
        inner.color = InnerColor;
   
        inner.transform.localScale = new Vector3(transform.localScale.x - lineWidth,transform.localScale.y - lineWidth, 1);
        inner.sortingOrder = spriteRenderer.sortingOrder+1;
        
        inner.transform.parent = transform;
        inner.transform.localPosition = Vector3.zero;
        
        spriteRenderer.color = LineColor;
        
    }
}
