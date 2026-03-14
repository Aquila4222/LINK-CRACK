using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;

    private Vector2 repel;

    private float timer;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="velocity"></param>
    public void Shoot(Vector3 pos, Vector2 velocity)
    {
        transform.position = pos;
        repel =  velocity;
        rb.velocity = velocity;
        StartCoroutine(Timer(20f));
    }

    IEnumerator Timer(float time)
    {
        yield return new WaitForSeconds(time);
        BulletPool.Instance.ReturnObject(gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            return;
        }
        IHurt hurt = collision.gameObject.GetComponent<IHurt>();
        if (hurt != null)
        {
            hurt.Hurt(repel);
        }
        StopAllCoroutines();
        RingVEPool.Instance.Play(transform.position,0.2f,Color.red,0.2f,5f);
       
        RingVEPool.Instance.Play(transform.position,0.15f,Color.white,0.8f,4f);
        CameraControl.Instance.Shock(transform.position);
        for (int i = 0; i < 10; i++)
        {
            ParticleVEPool.Instance.Play(transform.position,0.15f*Random.Range(1,1.5f),40*Random.Range(1,3f),Random.onUnitSphere.normalized,new Vector3(0.01f,0.5f,1),new Vector3(0.2f,0.7f,1),Color.white,true);
        }
        BulletPool.Instance.ReturnObject(gameObject);
    }

    private void FixedUpdate()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            timer = 0.04f;
            RingVEPool.Instance.Play(transform.position,0.5f,Color.red,0.4f,0f);

        }
            
    }
}
