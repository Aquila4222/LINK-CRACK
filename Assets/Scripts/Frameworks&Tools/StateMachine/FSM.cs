using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM<TStateEnum> where TStateEnum: Enum
{
    private Dictionary<TStateEnum,IState<TStateEnum>>  _states = new Dictionary<TStateEnum,IState<TStateEnum>>();

    public TStateEnum CurrentEnumState { get; private set; }
    public IState<TStateEnum> CurrentIState { get; private set; }
    
    /// <summary>
    /// 构造方法，注入当前状态
    /// </summary>
    /// <param name="currentState"></param>
    public FSM(TStateEnum currentState)
    {
        CurrentEnumState = currentState;
    }

    /// <summary>
    /// 添加状态
    /// </summary>
    /// <param name="stateEnum"></param>
    /// <param name="state"></param>
    public void AddState(TStateEnum stateEnum, IState<TStateEnum> state)
    {
        _states.Add(stateEnum, state);
    }
    
    /// <summary>
    /// 转换状态
    /// </summary>
    /// <param name="newState"></param>
    public void SwitchState(TStateEnum newState)
    {
        CurrentIState.OnExitState();
        CurrentIState = _states[newState];
        CurrentIState.OnEnterState(CurrentEnumState);
        CurrentEnumState = newState;
    }

    /// <summary>
    /// 状态中逻辑
    /// </summary>
    public void OnState()
    {
        CurrentIState.OnState();
    }
    
    
}
