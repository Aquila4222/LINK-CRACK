using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState<TStateEnum,TContext> where TStateEnum : Enum
{
    public TContext Context {get;set; }
    
    public void OnEnterState(TStateEnum lastState);
    public void OnState();
    public void OnExitState();
}
