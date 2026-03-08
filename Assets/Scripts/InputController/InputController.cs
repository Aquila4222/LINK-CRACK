using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    /// <summary>
    /// 移动向量
    /// </summary>
    public Vector2 MoveInput => _moveInput;

    /// <summary>
    /// 注册跳跃
    /// </summary>
    /// <param name="a"></param>
    public void RegisterJump(Action a)
    {
        jumpAction += a;
    }
    
    /// <summary>
    /// 注册链接
    /// </summary>
    /// <param name="a"></param>
    public void RegisterLink(Action a)
    {
        linkAction += a;
    }
    
    /// <summary>
    /// 注册断链
    /// </summary>
    /// <param name="a"></param>
    public void RegisterUnlink(Action a)
    {
        unlinkAction += a;
    }
    
    /// <summary>
    /// 注册切换链接形态
    /// </summary>
    /// <param name="a"></param>
    public void RegisterLinkSwitch(Action a)
    {
        switchLinkAction += a;
    }
    
    private Action jumpAction;
    private Action linkAction;
    private Action unlinkAction;
    private Action switchLinkAction;
    
    // 引用上一步生成的C#类
    private PlayerInputs _playerControls;
    
    private Vector2 _moveInput;
    
    private void Awake()
    {
        // 实例化
        _playerControls = new PlayerInputs();
    }

    private void OnEnable()
    {
        // 启用输入
        _playerControls.Enable();

        // 为“Jump”动作注册一个“执行时”的回调函数
        // 当玩家按下跳跃键，这个方法就会被自动调用
        _playerControls.Gameplay.Jump.performed += OnJumpPerformed;
        _playerControls.Gameplay.Link.performed += OnLinkPerformed;
        _playerControls.Gameplay.Unlink.canceled += OnUnlinkPerformed;
        _playerControls.Gameplay.SwichLink.performed += OnSwitchLinkPerformed;
    }

    private void OnDisable()
    {
        // 禁用输入并注销回调，防止出错
        _playerControls.Disable();
        _playerControls.Gameplay.Jump.performed -= OnJumpPerformed;
        _playerControls.Gameplay.Link.performed -= OnLinkPerformed;
        _playerControls.Gameplay.Unlink.canceled -= OnUnlinkPerformed;
        _playerControls.Gameplay.SwichLink.performed -= OnSwitchLinkPerformed;
    }

    private void Update()
    {
        _moveInput = _playerControls.Gameplay.Move.ReadValue<Vector2>();
    }
    
    
    
    
    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Jump performed!");
        jumpAction();
    }
    
    private void OnLinkPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Link performed!");
        linkAction();
    }

    private void OnUnlinkPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Unlink performed!");
        unlinkAction();
    }
    
    private void OnSwitchLinkPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Switch link performed!");
        switchLinkAction();
    }

}
