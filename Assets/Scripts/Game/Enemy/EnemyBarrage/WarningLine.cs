using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WarningLine : MonoBehaviour
{
    LayerMask layerMask;
    SpriteRenderer lineSprite;
    float flashingTime;
    float flashingTotalTime;

    void Awake()
    {
        lineSprite = GetComponent<SpriteRenderer>();
        layerMask =LayerMask.GetMask("Player") + LayerMask.GetMask("Object")+ LayerMask.GetMask("Enemy")+ LayerMask.GetMask("Ground");
        flashingTime = 0.2f;
    }

    public void ShowWarningLine(Vector3 position, Vector3 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, direction, 100f, layerMask);
        
        float length = 0;
        if (hit)
        {
            length = hit.distance;
            
        }

        
    }

    


    
}
