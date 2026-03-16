using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
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
    [SerializeField] private bool drawLine;
    
    [Header("调试检测参数")]
    [SerializeField] private RemoteBulletState currentRemoteState;
    
    [Header("敌人参数")]
    [SerializeField] public Vector3 facingDirection;
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private SpriteRenderer enemySpriteRenderer;
    [SerializeField] private EnemyAnimation enemyAnimation;
    [SerializeField] public Transform accumulateRing;
    
    [Header("弹幕参数")]
    [SerializeField] public Transform gunTransform;
    [SerializeField] public Transform bulletGenerateTransform;
    [SerializeField] public float angleOffset;
    [SerializeField] public float bulletSpeed;
    

    public event Action StartIdle; 
    public event Action StopIdle;
    public event Action StartAccumulate;

    public void InvokeStartIdle()
    {
        StartIdle?.Invoke();
    }

    public void InvokeStopIdle()
    {
        StopIdle?.Invoke();
    }

    public void InvokeStartAccumulate()
    {
        StartAccumulate?.Invoke();
    }
    
    [Header("检测参数")] 
    [SerializeField] public Transform targetTransform;
    [SerializeField] private Collider2D[] targetColliders;
    [SerializeField] private float alertRadius;
    [SerializeField] private Vector3 fallDetectionOffsetDistance;
    [SerializeField] private float fallDetectionDistance;
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private LayerMask whatIsObstruction;
    [SerializeField] public bool lockTarget;
    [SerializeField] public bool insight;
    [SerializeField] public bool sightObstructed;
    [SerializeField] private float maxLockTime;
    [SerializeField] private float lockTime;

    [Header("待机状态参数")]
    [SerializeField] public float maxTurnAroundTime;
    [SerializeField] public float minTurnAroundTime;
    
    [Header("追击状态参数")] 
    [SerializeField] public float chaseSpeed;
    
    [Header("攻击状态参数")]
    [SerializeField] public float accumulateMaxTime;
    [SerializeField] public float maxAfterAttackShakeTime;

    public void Initialized()
    {
        targetColliders = new Collider2D[1];
        rigidbody = GetComponent<Rigidbody2D>();

        RemoteBulletAttack remoteBulletAttack = new RemoteBulletAttack(this);
        RemoteBulletChase remoteBulletChase = new RemoteBulletChase(this);
        RemoteBulletIdle remoteBulletIdle = new RemoteBulletIdle(this);
        remoteFSM = new FSM<RemoteBulletState, RemoteBulletEnemy>(RemoteBulletState.Idle,remoteBulletIdle);
        remoteFSM.AddState(RemoteBulletState.Chase,remoteBulletChase);
        remoteFSM.AddState(RemoteBulletState.Attacking,remoteBulletAttack);
    }

    protected new void Awake()
    {
        base.Awake();
        Initialized();
    }
    

    // Update is called once per frame
    new void Update()
    {
        
        base.Update();

        enemyAnimation.SetIsGrounded(DetectOnGround());
        enemyAnimation.IsFrozen = currentState == EnemyStateType.OnFroze;
        if (currentState == EnemyStateType.OnFroze || rb.velocity.x == 0)
        {
            enemyAnimation.SetMove(0);
        }
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
    
    /// <summary>
    /// 受伤接口
    /// </summary>
    /// <param name="repulseForce"></param>
    /// <param name="damage"></param>
    public override void Hurt(Vector2 repulseForce, float damage = 1)
    {
        base.Hurt(repulseForce, damage);
        StartCoroutine(HurtEffect(enemySpriteRenderer));
    }
    
    IEnumerator HurtEffect(SpriteRenderer spriteRenderer)
    {
        spriteRenderer.enabled = true;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.enabled = false;
    }

    /// <summary>
    /// 转身函数
    /// </summary>
    /// <param name="direction"></param>
    public void TurnOrientation(Vector3 direction)
    {
        facingDirection = direction;
        transform.localScale = new Vector3(facingDirection.x,transform.localScale.y,transform.localScale.z);
    }
    
    /// <summary>
    /// 移动方法
    /// </summary>
    /// <param name="speed"></param>
    /// <param name="direction"></param>
    public void Move(float speed, Vector3 direction)
    {
        bool fallDetection = Physics2D.Raycast(
            transform.position + new Vector3(fallDetectionOffsetDistance.x * facingDirection.x,
                fallDetectionOffsetDistance.y, fallDetectionOffsetDistance.z), Vector2.down, fallDetectionDistance,
            whatIsGround);
        if (rigidbody != null && fallDetection)
        {
            rigidbody.velocity = direction * speed;
        }
        else
        {
            rigidbody.velocity = Vector2.zero;
        }

        if (direction.x > 0)
        {
            enemyAnimation.SetMove(1);
        }
        else if(direction.x < 0)
        {
            enemyAnimation.SetMove(-1);
        }
        else if (direction.x == 0)
        {
            enemyAnimation.SetMove(0);
        }
    }
    
    /// <summary>
    /// 提供给子状态的状态转换方法
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
        BulletPool.Instance.Shoot(bulletPos, bulletSpeed);
    }

    /// <summary>
    /// 枪指向角色
    /// </summary>
    public void PointGunAtTarget()
    {
        if(targetTransform == null)
            return;
        
        Vector2 direction = targetTransform.position - gunTransform.position;
        if (direction == Vector2.zero) return;
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg +  angleOffset;
        
        gunTransform.localScale = transform.localScale;
        gunTransform.rotation = Quaternion.Euler(0,0,angle);
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

    private void OnDrawGizmos()
    {
        if (!drawLine) return;
        // 确保必要组件不为空，避免空引用异常
        if (this == null) return;


        // 1. 绘制视野范围（圆形）
        //    颜色：绿色表示当前看到玩家（insight 为 true），灰色表示未看到
        Gizmos.color = insight ? Color.green : Color.gray;
        Gizmos.DrawWireSphere(transform.position, alertRadius);
        // 1. 绘制坠落检测射线（Move 方法中的射线）
        //    射线起点 = transform.position + fallDetectionOffsetDistance（根据朝向调整 x）
        Vector3 fallStart = transform.position + new Vector3(
            fallDetectionOffsetDistance.x * facingDirection.x,
            fallDetectionOffsetDistance.y,
            fallDetectionOffsetDistance.z);
        Vector3 fallDirection = Vector3.down;
        float fallDistance = fallDetectionDistance;
        Vector3 fallEnd = fallStart + fallDirection * fallDistance;

        // 执行射线检测，判断是否击中地面（使用 whatIsGround，该字段可能定义在基类 Enemy 中）
        bool hitGround = Physics2D.Raycast(fallStart, fallDirection, fallDistance, whatIsGround);

        // 根据是否击中地面设置颜色：击中为绿色，未击中为黄色
        Gizmos.color = hitGround ? Color.green : Color.yellow;
        Gizmos.DrawLine(fallStart, fallEnd);
        // 在起点绘制一个小球，便于观察位置
        Gizmos.DrawSphere(fallStart, 0.1f);

        // 2. 绘制视线阻挡检测射线（Detect 方法中的 Linecast）
        //    只有当锁定目标存在时绘制
        if (lockTarget && targetTransform != null)
        {
            Vector3 sightStart = transform.position;
            Vector3 sightEnd = targetTransform.position;

            // 执行 Linecast 检测是否被障碍物阻挡
            bool obstructed = Physics2D.Linecast(sightStart, sightEnd, whatIsObstruction);

            // 根据是否阻挡设置颜色：阻挡为红色，未阻挡为绿色
            Gizmos.color = obstructed ? Color.red : Color.green;
            Gizmos.DrawLine(sightStart, sightEnd);
            // 在起点和终点绘制小球
            Gizmos.DrawSphere(sightStart, 0.1f);
            Gizmos.DrawSphere(sightEnd, 0.1f);
        }
    }
}
