using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum EnemyStateType
{
    UnGenerated,
    WarmUp,
    Alive,
    OnCaught,
    Dead,
}

public class Enemy : CatchableMono
{
    protected float Health { get; set; }

    [SerializeField] private EnemyStateType currentState;
    [SerializeField] private Vector3 detectStartOffsetDistance;
    [SerializeField] private Vector3  detectEndOffsetDistance;

    protected void Update()
    {
        base.Update();
        switch (currentState)
        {
            case EnemyStateType.Alive:
                break;
            case EnemyStateType.OnCaught:
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
        rb.freezeRotation = false;
        
        
        //状态转换
        if (canHurtOther && Health > 0)
        {
            currentState = EnemyStateType.OnCaught;
        }
        else if (Health <= 0)
        {
            currentState = EnemyStateType.Dead;
        }
        
    }
    
    /// <summary>
    ///被抓住
    /// </summary>
    private void OnCaught()
    {
        
        //状态转换
        if (!canHurtOther && Health > 0)
        {
            currentState = EnemyStateType.Alive;
        }
        else if (Health <= 0)
        {
            currentState = EnemyStateType.Dead;
        }
    }


}
