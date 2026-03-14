using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteBulletAttack : IState<RemoteBulletState,RemoteBulletEnemy>
{
    public RemoteBulletEnemy Context { get; set; }
    
    //行为参数
    private float accumulateCurrentTime;
    private float currentAfterAttackShakeTime;
    private bool attacked;
    
    //弹幕参数
    private Vector3 bulletDirectionSpeed;
    private float bulletSpeed;

    public RemoteBulletAttack(RemoteBulletEnemy  context)
    {
        Context = context;
    }
    
    public void OnEnterState(RemoteBulletState lastState)
    {
        accumulateCurrentTime = Context.accumulateMaxTime;
        currentAfterAttackShakeTime = Context.maxAfterAttackShakeTime;
        attacked = false;
        
        bulletSpeed = Context.bulletSpeed;
        
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
        }
        else if (!attacked)
        {
            bulletDirectionSpeed = (Context.targetTransform.position  - Context.gunTransform.position).normalized * bulletSpeed;
            
            Context.ShootBullet(Context.gunTransform.position, bulletDirectionSpeed);
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
                Context.SwitchState(RemoteBulletState.Chase);
            }
            else if (!Context.lockTarget)
            {
                Context.SwitchState(RemoteBulletState.Idle);
            }
        }
        
    }

    public void OnExitState()
    {
        
    }
}
