using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleePatrol : IState<MeleeStateType,MeleeEnemy>
{
    public MeleeEnemy Context { get; set; }

    private float patrolSpeed;
    private float maxTurnAroundTime;
    private float turnAroundTime;
    
    public MeleePatrol(MeleeEnemy context)
    {
        Context = context;
    }
    

    public void OnEnterState(MeleeStateType lastState)
    {
        maxTurnAroundTime = Context.maxTurnAroundTime;
        turnAroundTime = maxTurnAroundTime;
        patrolSpeed = Context.patrolSpeed;
    }

    public void OnState()
    {
        //临时调试变量同步区
        patrolSpeed = Context.patrolSpeed;
        maxTurnAroundTime  = Context.maxTurnAroundTime;
        
        //移动
        Context.Move(patrolSpeed,Context.facingDirection);
        
        //转身计时
        turnAroundTime -= Time.deltaTime;
        if (turnAroundTime <= 0)
        {
            Context.TurnAround();
            turnAroundTime = maxTurnAroundTime;
        }
            
    }

    public void OnExitState()
    {
        Context.Move(0,Context.facingDirection);
    }
    
}
