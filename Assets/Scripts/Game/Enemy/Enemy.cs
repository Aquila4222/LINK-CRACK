using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

enum EnemyStateType
{
    UnGenerated,
    WarmUp,
    Alive,
    OnFroze,
    Dead,
}

public class Enemy : CatchableMono,IHurt
{
    protected float Health { get; set; }

     private EnemyStateType currentState;
     private Vector3 detectStartOffsetDistance;
     private Vector3  detectEndOffsetDistance;
     protected LayerMask whatIsGround;

     private void Initialized()
     {
         currentState = EnemyStateType.Alive;
         detectEndOffsetDistance = new Vector3(-0.5f, -0.53f, 0);
         detectStartOffsetDistance = new Vector3(0.5f, -0.53f, 0);
         whatIsGround = LayerMask.GetMask("Ground");
         Health = 100;
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
        Alive();
        
        //状态转换
        if (!DetectOnGround())
        {
            currentState = EnemyStateType.OnFroze;
            rb.freezeRotation = true;
        }
        else if (canHurtOther && Health > 0)
        {
            currentState = EnemyStateType.OnFroze;
            rb.velocity = new Vector2(0,rb.velocity.y);
            rb.freezeRotation = true;
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
            rb.freezeRotation = false;
        }
        else if (Health <= 0)
        {
            currentState = EnemyStateType.Dead;
        }
    }

    private void OnDead()
    {
        
    }

    /// <summary>
    /// 地面检测
    /// </summary>
    /// <returns></returns>
    private bool DetectOnGround()
    {
        bool onGround = Physics2D.Linecast(transform.position +  detectStartOffsetDistance, transform.position + detectEndOffsetDistance,whatIsGround);
        return onGround;
    }

    public void Hurt(Vector2 repulseForce, float damage = 1)
    {
        Health -= damage;
        rb.AddForce(repulseForce, ForceMode2D.Impulse);
    }
}


