using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MeleeChase : IState<MeleeStateType,MeleeEnemy>
{
    public MeleeEnemy Context { get; set; }

    private float chaseSpeed;

    public MeleeChase(MeleeEnemy context)
    {
        Context = context;
    }
    
    public void OnEnterState(MeleeStateType lastState)
    {
        chaseSpeed = Context.chaseSpeed;
    }

    public void OnState()
    {
        if (math.abs(Context.transform.position.x - Context.targetTransform.position.x) > 0.3)
        {
            Context.facingDirection = new Vector2(Context.targetTransform.position.x > Context.transform.position.x  ? math.abs(Context.targetTransform.localScale.x) * Vector2.right.x : math.abs(Context.targetTransform.localScale.x) * Vector2.left.x,Context.targetTransform.localScale.y);
            
            Context.Move(chaseSpeed,Context.targetTransform.position.x > Context.transform.position.x  ? Vector2.right : Vector2.left);
        }
        else
        {
            Context.Move(0,Context.targetTransform.position.x > Context.transform.position.x  ? Vector2.right : Vector2.left);
        }
    }

    public void OnExitState()
    {
        Context.Move(0,Context.facingDirection);
    }
}
