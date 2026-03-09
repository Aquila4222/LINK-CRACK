using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchableMono : MonoBehaviour
{
    private const float CrashSpeed = 10f;
    
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
            if (rb.velocity.magnitude > CrashSpeed && isChained)
            {
                OnCrash();
            }
        }
    }

    protected virtual void OnCrash()
    {
        
    }
}
