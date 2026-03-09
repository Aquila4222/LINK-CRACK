using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState<TStateEnum> where TStateEnum : Enum
{
    public TStateEnum LastState {get; set; }
    
    public void OnEnterState(TStateEnum lastState);
    public void OnState();
    public void OnExitState();
}
