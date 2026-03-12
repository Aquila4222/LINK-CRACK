using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleVE : MonoBehaviour
{
    private SpriteRenderer sr;
    
  

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Play(Vector3 pos,float duration, float speed,Vector2 direction,Vector3 startScale,Vector3 endScale,Color color)
    {
        StartCoroutine(PlayEffect(pos, duration, speed, direction, startScale, endScale, color));
    }
    
    IEnumerator PlayEffect(Vector3 pos,float duration, float speed,Vector2 direction,Vector3 startScale,Vector3 endScale,Color color)
    {
        transform.position = pos;
        transform.localScale = startScale;
        transform.up = direction;
        
        float timer = 0;

        while (timer < duration)
        {
            float i = timer/duration;
            float j = i*(2-i);
            
            
            transform.localScale =  (endScale - startScale)*j+startScale;
            sr.color = new Color(color.r, color.g, color.b, (1-j));
            
            transform.position += (Vector3)direction.normalized*(speed*0.01f*(1-i));
            
            timer += 0.01f;
            yield return new WaitForSeconds(0.01f);
            
        }
        
        ParticleVEPool.Instance.ReturnObject(gameObject);
    }
}
