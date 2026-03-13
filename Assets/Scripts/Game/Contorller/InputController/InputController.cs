using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoSingletonHungry<InputController>
{
    /// <summary>
    /// 移动向量
    /// </summary>
    public Vector2 MoveInput => _moveInput;
    
    public Vector2 AimInput => _aimInput;

    public bool isLinking => _playerControls.Gameplay.Link.IsPressed();

    /// <summary>
    /// 注册跳跃
    /// </summary>
    /// <param name="a"></param>
    public void RegisterJump(Action a)
    {
        jumpAction += a;
    }

    public void UnRegisterJump(Action a)
    {
        jumpAction -= a;
    }
    
    /// <summary>
    /// 注册链接
    /// </summary>
    /// <param name="a"></param>
    public void RegisterLink(Action a)
    {
        linkAction += a;
    }

    public void UnRegisterLink(Action a)
    {
        linkAction -= a;
    }
    
    /// <summary>
    /// 注册断链
    /// </summary>
    /// <param name="a"></param>
    public void RegisterUnlink(Action a)
    {
        unlinkAction += a;
    }

    public void UnRegisterUnlink(Action a)
    {
        unlinkAction -= a;
    }
    
    /// <summary>
    /// 注册切换链接形态
    /// </summary>
    /// <param name="a"></param>
    public void RegisterLinkSwitch(Action a)
    {
        switchLinkAction += a;
    }

    public void UnRegisterLinkSwitch(Action a)
    {
        switchLinkAction -= a;
    }

    public int GetScheme()
    {
        if (_playerInput.currentControlScheme == "Keyboard&Mouse")
            return 0;
        else if (_playerInput.currentControlScheme == "Gamepad")
            return 1;
        else
            return -1;
    }
    
    
    private Action jumpAction;
    private Action linkAction;
    private Action unlinkAction;
    private Action switchLinkAction;
    
    // 引用上一步生成的C#类
    private PlayerInputs _playerControls;

    private PlayerInput _playerInput;
    
    private Vector2 _moveInput;
    
    private Vector2 _aimInput;
    
    
    private void Awake()
    {
        // 实例化
        _playerControls = new PlayerInputs();

        _playerInput = gameObject.AddComponent<PlayerInput>();

        _playerInput.actions = Resources.Load<InputActionAsset>("PlayerInputs");
    }

    private void OnEnable()
    {
        // 启用输入
        _playerControls.Enable();

        // 为“Jump”动作注册一个“执行时”的回调函数
        // 当玩家按下跳跃键，这个方法就会被自动调用
        _playerControls.Gameplay.Jump.performed += OnJumpPerformed;
        _playerControls.Gameplay.Link.performed += OnLinkPerformed;
        _playerControls.Gameplay.Link.canceled += OnUnlinkPerformed;
    }

    private void OnDisable()
    {
        // 禁用输入并注销回调，防止出错
        _playerControls.Disable();
        _playerControls.Gameplay.Jump.performed -= OnJumpPerformed;
        _playerControls.Gameplay.Link.performed -= OnLinkPerformed;
        _playerControls.Gameplay.Link.canceled -= OnUnlinkPerformed;
    }

    private void Update()
    {
        _moveInput = _playerControls.Gameplay.Move.ReadValue<Vector2>();
        
        _aimInput = _playerControls.Gameplay.Aim.ReadValue<Vector2>();
    }
    
    
    
    
    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        jumpAction?.Invoke();
    }
    
    private void OnLinkPerformed(InputAction.CallbackContext context)
    {
        linkAction?.Invoke();
    }

    private void OnUnlinkPerformed(InputAction.CallbackContext context)
    {
        unlinkAction?.Invoke();
    }
    
    private void OnSwitchLinkPerformed(InputAction.CallbackContext context)
    {
        switchLinkAction?.Invoke();
    }

}
