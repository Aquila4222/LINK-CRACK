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
    
    private Vector3 laserLockDirection;
    
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
        if (accumulateCurrentTime > 0)
        {
            accumulateCurrentTime -= Time.deltaTime;
            Vector3 direction = Context.targetTransform.position.x > Context.transform.position.x ? Vector3.right : Vector3.left;
            Context.TurnOrientation(direction);
            Context.PointGunAtTarget();
            
            laserLockDirection = (Context.targetTransform.position - Context.bulletGenerateTransform.position).normalized;
            LaserWarning(Context.bulletGenerateTransform.position, laserLockDirection);
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

    private void LaserWarning(Vector3 position, Vector3 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, direction,100f,Context.whatCanBlockLaser);

        float length;
        if (hit)
        {
            length = hit.distance;
            if (hit.collider != null)
            {
                
            }
        }
    }
}
