using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MeleeAttack : IState<MeleeStateType,MeleeEnemy>
{
    enum MeleeAttackState
    {
        Accumulate = 1,
        Spike = 2,
        Idle = 3
    }

    private MeleeAttackState attackState;
    public MeleeEnemy Context { get; set; }
    
    //行为参数
    private float maxAccumulateTime;
    private float AccumulateTime;
    private float attackMaxTime;
    private float attackStartTime;
    private float attackStartMinTime;
    private float damageMaxStartTime;
    private float damageStartTime;
    private float damageMaxContinueTime;
    private float damageContinueTime;

    private Vector3 spikeOffsetDistance;
    private float spikeMaxTime;
    private float spikeTime;
    private Transform animationTransform;
    private float spikeSpeed;
    private float reboundForce;
    private Vector3 targetLockDirection;
    private bool hitAnObject;
    
    //状态转换条件
    private bool spikeForward;

    public MeleeAttack(MeleeEnemy  context)
    {
        Context = context;
    }
    
    public void OnEnterState(MeleeStateType lastState)
    {
        attackState = MeleeAttackState.Idle;
        maxAccumulateTime = Context.maxAccumulateTime;
        spikeForward = false;
        attackMaxTime = Context.attackMaxTime;
        attackStartTime = attackMaxTime;
        
        spikeOffsetDistance = Context.spikeOffsetDistance;
        spikeMaxTime = Context.spikeMaxTime;
        animationTransform = Context.animationTransform;
        damageMaxStartTime = Context.damageStartTime;
        damageMaxContinueTime = Context.damageContinueTime;
        spikeSpeed = Context.spikeSpeed;
        reboundForce = Context.reboundForce;
    }

    public void OnState()
    {

        Context.attackState = (int)attackState;
        
        Context.onAttack = attackState != MeleeAttackState.Idle ? true : false;
        
        switch (attackState)
        {
            case MeleeAttackState.Idle:
                OnIdle();
                break;
            case MeleeAttackState.Accumulate:
                OnAccumulate();
                break;
            case MeleeAttackState.Spike:
                OnSpike();
                break;
            default:
                break;
        }
    }

    public void OnExitState()
    {
        Context.Move(0,Context.facingDirection);
        animationTransform.position = Context.transform.position;
    }

    //战斗待机态
    public void OnIdle()
    {
        Context.onAttack = false;
        targetLockDirection = (Context.targetTransform.position - Context.transform.position).normalized;
        
        if (attackStartTime > 0)
        {
            attackStartTime -= Time.deltaTime;
        }
        else
        {
            attackStartTime = attackStartMinTime;
            attackState = MeleeAttackState.Accumulate;
        }
    }

    /// <summary>
    /// 蓄力
    /// </summary>
    private void OnAccumulate()
    {
        Context.onAttack = false;
        
        Context.Move(0,Context.facingDirection);
        if (Mathf.Approximately(AccumulateTime, maxAccumulateTime))
        {
            Context.InvokeAccumulate();
        }
        
        if (AccumulateTime > 0)
        {
            AccumulateTime -= Time.deltaTime;
        }
        else
        {
            AccumulateTime = maxAccumulateTime;
            attackState = MeleeAttackState.Spike;
            spikeForward = true;
            
            spikeTime = spikeMaxTime;
            damageStartTime = damageMaxStartTime;
            damageContinueTime = damageMaxContinueTime;
            Context.transform.localScale= new Vector2(Context.targetTransform.position.x > Context.transform.position.x  ? math.abs(Context.targetTransform.localScale.x) * Vector2.right.x : math.abs(Context.targetTransform.localScale.x) * Vector2.left.x,Context.targetTransform.localScale.y);
        }
    }

    /// <summary>
    /// 突刺
    /// </summary>
    private void OnSpike()
    {
        Context.onAttack = true;
        
        if (spikeForward)
        {
            Context.InvokeSpike();
            spikeForward = false;
            Context.onSpike = true;
        }

        if (spikeTime > 0)
        {
            spikeTime -= Time.deltaTime;
            Spike();
        }
        else if(!hitAnObject && Context.DetectOnGround())
        {
            attackState = MeleeAttackState.Idle;
            Context.onSpike = false;
        }
        
    }
    
    private void Spike()
    {
        Collider2D attackCollider = null;
        //突刺
        if (!hitAnObject)
        {
            Context.rigidBody.velocity = targetLockDirection *  spikeSpeed;
            attackCollider = Context.Attack();
        }

        
        //撞到物体回弹并回复状态
         
        if (attackCollider != null && !hitAnObject)
        {
            Context.InvokeSpikeEnd();
            Context.rigidBody.AddForce((Context.transform.position - attackCollider.transform.position).normalized * reboundForce, ForceMode2D.Impulse);
            hitAnObject = true;
        }

        if (hitAnObject && Context.DetectOnGround())
        {
            attackState = MeleeAttackState.Idle;
            hitAnObject = false;
            Context.onSpike =  false;
        }
    }
}
