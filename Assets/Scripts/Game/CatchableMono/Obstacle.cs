using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : CrackableMono,IHurt
{
    private SpriteRenderer sr;
    
    private int blood;

    public Color ObjectColor;

    public SpriteRenderer[] cracks;
    
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;
        blood = 3;
        SetCracksAlpha(0.1f);
    }

    protected override void OnCrash(ContactPoint2D contact)
    {
        base.OnCrash(contact);
        RingVEPool.Instance.Play(contact.point,0.15f,Color.white,0.8f,4f);
        CameraControl.Instance.Shock(contact.point);
        
        
        TakeDamage(contact.normal);

        
    }
    
    
    private void TakeDamage(Vector2 direction)
    {
        SEPool.Instance.PlaySE("Stone",1f);
        StartCoroutine(HurtEffect());
        for (int i = 0; i < 10; i++)
        {
            ParticleVEPool.Instance.Play(transform.position+new Vector3(Random.Range(-transform.localScale.x,transform.localScale.x),Random.Range(-transform.localScale.y,transform.localScale.y),1),0.5f*Random.Range(1,1.5f),0,Vector2.down,new Vector3(0.3f,0.3f,1),new Vector3(0.3f,0.3f,1),ObjectColor,true);
        }
        if (blood > 1)
        {
            blood--;
            switch (blood)
            {
                case 2:
                    SetCracksAlpha(0.5f);
                    break;
                case 1:
                    SetCracksAlpha(1f);
                    break;
                default:
                    break;
            }
        }
        else
        {
            for (int i = 0; i < 20; i++)
            {
                PhysicalVEPool.Instance.Play(transform.position+new Vector3(Random.Range(-transform.localScale.x,transform.localScale.x),Random.Range(-transform.localScale.y,transform.localScale.y),1),Random.Range(1f,3f),(direction.normalized+(Vector2)Random.onUnitSphere.normalized)*Random.Range(20f,30f),Vector3.one*0.4f,ObjectColor);
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

    private void SetCracksAlpha(float alpha)
    {
        foreach (var crack in cracks)
        {
            crack.color = new Color(crack.color.r,crack.color.g,crack.color.b,alpha);
        }
    }


    public void Hurt(Vector2 repulseForce, float damage = 1)
    {
        
        TakeDamage(repulseForce);
    }
}
