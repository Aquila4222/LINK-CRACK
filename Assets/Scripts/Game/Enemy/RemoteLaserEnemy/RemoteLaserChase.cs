using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class RemoteLaserChase : IState<RemoteLaserState,RemoteLaserEnemy>
{

    private float chaseSpeed;
    public RemoteLaserEnemy Context { get; set; }
    
    public RemoteLaserChase(RemoteLaserEnemy  context)
    {
        Context = context;
    }
    
    public void OnEnterState(RemoteLaserState lastState)
    {
        chaseSpeed = Context.chaseSpeed;
    }

    public void OnState()
    {
        if (Context.targetTransform != null)
        {
            Vector3 direction = Context.targetTransform.position.x > Context.transform.position.x ? Vector3.right : Vector3.left;
            Context.TurnOrientation(direction);
            if (math.abs(Context.targetTransform.position.x - Context.transform.position.x) > 0.3)
            {
                Context.Move(chaseSpeed,direction);
            }
            else
            {
                Context.Move(0,direction);
            }
        }
        
        //状态转换
        if (!Context.lockTarget)
        {
            Context.SwitchState(RemoteLaserState.Idle);
        }
        else if (Context.lockTarget && !Context.sightObstructed && Context.insight)
        {
            Context.SwitchState(RemoteLaserState.Attacking);
        }
    }

    public void OnExitState()
    {
        Context.Move(0,Context.facingDirection);
    }
}
