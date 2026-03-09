using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleePatrol : IState<MeleeStateType>
{
    public MeleeStateType LastState { get; set; }
    
    public void OnEnterState(MeleeStateType lastState)
    {
        throw new System.NotImplementedException();
    }

    public void OnState()
    {
        throw new System.NotImplementedException();
    }

    public void OnExitState()
    {
        throw new System.NotImplementedException();
    }
    
}
