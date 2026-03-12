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
        
    }

    private void Detect()
    {
        int targetNum = Physics2D.OverlapCircleNonAlloc(transform.position,alertRadius,targetColliders,whatIsPlayer);
        if (targetNum > 0)
        {
            targetTransform = targetColliders[0].transform;
        }
        
    }
}
