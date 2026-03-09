using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchableMono : MonoBehaviour
{
    private const float CrashSpeed = 30f;
    
    public Rigidbody2D rb
    {
        get
        {
            if (!_rb) 
                _rb = GetComponent<Rigidbody2D>();
            return _rb;
        }
        set
        {
            if (!_rb) 
                _rb = GetComponent<Rigidbody2D>();
            if (value != null) 
                _rb = value;
        }
    }

    private Rigidbody2D _rb;

    protected bool isChained;

    private Vector2 maxSpeed;
    private float timer;
    private bool isRising;


    void FixedUpdate()
    {
        if (rb.velocity.magnitude > maxSpeed.magnitude)
        {
            maxSpeed = rb.velocity;
            timer = 0.05f;
            isRising =  true;
        }
        else
        {
            if (isRising)
            {
                if (timer >= 0)
                {
                    timer -= Time.deltaTime;
                }
                else
                {
                    maxSpeed = rb.velocity;
                    isRising = false;
                }
            }
            else
            {
                maxSpeed = rb.velocity;
            }
        }
    }
    
    
    public void Chain()
    {
        isChained = true;
    }

    public void UnChain()
    {
        isChained = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Vector2 normal = collision.contacts[0].normal;
            if (Vector2.Dot(maxSpeed,-normal) > CrashSpeed && isChained)
            {
                OnCrash();
            }
        }
    }

    protected virtual void OnCrash()
    {
        
    }
}
