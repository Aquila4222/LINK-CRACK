using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public enum EnemyStateType
{
    WarmUp,
    Alive,
    OnFroze,
    Dead,
}

public class Enemy : CatchableMono,IHurt
{
     protected float Health { get; set; }

     public EnemyStateType currentState;
     private Vector3 detectStartOffsetDistance;
     private Vector3  detectEndOffsetDistance;
     protected LayerMask whatIsGround;
     [SerializeField]private string SEName;
     [SerializeField]private Color ObjectColor;
     public bool onSpike;

     private void Initialized()
     {
         currentState = EnemyStateType.Alive;
         detectEndOffsetDistance = new Vector3(-0.5f, -0.53f, 0);
         detectStartOffsetDistance = new Vector3(0.5f, -0.53f, 0);
         whatIsGround = LayerMask.GetMask("Ground") + LayerMask.GetMask("Object") + LayerMask.GetMask("Player") +
                        LayerMask.GetMask("Platform");
         Health = 3;
     }
     
     protected void Awake()
     {
         Initialized();
     }

     protected void Update()
    {
        base.Update();
        switch (currentState)
        {
            case EnemyStateType.Alive:
                OnAlive();
                break;
            case EnemyStateType.OnFroze:
                OnFroze();
                break;
            case EnemyStateType.Dead:
                OnDead();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 活着，不被抓取
    /// </summary>
    private void OnAlive()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        
        Alive();
        
        //状态转换
        if (!DetectOnGround() && !onSpike)
        {
            currentState = EnemyStateType.OnFroze;
            rb.freezeRotation = false;
        }
        else if (canHurtOther && Health > 0 && !onSpike)
        {
            currentState = EnemyStateType.OnFroze;
            rb.velocity = new Vector2(0,rb.velocity.y);
            rb.freezeRotation = false;
        }
        else if (Health <= 0)
        {
            currentState = EnemyStateType.Dead;
        }
        
    }

    protected virtual void Alive()
    {
        
    }
    
    /// <summary>
    ///被抓住
    /// </summary>
    private void OnFroze()
    {
        
        
        //状态转换
        if (!canHurtOther && Health > 0 && DetectOnGround())
        {
            currentState = EnemyStateType.Alive;
            rb.freezeRotation = true;
        }
        else if (Health <= 0)
        {
            currentState = EnemyStateType.Dead;
        }
    }

    private void OnDead()
    {
        for (int i = 0; i < 20; i++)
        {
            PhysicalVEPool.Instance.Play(transform.position + new Vector3(Random.Range(-transform.localScale.x,transform.localScale.x),Random.Range(-transform.localScale.y,transform.localScale.y),1),Random.Range(1f,3f),Random.onUnitSphere.normalized*Random.Range(20f,30f),Vector3.one*0.4f,ObjectColor);
        }
        
        Destroy(this.gameObject);
    }

    /// <summary>
    /// 地面检测
    /// </summary>
    /// <returns></returns>
    public bool DetectOnGround()
    {
        bool onGround = Physics2D.Linecast(transform.position +  detectStartOffsetDistance, transform.position + detectEndOffsetDistance,whatIsGround);
        return onGround;
    }

    //受伤接口
    public virtual void Hurt(Vector2 repulseForce, float damage = 1)
    {
        Health -= damage;
        rb.AddForce(repulseForce, ForceMode2D.Impulse);
    }

    
    
    /// <summary>
    /// 碰撞效果
    /// </summary>
    /// <param name="contact"></param>
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
        Hurt(new Vector2(0,0),1);
    }
    
    private void TakeDamage()
    {
        SEPool.Instance.PlaySE(SEName,1f);
        for (int i = 0; i < 10; i++)
        {
            ParticleVEPool.Instance.Play(transform.position,0.2f*Random.Range(1,1.5f),10*Random.Range(1,1.5f),Random.onUnitSphere.normalized,new Vector3(0.01f,0.1f,0.1f),new Vector3(0.5f,0.5f,0.5f),Color.red,true);
        }
    }
}


