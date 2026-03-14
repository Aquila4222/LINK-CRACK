using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RemoteBulletState
{
    Idle,
    Chase,
    Attacking,
    
}

public class RemoteBulletEnemy : Enemy
{
    private FSM<RemoteBulletState, RemoteBulletEnemy> remoteFSM;
    
    [Header("调试检测参数")]
    [SerializeField] private RemoteBulletState currentRemoteState;
    
    [Header("敌人参数")]
    [SerializeField] private Vector3 facingDirection;
    [SerializeField] private Vector3 bulletGeneratePosition;
    

    [Header("检测参数")] 
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Collider2D[] targetColliders;
    [SerializeField] private float alertRadius;
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private LayerMask whatIsObstruction;
    [SerializeField] public bool lockTarget;
    [SerializeField] public bool insight;
    [SerializeField] public bool sightObstructed;
    [SerializeField] public bool canChase;
    [SerializeField] private float maxLockTime;
    [SerializeField] private float lockTime;

    [Header("追击状态参数")] 
    [SerializeField] private float chaseSpeed;
    
    [Header("攻击状态参数")]
    [SerializeField] public float accumulateMaxTime;
    [SerializeField] public float maxAfterAttackShakeTime;

    public void Initialized()
    {
        targetColliders = new Collider2D[1];

        RemoteBulletAttack remoteBulletAttack = new RemoteBulletAttack(this);
        RemoteBulletChase remoteBulletChase = new RemoteBulletChase(this);
        RemoteBulletIdle remoteBulletIdle = new RemoteBulletIdle(this);
        remoteFSM = new FSM<RemoteBulletState, RemoteBulletEnemy>(RemoteBulletState.Idle,remoteBulletIdle);
        remoteFSM.AddState(RemoteBulletState.Chase,remoteBulletChase);
        remoteFSM.AddState(RemoteBulletState.Attacking,remoteBulletAttack);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Initialized();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    /// <summary>
    /// 活着，不被抓取状态
    /// </summary>
    protected override void Alive()
    {
        base.Alive();
     
        currentRemoteState = remoteFSM.CurrentEnumState;
        
        
        Detect();
        remoteFSM.OnState();
    }

    public void Move(float speed, Vector3 direction)
    {
        
    }
    
    /// <summary>
    /// 提供给子状态的状态转换函数
    /// </summary>
    /// <param name="state"></param>
    public void SwitchState(RemoteBulletState state)
    {
        remoteFSM.SwitchState(state);
    }
    
    /// <summary>
    /// 射击弹幕方法
    /// </summary>
    /// <param name="bulletPos"></param>
    /// <param name="bulletSpeed"></param>
    public void ShootBullet(Vector3 bulletPos, Vector3 bulletSpeed)
    {
        //TODO:射击弹幕方法待写
    }
    
    /// <summary>
    /// 检测
    /// </summary>
    private void Detect()
    {
        int targetNum = Physics2D.OverlapCircleNonAlloc(transform.position,alertRadius,targetColliders,whatIsPlayer);
        insight = targetNum > 0;
        if (targetNum > 0)
        {
            lockTarget =  true;
            targetTransform = targetColliders[0].transform;
            lockTime =maxLockTime;
        }

        if (lockTarget)
        {
            sightObstructed = Physics2D.Linecast(transform.position, targetTransform.position, whatIsObstruction);
            if (!sightObstructed)
            {
                canChase = true;
            }
        }
        
        
        //脱离视野回正
        if (targetNum == 0 && lockTarget)
        {
            lockTime -= Time.deltaTime;
            if (lockTime < 0)
            {
                lockTarget = false;
                canChase = false;
                targetTransform = null;
            }
        }
    }

    
}
