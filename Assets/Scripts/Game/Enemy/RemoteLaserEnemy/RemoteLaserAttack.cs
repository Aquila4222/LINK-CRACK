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
    private LayerMask layerMask;
    
    private Vector3 laserLockDirection;
    private float maxWarningTime;
    private float warningTime;
    
    
    public RemoteLaserAttack(RemoteLaserEnemy  context)
    {
        Context = context;
        layerMask =LayerMask.GetMask("Player") + LayerMask.GetMask("Object")+ LayerMask.GetMask("Enemy")+ LayerMask.GetMask("Ground");
    }
    
    public void OnEnterState(RemoteLaserState lastState)
    {
        accumulateCurrentTime = Context.accumulateMaxTime;
        currentAfterAttackShakeTime = Context.maxAfterAttackShakeTime;
        warningTime = Context.warningTime;
        attacked = false;

        Context.InvokeStartAccumulate();
    }

    public void OnState()
    {
        if (Mathf.Approximately(accumulateCurrentTime, Context.accumulateMaxTime) && warningTime <= 0)
        {
            RingVEPool.Instance.Play(Context.accumulateRing.position,Context.accumulateMaxTime,Color.red, 1,0,1,Context.accumulateRing);
        }
        
        if (warningTime > 0)
        {
            warningTime -= Time.deltaTime;
            Vector3 direction = Context.targetTransform.position.x > Context.transform.position.x ? Vector3.right : Vector3.left;
            Context.TurnOrientation(direction);
            Context.PointGunAtTarget();
            laserLockDirection =(Context.targetTransform.position - Context.bulletGenerateTransform.position).normalized;
            LaserWarning(Context.bulletGenerateTransform.position, laserLockDirection);
            
        }
        else if (accumulateCurrentTime > 0)
        {
            accumulateCurrentTime -= Time.deltaTime;
            LaserWarning(Context.bulletGenerateTransform.position, laserLockDirection);
        }
        else if (!attacked)
        {
            Context.ShootLaser(Context.bulletGenerateTransform.position, laserLockDirection);
            attacked = true;
            EndLaserWarning();
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
        Context.warningLine.SetActive(true);
        
        RaycastHit2D hit = Physics2D.Raycast(position, direction, 100f, layerMask);
        
        float length = 0;
        length = hit ? hit.distance : 100f;
        
        Context.warningLine.transform.localScale = new Vector3(0.1f, length, 1f);
        
        Context.warningLine.transform.position = position;
        Context.warningLine.transform.up = direction;
    }
    
    private void EndLaserWarning()
    {
        Context.warningLine.SetActive(false);
    }
}
