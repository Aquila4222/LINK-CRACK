using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WarningLine : MonoBehaviour
{
    LayerMask layerMask;
    SpriteRenderer lineSprite;
    private float continueTime;

    void Awake()
    {
        lineSprite = GetComponentInChildren<SpriteRenderer>();
        layerMask =LayerMask.GetMask("Player") + LayerMask.GetMask("Object")+ LayerMask.GetMask("Enemy")+ LayerMask.GetMask("Ground");
    }

    public void ShowWarningLine(Vector3 position, Vector3 direction,float time)
    {
        continueTime = time;
        RaycastHit2D hit = Physics2D.Raycast(position, direction, 100f, layerMask);
        
        float length = 0;
        if (hit)
        {
            length = hit.distance;
        }
        else
        {
            length = 100f;
        }
        
        lineSprite.color = new Color(lineSprite.color.r, lineSprite.color.g, lineSprite.color.b, 0.25f);
        transform.localScale = new Vector3(0.1f, length, 1);
        
        transform.up = direction;
        transform.position = position;
    }

    private void Update()
    {
        if (continueTime > 0)
        {
            continueTime -= Time.deltaTime;
        }
        else
        {
            WarningLinePool.Instance.ReturnObject(gameObject);
        }
    }
}
