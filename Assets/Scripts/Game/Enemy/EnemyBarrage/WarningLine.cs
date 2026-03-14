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
        
        float length;
        if (hit)
        {
            length = hit.distance;
            
        }

        
    }

    


    IEnumerable PlayEffect()
    {
        float totalTime = 0;

        while (totalTime < flashingTotalTime)
        {
            float time = 0;
            while(time < flashingTime / 2 && totalTime < flashingTotalTime)
            {
                lineSprite.color = new Color(1f, 0f, 0f, time / flashingTime / 2);
                
                totalTime += 0.01f;
                yield return new WaitForSeconds(0.01f);
            }
            while (time < flashingTime && totalTime < flashingTotalTime)
            {
                lineSprite.color = new Color(1f, 0f, 0f, (flashingTime -  time) / flashingTime / 2);
                
                totalTime += 0.01f;
                yield return new WaitForSeconds(0.01f);
            }


            totalTime += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        
        WarningLinePool.Instance.ReturnObject(gameObject);
    }
}
