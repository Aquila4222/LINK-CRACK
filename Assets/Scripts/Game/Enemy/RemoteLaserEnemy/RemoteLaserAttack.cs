using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteLaserAttack : IState<RemoteLaserState,RemoteLaserEnemy>
{

    public RemoteLaserEnemy Context { get; set; }
    
    //行为参数
    private float accumulateCurrentTime;
    private float currentAfterAttackShakeTime;
    private bool attacked;
    private bool startAccumulate;
    private bool startWarning;
    
    private Vector3 laserLockDirection;
    private float maxWarningTime;
    private float warningTime;
    
    
    public RemoteLaserAttack(RemoteLaserEnemy  context)
    {
        Context = context;
    }
    
    public void OnEnterState(RemoteLaserState lastState)
    {
        accumulateCurrentTime = Context.accumulateMaxTime;
        currentAfterAttackShakeTime = Context.maxAfterAttackShakeTime;
        attacked = false;

        Context.InvokeStartAccumulate();
    }

    public void OnState()
    {
        if (startWarning)
        {
            startWarning = false;
            LaserWarning(Context.bulletGenerateTransform.position, laserLockDirection,warningTime);
        }
        if (warningTime > 0)
        {
            warningTime -= Time.deltaTime;
            
        }else if (startAccumulate)
        {
            startAccumulate = false;
            
        }
        else if (accumulateCurrentTime > 0)
        {
            accumulateCurrentTime -= Time.deltaTime;
            Vector3 direction = Context.targetTransform.position.x > Context.transform.position.x ? Vector3.right : Vector3.left;
            Context.TurnOrientation(direction);
            Context.PointGunAtTarget();
            
            laserLockDirection = (Context.targetTransform.position - Context.bulletGenerateTransform.position).normalized;
        }
        else if (!attacked)
        {
            //TODO:激光攻击调用
            attacked = true;
        }
        else if (currentAfterAttackShakeTime > 0)
        {
            currentAfterAttackShakeTime -= Time.deltaTime;
            Vector3 direction = Context.targetTransform.position.x > Context.transform.position.x ? Vector3.right : Vector3.left;
            Context.TurnOrientation(direction);
        }
        else
        {
            //状态转换
            if(Context.lockTarget && ((!Context.insight && Context.sightObstructed) || !Context.sightObstructed))
            {
                Context.SwitchState(RemoteLaserState.Chasing);
            }
            else if (!Context.lockTarget)
            {
                Context.SwitchState(RemoteLaserState.Idle);
            }
        }
    }

    public void OnExitState()
    {
        
    }

    private void LaserWarning(Vector3 position, Vector3 direction,float time)
    {
        WarningLinePool.Instance.ShowWarningLine(position,direction,time);
    }
}
