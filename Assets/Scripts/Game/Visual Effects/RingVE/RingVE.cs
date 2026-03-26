using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingVE : MonoBehaviour
{
    private SpriteRenderer sr;
    
    
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void PlayEffect(Vector3 pos, float duration, Color color , float scale , float endScale,float startAlpha,Transform parent = null)
    {
        StartCoroutine(Play(pos, duration, color, scale, endScale,startAlpha,parent));
    }

    IEnumerator Play(Vector3 pos, float duration, Color color , float scale , float endScale,float startAlpha,Transform parent = null)
    {
        sr.color = new Color(color.r, color.g, color.b, startAlpha);
        transform.position = pos;
        transform.localScale = new Vector3(scale, scale, scale);

        float timer = 0;

        while (timer < duration)
        {
            if (parent)
            {
                transform.position = parent.position;
            }
            
            float i = timer/duration;
            float j = i*(2-i);
            
            float s = (endScale-1)*j*scale + scale;
            transform.localScale =  new Vector3(s, s, s);
            sr.color = new Color(color.r, color.g, color.b, startAlpha*(1-j));
            
            yield return new WaitForSeconds(0.01f);
            timer += 0.01f;
        }
        transform.localScale =  new Vector3(endScale, endScale, endScale);
        sr.color = new Color(color.r, color.g, color.b, 0);
        
        yield return new WaitForSeconds(0.01f);
        
        RingVEPool.Instance.ReturnObject(gameObject);
    }
    
}
