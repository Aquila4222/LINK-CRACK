using System;
using System.Collections;
using System.Collections.Generic;
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
    private FSM<MeleeStateType> meleeFSM;

    [Header("状态转换条件")]
    [SerializeField] private Transform targetTransform;
    [SerializeField] private bool lockedPlayer;
    [SerializeField] private bool sightObstructed;
    [SerializeField] private bool canAttack;
    
    [Header("检测参数")]
    [SerializeField] private Collider2D[] detectedColliders;
    [SerializeField] private float detectRadius;
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private LayerMask whatIsObstruction;
    [SerializeField] private float attackRadius;
    

    public void Initialized()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        meleeFSM.OnState();
    }

    /// <summary>
    /// 视野检测
    /// </summary>
    private void Detect()
    {
        //检测圆形范围内是否有角色
        int detectedColliderNum = Physics2D.OverlapCircleNonAlloc(transform.position, detectRadius, detectedColliders, whatIsPlayer);
        if (detectedColliderNum > 0)
        {
            lockedPlayer = true;
            targetTransform = detectedColliders[0].transform;
        }

        //锁定角色后进行视线判定
        if (lockedPlayer)
        {
            sightObstructed = Physics2D.Linecast(transform.position, targetTransform.position, whatIsObstruction);
            if (Vector2.Distance(targetTransform.position, transform.position) < attackRadius)
            {
                canAttack = true;
            }
            else
            {
                canAttack = false;
            }
        }

        
    }

    /// <summary>
    /// 状态转换判断逻辑
    /// </summary>
    private void JudgeState()
    {
        /*switch (meleeFSM.CurrentEnumState)
        {
            case MeleeStateType.Attacking:
                
            case MeleeStateType.Patrolling:
                
            case MeleeStateType.Chasing:
                
            default:
        }*/
    }

    private void OnDrawGizmosSelected()
    {
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
    }
}
