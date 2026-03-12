using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalVE : MonoBehaviour
{
    private SpriteRenderer sr;
    private Rigidbody2D rb;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }
    
    
    public void Play(Vector3 pos, float time, Vector2 vel, Vector3 scale, Color color)
    {
        StartCoroutine(PlayEffect(pos, time, vel, scale, color));
    }
    
    IEnumerator PlayEffect(Vector3 pos,float time , Vector2 vel,Vector3 scale,Color color)
    {
        transform.position = pos;
        rb.velocity = vel;
        transform.localScale = scale;
        sr.color = color;
        yield return new WaitForSeconds(time);
        
        float timer = 0;

        while (timer < 0.5f)
        {
            float i = timer/0.5f;
            sr.color =  new Color(color.r,color.g,color.b,color.a*(1-i));
            
            timer += 0.01f;
            yield return new WaitForSeconds(0.01f);
            
        }
        PhysicalVEPool.Instance.ReturnObject(gameObject);
    }
}
