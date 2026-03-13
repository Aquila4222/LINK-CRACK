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
    private Vector3 bulletGeneratePosition;
    private Vector3 bulletSpeed;
    
    public void OnEnterState(RemoteBulletState lastState)
    {
        accumulateCurrentTime = Context.accumulateMaxTime;
        currentAfterAttackShakeTime = Context.maxAfterAttackShakeTime;
        attacked = false;
    }

    public void OnState()
    {
        if (accumulateCurrentTime > 0)
        {
            accumulateCurrentTime -= Time.deltaTime;
        }
        else if (!attacked)
        {
            Context.ShootBullet(bulletGeneratePosition, bulletSpeed);
            attacked = true;
        }
        else if (currentAfterAttackShakeTime > 0)
        {
            currentAfterAttackShakeTime -= Time.deltaTime;
        }
        else
        {
            //状态转换
            if (Context.canChase && Context.sightObstructed)
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
        throw new System.NotImplementedException();
    }
}
