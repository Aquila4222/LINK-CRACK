using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteBulletIdle : IState<RemoteBulletState,RemoteBulletEnemy>
{
   

    public RemoteBulletEnemy Context { get; set; }

    public RemoteBulletIdle(RemoteBulletEnemy context)
    {
        Context = context;
    }
    
    public void OnEnterState(RemoteBulletState lastState)
    {
        Context.InvokeStartIdle();
    }

    public void OnState()
    {
        
        
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
