using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RemoteLaserState
{
    Idle,
    Chasing,
    Attacking,
}

public class RemoteLaserEnemy : Enemy
{
    private FSM<RemoteLaserState,RemoteLaserEnemy> remoteFSM;
    
    [Header("敌人参数")]
    [SerializeField] public Vector3 facingDirection;
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private SpriteRenderer enemySpriteRenderer;
    [SerializeField] private EnemyAnimation enemyAnimation;

    [Header("弹幕参数")] 
    [SerializeField] public GameObject warningLine;
    [SerializeField] public Transform gunTransform;
    [SerializeField] public Transform bulletGenerateTransform;
    [SerializeField] public float angleOffset;
    
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
    [SerializeField] public LayerMask whatCanBlockLaser;
    [SerializeField] public bool lockTarget;
    [SerializeField] public bool insight;
    [SerializeField] public bool sightObstructed;
    [SerializeField] private float maxLockTime;
    [SerializeField] private float lockTime;
    
    [Header("追击状态参数")] 
    [SerializeField] public float chaseSpeed;

    [Header("攻击状态参数")] 
    [SerializeField] public float warningTime;
    [SerializeField] public float accumulateMaxTime;
    [SerializeField] public float maxAfterAttackShakeTime;

    private void Initialized()
    {
        targetColliders = new Collider2D[1];
        rigidbody = GetComponent<Rigidbody2D>();

        RemoteLaserAttack attack = new RemoteLaserAttack(this);
        RemoteLaserChase chase = new RemoteLaserChase(this);
        RemoteLaserIdle idle = new RemoteLaserIdle(this);
        remoteFSM = new FSM<RemoteLaserState, RemoteLaserEnemy>(RemoteLaserState.Idle, idle);
        remoteFSM.AddState(RemoteLaserState.Chasing, chase);
        remoteFSM.AddState(RemoteLaserState.Attacking,attack);
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
    
    protected override void Alive()
    {
        base.Alive();
        
        Detect();
        remoteFSM.OnState();
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
    public void SwitchState(RemoteLaserState state)
    {
        remoteFSM.SwitchState(state);
    }
    
    /// <summary>
    /// 射击激光方法
    /// </summary>
    /// <param name="bulletPos"></param>
    /// <param name="bulletSpeed"></param>
    public void ShootLaser(Vector3 bulletPos, Vector3 bulletSpeed)
    {
        LaserPool.Instance.Shoot(bulletPos, bulletSpeed);
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
}
