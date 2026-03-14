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
        throw new System.NotImplementedException();
    }

    public void OnState()
    {
        
        
        //状态转换
        if (Context.canChase && Context.sightObstructed)
        {
            Context.SwitchState(RemoteBulletState.Chase);
        }
        else if(Context.lockTarget && !Context.sightObstructed && Context.insight)
        {
            Context.SwitchState(RemoteBulletState.Attacking);
        }
    }

    public void OnExitState()
    {
        throw new System.NotImplementedException();
    }
}
