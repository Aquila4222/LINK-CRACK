using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BGLight : MonoBehaviour
{
    private Light2D light2D;

    void Awake()
    {
        light2D = GetComponent<Light2D>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="duration"></param>
    /// <param name="velocity"></param>
    /// <param name="color"></param>
    public void Play(Vector3 pos ,float duration ,Vector2 velocity, Color color,Transform parent)
    {
        StartCoroutine(PlayEffect(pos,duration, velocity, color,parent));
    }
    
    IEnumerator PlayEffect(Vector3 pos ,float duration ,Vector2 velocity, Color color,Transform parent)
    {
        if (parent)
        {
            transform.parent =  parent;
        }
        transform.position = pos;
        light2D.enabled = true;
        light2D.color = color;

        float timer = 0;
        while (timer < duration)
        {
            float i = timer/duration;
            float j = i*(2-i);
            
            transform.localPosition += (Vector3)velocity * 0.01f;
            
            float y = 1-Mathf.Cos(i*2*Mathf.PI);
            
            light2D.intensity = y*0.01f;
            
            timer += 0.01f;
            yield return new WaitForSeconds(0.01f);
            
        }

        light2D.enabled = false;
        BGLightsPool.Instance.ReturnObject(gameObject);
    }
}
