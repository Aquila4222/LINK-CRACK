using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum MeleeStateType
{
    Attacking,
    Patrolling,
    Chasing
}

public class MeleeEnemy : Enemy
{
    //状态机
    private FSM<MeleeStateType,MeleeEnemy> meleeFSM;
    [SerializeField] private bool drawLine;

    [Header("角色参数")]
    [SerializeField] public Vector3 facingDirection;
    [SerializeField] public Rigidbody2D rigidBody;
    [SerializeField] private EnemyAnimation enemyAnimation;
    [SerializeField] private SpriteRenderer enemySpriteRenderer;

    public delegate void Accumulate();
    public delegate void Spike();
    public delegate void SpikeEnd();
    
    public event Accumulate StartAccumulate;
    public event Spike StartSpike;
    public event SpikeEnd OnSpikeEnd;

    public void InvokeAccumulate()
    {
        StartAccumulate?.Invoke();
    }

    public void InvokeSpike()
    {
        StartSpike?.Invoke();
    }

    public void InvokeSpikeEnd()
    {
        OnSpikeEnd?.Invoke();
    }
    
    
    [Header("状态转换条件")]
    [SerializeField] public Transform targetTransform;
    [SerializeField] private bool lockedPlayer;
    [SerializeField] private bool inSight;
    [SerializeField] private bool sightObstructed;
    [SerializeField] private bool canAttack;
    [SerializeField] public bool onAttack;

    [Header("检测参数")]
    [SerializeField] private Collider2D enemyCollider;
    [SerializeField] private Collider2D hurtCollider;
    [SerializeField] private Collider2D[] detectedColliders;
    [SerializeField] private Collider2D[] attackColliders;
    [SerializeField] private float detectRadius;
    [SerializeField] private float attackRadius;
    [SerializeField] private Vector3 attackSize;
    [SerializeField] private Vector3 jumpDetectionStartOffsetDistance;
    [SerializeField] private Vector3 jumpDetectionEndOffsetDistance;
    [SerializeField] private Vector3 fallDetectionOffsetDistance;
    [SerializeField] private float fallDetectionDistance;
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private LayerMask whatIsCanHurt;
    [SerializeField] private LayerMask whatIsObstruction;
    [SerializeField] private float maxLockTime;
    [SerializeField] private float lockTime;
    [SerializeField] public bool onGroundForJump;
    [SerializeField] private bool onGroundForFalling;

    [Header("调试状态参数")]
    [SerializeField] private MeleeStateType meleeState;
    [SerializeField] public float maxTurnAroundTime;
    [SerializeField] public float turnAroundTime;
    [SerializeField] public float patrolSpeed;
    [SerializeField] public float chaseSpeed;
    
    [Header("攻击状态参数")]
    [SerializeField] public float idleTime;
    [SerializeField] public int attackState;
    [SerializeField] public Transform animationTransform;

    [Header("攻击状态参数改动区")]
    [SerializeField] public float maxShakeAfterAttackTime;
    [SerializeField] public float spikeMaxTime;
    [SerializeField] public Vector3 spikeOffsetDistance;
    [SerializeField] public float maxAccumulateTime;
    [SerializeField] public float damageStartTime;
    [SerializeField] public float damageContinueTime;
    [SerializeField] public float spikeSpeed;
    [SerializeField] public float reboundForce;

    public void Initialized()
    {
        enemyCollider = gameObject.GetComponent<Collider2D>();
        facingDirection = new Vector2(transform.localScale.x, 0);
        rigidBody = GetComponent<Rigidbody2D>();
        detectedColliders = new Collider2D[1];
        attackColliders = new Collider2D[10];
        
        //状态机初始化
        MeleePatrol meleePatrol = new(this);
        MeleeChase meleeChase = new(this);
        MeleeAttack meleeAttack = new MeleeAttack(this);
        
        meleeFSM = new FSM<MeleeStateType, MeleeEnemy>(MeleeStateType.Patrolling, meleePatrol);
        meleeFSM.AddState(MeleeStateType.Patrolling,meleePatrol);
        meleeFSM.AddState(MeleeStateType.Chasing,meleeChase);
        meleeFSM.AddState(MeleeStateType.Attacking,meleeAttack);

    }

    protected new void Awake()
    {
        base.Awake();
        Initialized();
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialized();
    }

    // Update is called once per frame
    protected new void Update()
    {
        base.Update();

        
        
        enemyAnimation.SetIsGrounded(onGroundForJump);
        enemyAnimation.IsFrozen = currentState == EnemyStateType.OnFroze;
        if (currentState == EnemyStateType.OnFroze || rb.velocity.x == 0)
        {
            enemyAnimation.SetMove(0);
        }
    }

    protected override void Alive()
    {
        meleeState = meleeFSM.CurrentEnumState;

        facingDirection = transform.localScale;
        
        Detect();
        JudgeState();
        meleeFSM.OnState();
        
        if (rigidBody.velocity.x > 0)
        {
            enemyAnimation.SetMove(1);
        }
        else if (rigidBody.velocity.x < 0)
        {
            enemyAnimation.SetMove(-1);
        }
    }

    /// <summary>
    /// 攻击
    /// </summary>
    public Collider2D Attack()
    {
        //受击调用
        int attackNum = Physics2D.OverlapBoxNonAlloc(transform.position ,attackSize,0,attackColliders,whatIsCanHurt);
        if (attackNum > 1)
        {
            foreach (var canHurt in attackColliders)
            {
                if (canHurt != enemyCollider)
                {
                    canHurt.gameObject.GetComponent<IHurt>()?.Hurt(Vector2.zero);
                    hurtCollider = canHurt;
                    return canHurt;
                }
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="speed"></param>
    /// <param name="direction"></param>
    public void Move(float speed,Vector2 direction)
    {
        if (rigidBody != null && onGroundForFalling)
        {
            rigidBody.velocity = new Vector2(direction.x * speed, rigidBody.velocity.y);
        }
        else
        {
            rigidBody.velocity = Vector2.zero;
        }
        
        
    }

    public override void Hurt(Vector2 repulseForce, float damage = 1)
    {
        //base.Hurt(repulseForce, damage);
        StartCoroutine(HurtEffect(enemySpriteRenderer));
    }
    
    IEnumerator HurtEffect(SpriteRenderer spriteRenderer)
    {
        spriteRenderer.enabled = true;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.enabled = false;
    }
    
    /// <summary>
    /// 转身
    /// </summary>
    public void TurnAround()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }
    
    /// <summary>
    /// 条件检测
    /// </summary>
    private void Detect()
    {
        //检测圆形范围内是否有角色
        int detectedColliderNum = Physics2D.OverlapCircleNonAlloc(transform.position, detectRadius, detectedColliders, whatIsPlayer);
        inSight = detectedColliderNum > 0;
        if (detectedColliderNum > 0)
        {
            lockedPlayer = true;
            lockTime = maxLockTime;
            targetTransform = detectedColliders[0].transform;
        }
        

        //锁定角色后进行视线判定
        if (lockedPlayer)
        {
            sightObstructed = Physics2D.Linecast(transform.position, targetTransform.position, whatIsObstruction);
            if (Vector2.Distance(targetTransform.position, transform.position) < attackRadius && !sightObstructed)
            {
                canAttack = true;
            }
            else
            {
                canAttack = false;
            }

            /*if (Vector2.Distance(targetTransform.position, transform.position) > detectRadius)
            {
                detectedColliders[0] = null;
            }*/
        }

        //脱离视野后回正
        if (detectedColliderNum == 0 && lockedPlayer)
        {
            lockTime -= Time.deltaTime;
            if (lockTime <= 0)
            {
                lockedPlayer = false;
                targetTransform = null;
            }
        }
        
        //地面检测(分坠崖检测和跳跃检测两部分）
        onGroundForJump = Physics2D.Linecast(transform.position + jumpDetectionStartOffsetDistance, transform.position + jumpDetectionEndOffsetDistance, whatIsGround);
        onGroundForFalling = Physics2D.Raycast(transform.position + new Vector3(fallDetectionOffsetDistance.x * facingDirection.x,fallDetectionOffsetDistance.y,fallDetectionOffsetDistance.z),Vector2.down,fallDetectionDistance,whatIsGround);
        
    }

    /// <summary>
    /// 状态转换判断逻辑
    /// </summary>
    private void JudgeState()
    {
        switch (meleeFSM.CurrentEnumState)
        {
            case MeleeStateType.Attacking:
                if (!lockedPlayer && !inSight && !onAttack && !onSpike)
                {
                    meleeFSM.SwitchState(MeleeStateType.Patrolling);
                }
                else if (lockedPlayer && !canAttack &&!onAttack && !onSpike)
                {
                    meleeFSM.SwitchState(MeleeStateType.Chasing);
                }
                break;
            case MeleeStateType.Patrolling:
                if (lockedPlayer && canAttack)
                {
                    meleeFSM.SwitchState(MeleeStateType.Attacking);
                }
                else if (lockedPlayer && !sightObstructed)
                {
                    meleeFSM.SwitchState(MeleeStateType.Chasing);
                }
                
                break;
            case MeleeStateType.Chasing:
                if (inSight && lockedPlayer && canAttack)
                {
                    meleeFSM.SwitchState(MeleeStateType.Attacking);
                }
                else if(!lockedPlayer && !inSight)
                {
                    meleeFSM.SwitchState(MeleeStateType.Patrolling);
                }

                break;
            default:
                break;
        }
    }

    private void OnDrawGizmos()
    {
        if(!drawLine)
            return;
        
        // 绘制检测半径（半透明，表示范围）
        Gizmos.color = new Color(0, 1, 0, 0.3f); // 半透明绿
        Gizmos.DrawWireSphere(transform.position, detectRadius);

        // 如果锁定了玩家，绘制视线和攻击范围
        if (lockedPlayer && targetTransform != null)
        {
            // 绘制攻击半径
            Gizmos.color = canAttack ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, attackRadius);

            // 绘制视线（从敌人到玩家）
            if (sightObstructed)
            {
                Gizmos.color = Color.red; // 被遮挡
                // 还可以绘制碰撞点
                RaycastHit2D hit = Physics2D.Linecast(transform.position, targetTransform.position, whatIsObstruction);
                if (hit.collider != null)
                {
                    Gizmos.DrawSphere(hit.point, 0.2f); // 标记遮挡点
                }
            }
            else
            {
                Gizmos.color = Color.green; // 视线清晰
            }
            Gizmos.DrawLine(transform.position, targetTransform.position);
        }
        else
        {
            // 未锁定玩家：检测半径内如果有玩家，可以画一个提示点？
            // 可选：绘制 detectedColliders 中的玩家位置
            if (detectedColliders != null)
            {
                Gizmos.color = Color.cyan;
                foreach (var col in detectedColliders)
                {
                    if (col != null)
                        Gizmos.DrawSphere(col.transform.position, 0.3f);
                }
            }
        }
        
        Vector3 jumpStart = transform.position + (Vector3)jumpDetectionStartOffsetDistance;
        Vector3 jumpEnd   = transform.position + (Vector3)jumpDetectionEndOffsetDistance;

        // 根据检测结果设置颜色（需要运行时才能得到结果，编辑模式下可能为false）
        // 如果想在编辑模式下也看到固定颜色，可以直接使用一种颜色
        Gizmos.color = onGroundForJump ? Color.red : Color.green;
        Gizmos.DrawLine(jumpStart, jumpEnd);

        // 2. 绘制下落检测射线（Raycast）
        Vector3 fallStart = transform.position + (Vector3)fallDetectionOffsetDistance;
        Vector3 fallEnd   = fallStart + Vector3.down * fallDetectionDistance;

        Gizmos.color = onGroundForFalling ? Color.red : Color.green;
        Gizmos.DrawLine(fallStart, fallEnd);
    }
    
}
