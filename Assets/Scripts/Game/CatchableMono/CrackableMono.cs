using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackableMono : MonoBehaviour
{
    private const float CrashSpeed = 20f;
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        CatchableMono c = collision.gameObject.GetComponent<CatchableMono>();
        if (c)
        {
            Vector2 normal = collision.contacts[0].normal;
            // if (Vector2.Dot(c.maxSpeed,normal) > CrashSpeed && c.canHurtOther)
            {
                OnCrash(collision.contacts[0]);
            }
        }
    }

    protected virtual void OnCrash(ContactPoint2D contact)
    {
        
    }
}
