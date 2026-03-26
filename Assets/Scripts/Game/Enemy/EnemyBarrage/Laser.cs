using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    LayerMask layerMask;

    void Awake()
    {
        layerMask =LayerMask.GetMask("Player") + LayerMask.GetMask("Object")+ LayerMask.GetMask("Enemy")+ LayerMask.GetMask("Ground");
    }
    
    
    public void Shoot(Vector3 pos, Vector2 direction)
    {
        SEPool.Instance.PlaySE("Laser",1);
        
        RaycastHit2D hit =  Physics2D.Raycast(pos, direction, 100f, layerMask);

        
        
        float lenth;
        if (hit)
        {
            lenth = hit.distance;
            IHurt hurt = hit.transform.GetComponent<IHurt>();
            if (hurt != null)
            {
                hurt.Hurt(direction.normalized * 20f);
            }
            RingVEPool.Instance.Play(hit.point,0.15f,Color.white,0.8f,4f);
            CameraControl.Instance.Shock(hit.point);
            for (int i = 0; i < 10; i++)
            {
                Vector3 r = VectorRotator.RotateByAngle(hit.normal,Random.Range(-90f,90f));
                ParticleVEPool.Instance.Play(hit.point,0.15f*Random.Range(1,1.5f),40*Random.Range(1,3f),r,new Vector3(0.01f,0.5f,1),new Vector3(0.2f,0.7f,1),Color.white,true);
            }
          
        }
        else
        {
            lenth = 100f;
        }
        transform.up = direction;
        transform.position = pos;
        StartCoroutine(PlayEffect(lenth));
    }

    IEnumerator PlayEffect(float distance)
    {
        
        float timer = 0;

        while (timer < 0.3f)
        {
            float i = timer/0.3f;
            float j = i*(2-i);
            
            transform.localScale = new Vector3((1-j)*0.5f,distance,1);
            
            timer += 0.01f;
            yield return new WaitForSeconds(0.01f);
            
        }
        
        LaserPool.Instance.ReturnObject(gameObject);
    }
}
