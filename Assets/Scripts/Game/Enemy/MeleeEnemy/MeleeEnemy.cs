using System.Collections;
using System.Collections.Generic;
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
    
    //状态转换条件
    private bool _inAttackingRange = false;
    private bool _inChasingRange = false;
    

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

    private void Detect()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f);
        Debug.DrawRay(transform.position, Vector2.down, Color.red);
        
    }
}
