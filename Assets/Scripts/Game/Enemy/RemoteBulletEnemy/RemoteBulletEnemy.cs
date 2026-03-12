using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum RemoteBulletState
{
    Idle,
    Alert,
    Attacking,
    
}

public class RemoteBulletEnemy : Enemy
{
    private FSM<RemoteBulletState, RemoteBulletEnemy> remoteFSM;
    
    [Header("敌人参数")]
    [SerializeField] private Transform bullletTransform;

    [Header("检测参数")] 
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Collider2D[] targetColliders;
    [SerializeField] private float alertRadius;
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private LayerMask whatIsObstruction;
    [SerializeField] private bool lockTarget;
    [SerializeField] private bool sightObstructed;
    [SerializeField] private bool canAttack;
    [SerializeField] private float maxLockTime;
    [SerializeField] private float lockTime;

    public void Initialized()
    {
        targetColliders = new Collider2D[1];
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    /// <summary>
    /// 活着，不被抓取状态
    /// </summary>
    protected override void Alive()
    {
        
    }

    /// <summary>
    /// 检测
    /// </summary>
    private void Detect()
    {
        int targetNum = Physics2D.OverlapCircleNonAlloc(transform.position,alertRadius,targetColliders,whatIsPlayer);
        if (targetNum > 0)
        {
            lockTarget =  true;
            targetTransform = targetColliders[0].transform;
            lockTime =maxLockTime;
        }

        if (lockTarget)
        {
            sightObstructed = Physics2D.Linecast(transform.position, targetTransform.position, whatIsObstruction);
        }
        
        
        //脱离视野回正
        if (targetNum == 0 && lockTarget)
        {
            lockTime -= Time.deltaTime;
            if (lockTime < 0)
            {
                lockTarget = false;
                targetTransform = null;
            }
        }
    }

    private void JudgeState()
    {
        switch (remoteFSM.CurrentEnumState)
        {
            //TODO:待修改
            case RemoteBulletState.Idle:
                if (lockTarget)
                {
                    remoteFSM.SwitchState(RemoteBulletState.Alert);
                }
                else if (lockTarget && canAttack)
                {
                    remoteFSM.SwitchState(RemoteBulletState.Attacking);
                }
                break;
            case RemoteBulletState.Alert:
                if (canAttack)
                {
                    remoteFSM.SwitchState(RemoteBulletState.Attacking);
                }
                else if (!lockTarget)
                {
                    remoteFSM.SwitchState(RemoteBulletState.Idle);
                }
                break;
            case RemoteBulletState.Attacking:
                if (lockTarget && !canAttack)
                {
                    remoteFSM.SwitchState(RemoteBulletState.Alert);
                }
                else if (lockTarget)
                {
                    
                }
                break;
            default:
                break;
        }
    }
}
