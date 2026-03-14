using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteLaserIdle : IState<RemoteLaserState,RemoteLaserEnemy>
{


    public RemoteLaserEnemy Context { get; set; }

    public RemoteLaserIdle(RemoteLaserEnemy  context)
    {
        Context = context;
    }
    
    public void OnEnterState(RemoteLaserState lastState)
    {
        Context.InvokeStartIdle();
    }

    public void OnState()
    {
        //状态转换

        if(Context.lockTarget && !Context.sightObstructed && Context.insight)
        {
            Context.SwitchState(RemoteLaserState.Attacking);
        }
    }

    public void OnExitState()
    {
        Context.InvokeStopIdle();
    }
}
