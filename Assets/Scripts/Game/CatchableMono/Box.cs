using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : CatchableMono,IHurt
{
    public int MaxBlood;
    
    private SpriteRenderer sr;

    private int blood;

    public Color ObjectColor;
    
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;
        blood = MaxBlood;
    }


    private void TakeDamage()
    {
        for (int i = 0; i < 10; i++)
        {
            ParticleVEPool.Instance.Play(transform.position,0.2f*Random.Range(1,1.5f),10*Random.Range(1,1.5f),Random.onUnitSphere.normalized,new Vector3(0.01f,0.1f,0.1f),new Vector3(0.5f,0.5f,0.5f),Color.white,true);
        }
        StartCoroutine(HurtEffect());
        if (blood > 1)
        {
            blood--;
        }
        else
        {
            for (int i = 0; i < 20; i++)
            {
                PhysicalVEPool.Instance.Play(transform.position + new Vector3(Random.Range(-transform.localScale.x,transform.localScale.x),Random.Range(-transform.localScale.y,transform.localScale.y),1),Random.Range(1f,3f),Random.onUnitSphere.normalized*Random.Range(20f,30f),Vector3.one*0.4f,ObjectColor);
            }
            GameObject.Destroy(gameObject);
        }
    }
    
    IEnumerator HurtEffect()
    {
        sr.enabled = true;
        yield return new WaitForSeconds(0.1f);
        sr.enabled = false;
    }
    
    protected override void OnCrash(ContactPoint2D contact)
    {
        base.OnCrash(contact);
        RingVEPool.Instance.Play(contact.point,0.15f,Color.white,0.8f,4f);
        CameraControl.Instance.Shock(contact.point);
       
        for (int i = 0; i < 10; i++)
        {
            Vector3 r = VectorRotator.RotateByAngle(contact.normal,Random.Range(-90f,90f));
            ParticleVEPool.Instance.Play(contact.point,0.15f*Random.Range(1,1.5f),40*Random.Range(1,3f),r,new Vector3(0.01f,0.5f,1),new Vector3(0.2f,0.7f,1),Color.white,true);
        }
        
        
        TakeDamage();
        
    }

    public void Hurt(Vector2 repulseForce, float damage = 1)
    {
        rb.AddForce(repulseForce, ForceMode2D.Impulse);
        TakeDamage();
    }
}
