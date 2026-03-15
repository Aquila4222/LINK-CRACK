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
    /*private float maxShakeAfterAttackTime;
    private float currentShakeTime;*/
    
    //状态转换条件
    private bool spikeForward;
    private bool spikeStop;
    private bool startShakeAfterAttack;

    public MeleeAttack(MeleeEnemy  context)
    {
        Context = context;
    }
    
    public void OnEnterState(MeleeStateType lastState)
    {
        attackState = MeleeAttackState.Idle;
        maxAccumulateTime = Context.maxAccumulateTime;
        spikeForward = false;
        attackMaxTime = Context.idleTime;
        attackStartTime = attackMaxTime;
        startShakeAfterAttack = false;
        
        spikeOffsetDistance = Context.spikeOffsetDistance;
        spikeMaxTime = Context.spikeMaxTime;
        animationTransform = Context.animationTransform;
        damageMaxStartTime = Context.damageStartTime;
        damageMaxContinueTime = Context.damageContinueTime;
        spikeSpeed = Context.spikeSpeed;
        reboundForce = Context.reboundForce;
        /*maxShakeAfterAttackTime = Context.maxShakeAfterAttackTime;
        currentShakeTime = maxShakeAfterAttackTime;*/
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
        Context.warningDirection.SetActive(false);
    }

    //战斗待机态
    public void OnIdle()
    {
        Context.onAttack = false;
        Context.warningDirection.SetActive(true);
        targetLockDirection = (Context.targetTransform.position - Context.transform.position).normalized;
        Context.warningDirection.transform.up = targetLockDirection;
        if (attackStartTime > 0)
        {
            attackStartTime -= Time.deltaTime;
        }
        else if(attackStartTime <= 0)
        {
            attackStartTime = attackMaxTime;;
            attackState = MeleeAttackState.Accumulate;
            AccumulateTime = maxAccumulateTime;
        }
    }

    /// <summary>
    /// 蓄力
    /// </summary>
    private void OnAccumulate()
    {
        Context.onAttack = true;
        
        Context.Move(0,Context.facingDirection);
        if (Mathf.Approximately(AccumulateTime, maxAccumulateTime))
        {
            Context.InvokeAccumulate();
            RingVEPool.Instance.Play(Context.transform.position,maxAccumulateTime,Color.red, 1,0,1,Context.transform);
        }
        
        if (AccumulateTime > 0)
        {
            AccumulateTime -= Time.deltaTime;
        }
        else
        {
            Context.warningDirection.SetActive(false);
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
        else if (spikeTime <= 0 && !spikeStop)
        {
            spikeStop = true;
            Context.InvokeSpikeEnd();
            Context.rigidBody.velocity /= 10;
        }
        else if(!hitAnObject && Context.DetectOnGround())
        {
            attackState = MeleeAttackState.Idle;
            Context.onSpike = false;
            spikeForward = false;
            spikeStop =  false;
        }
        /*else if (startShakeAfterAttack && currentShakeTime > 0)
        {
            currentShakeTime -= Time.deltaTime;
        }
        else if (currentShakeTime <= 0)
        {
            attackState = MeleeAttackState.Idle;
            Context.onSpike = false;
            currentShakeTime = maxShakeAfterAttackTime;
            spikeForward = false;
            spikeStop =  false;
        }*/
        
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

        if (hitAnObject)
        {
            attackState = MeleeAttackState.Idle;
            hitAnObject = false;
            Context.onSpike =  false;
            attackStartTime = attackMaxTime;
        }
        /*else if (startShakeAfterAttack  && currentShakeTime > 0)
        {
            currentShakeTime -= Time.deltaTime;
        }
        else if (currentShakeTime <= 0)
        {
            attackState = MeleeAttackState.Idle;
            hitAnObject = false;
            Context.onSpike =  false;
            attackStartTime = attackMaxTime;
            currentShakeTime = maxShakeAfterAttackTime;
        }*/
        
        
    }

    
}
