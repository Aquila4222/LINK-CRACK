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
        }
    }

    /// <summary>
    /// 突刺
    /// </summary>
    private void OnSpike()
    {
        Context.onAttack = true;
        
        //TODO:冲刺逻辑待优化
        /*if (spikeForward)
        {
            Context.transform.localScale= new Vector2(Context.targetTransform.position.x > Context.transform.position.x  ? math.abs(Context.targetTransform.localScale.x) * Vector2.right.x : math.abs(Context.targetTransform.localScale.x) * Vector2.left.x,Context.targetTransform.localScale.y);
            Context.rigidBody.AddForce(spikeForce * Context.facingDirection, ForceMode2D.Impulse);
            spikeForward = false;
        }
        else if(Context.onGroundForJump)
        {
            Context.Move(0,Context.facingDirection);
            attackState = MeleeAttackState.Idle;
            
        }*/

        if (spikeTime > spikeMaxTime / 2)
        {
            spikeTime -= Time.deltaTime;
            animationTransform.position = Context.transform.position + spikeOffsetDistance * (spikeMaxTime / 2 - math.abs(spikeTime - spikeMaxTime / 2)) / spikeMaxTime * 2 * Context.facingDirection.x;
            animationTransform.localScale = new Vector3(Context.transform.localScale.x * -1, Context.transform.localScale.y, Context.transform.localScale.z);
        }
        else if (spikeTime <= spikeMaxTime && spikeTime > 0)
        {
            spikeTime -= Time.deltaTime;
            animationTransform.position = Context.transform.position + spikeOffsetDistance * (spikeMaxTime / 2 - math.abs(spikeTime - spikeMaxTime / 2)) / spikeMaxTime * 2 * Context.facingDirection.x;
            animationTransform.localScale = Context.transform.localScale;
        }
        else
        {
            animationTransform.position = Context.transform.position;
            attackState = MeleeAttackState.Idle;
        }

        if (damageStartTime > 0)
        {
            damageStartTime -= Time.deltaTime;
        }
        else if(damageContinueTime > 0)
        {
            damageContinueTime -= Time.deltaTime;
            Context.Attack();
        }
        
    }
}
