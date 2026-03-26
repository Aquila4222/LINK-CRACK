using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteBulletIdle : IState<RemoteBulletState,RemoteBulletEnemy>
{
   

    public RemoteBulletEnemy Context { get; set; }

    private float turnAroundTime;
    
    public RemoteBulletIdle(RemoteBulletEnemy context)
    {
        Context = context;
    }
    
    public void OnEnterState(RemoteBulletState lastState)
    {
        turnAroundTime = Random.Range(Context.minTurnAroundTime, Context.maxTurnAroundTime);
        Context.InvokeStartIdle();
    }

    public void OnState()
    {
        if (turnAroundTime >= 0)
        {
            turnAroundTime -= Time.deltaTime;
        }
        else
        {
            turnAroundTime = Random.Range(Context.minTurnAroundTime, Context.maxTurnAroundTime);
            Context.TurnOrientation(new Vector3(Context.facingDirection.x * -1,0,0));
        }
        
        //状态转换

        if(Context.lockTarget && !Context.sightObstructed && Context.insight)
        {
            Context.SwitchState(RemoteBulletState.Attacking);
        }
    }

    public void OnExitState()
    {
        Context.InvokeStopIdle();
    }
}
