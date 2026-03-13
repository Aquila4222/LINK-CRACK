using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float MoveSpeed;
    
    public LayerMask GroundMask;
    public LayerMask ObjectMask;

    public float RayHeight;
    public float RayLength;
    
    public float JumpForce;
    
    private int xDirection;
    
    private Rigidbody2D rb;
    
    private bool isGrounded;
    
    private PlayerAnimation playerAnimation;
    

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnimation = GetComponentInChildren<PlayerAnimation>();
        InputController.Instance.RegisterJump(Jump);
    }

    private void OnDestroy()
    {
        InputController.Instance.UnRegisterJump(Jump);
    }


    private void Jump()
    {
        if (isGrounded)
        {
            rb.velocity =  new Vector2(rb.velocity.x, JumpForce);
        }
    }
        
        
    void Update()
    {
        if (InputController.Instance.MoveInput.x > 0)
        {
            xDirection = 1;
        }
        else if (InputController.Instance.MoveInput.x < 0)
        {
            xDirection = -1;
        }
        else
        {
            xDirection = 0;
        }
        
        isGrounded = Physics2D.Raycast(transform.position + new Vector3(-RayLength/2,RayHeight), Vector2.right, RayLength, GroundMask)
            ||Physics2D.Raycast(transform.position + new Vector3(-RayLength/2,RayHeight), Vector2.right, RayLength, ObjectMask);
        
        playerAnimation.SetMove(xDirection);
        playerAnimation.SetIsGrounded(isGrounded);
    }


    void FixedUpdate()
    {
        rb.velocity = new Vector2(xDirection * MoveSpeed, rb.velocity.y);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position + new Vector3(-RayLength/2,RayHeight) , transform.position + new Vector3(RayLength/2,RayHeight));
    }
}
