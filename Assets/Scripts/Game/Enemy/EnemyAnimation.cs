using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyAnimation : MonoBehaviour
{
    public enum AnimationState
    {
        Idle,
        Walking,
        Jumping,
        Landing
    }

    public float BreathSpeed;
    public float BreathRange;
    
    public float WalkSpeed;
    public float WalkRange;
    
    public float JumpSpeed;
    public float JumpRange;
    
    public float LandingSpeed;
    public float LandingRange;
    
    public AnimationState state;
    
    public int Move;
    public bool isGrounded;
    
    public float timer;
    private float t2;

    public int direction = 1;

    private Rigidbody2D rb;

    private float effectTimer;
    
    void Awake()
    {
        rb = GetComponentInParent<Rigidbody2D>();
    }
    
    void Start()
    {
        state =  AnimationState.Idle;
        direction = 1;
    }

    public void SetMove(int move)
    {
        Move = move;
    }

    public void SetIsGrounded(bool grounded)
    {
        isGrounded = grounded;
    }

    void Update()
    {
        if (Move == 1)
        {
            transform.right = new Vector3(1, 0, 0);
        }
        else if (Move == -1)
        {
            transform.right = new Vector3(-1, 0, 0);
        }

        if (state is AnimationState.Idle or AnimationState.Walking)
        {
            if (Move == 0)
                state = AnimationState.Idle;
            else
                state = AnimationState.Walking;

            if (!isGrounded)
                state = AnimationState.Jumping;
        }

        if (state is AnimationState.Jumping)
        {
            if (isGrounded)
            {
                timer = 0;
                state = AnimationState.Landing;
            }

        }

        if (state == AnimationState.Idle)
        {
            if (direction == 1)
            {
                if (timer <= 1)
                {
                    timer += Time.deltaTime * BreathSpeed;
                    float ctimer = Math.Clamp(timer, 0, 1);
                    t2 = ctimer * (2 - ctimer);
                }
                else
                {
                    direction = -1;
                    timer = 1;
                }
            }
            else if (direction == -1)
            {
                if (timer >= 0)
                {
                    timer -= Time.deltaTime * BreathSpeed;
                    float ctimer = Math.Clamp(timer, 0, 1);
                    t2 = ctimer * ctimer;
                }
                else
                {
                    direction = 1;
                    timer = 0;
                }
            }

            float h = (t2 - 0.5f) * BreathRange;
            float w = -(t2 - 0.5f) * BreathRange;

            transform.localScale = new Vector3(1 + w, 1 + h, 1);

            transform.localPosition = new Vector3(0, h / 2, 0);
        }

        if (state == AnimationState.Walking)
        {
            if (direction == 1)
            {
                if (timer <= 1)
                {
                    timer += Time.deltaTime * WalkSpeed;
                    float ctimer = Math.Clamp(timer, 0, 1);
                    t2 = ctimer * (2 - ctimer);
                }
                else
                {
                    direction = -1;
                    timer = 1;
                }
            }
            else if (direction == -1)
            {
                if (timer >= 0)
                {
                    timer -= Time.deltaTime * WalkSpeed * 4;
                    float ctimer = Math.Clamp(timer, 0, 1);
                    t2 = ctimer;
                }
                else
                {
                    direction = 1;
                    timer = 0;
                }
            }

            float h = (t2 - 0.5f) * WalkRange;
            float w = -(t2 - 0.5f) * WalkRange;

            transform.localScale = new Vector3(1 + w, 1 + h, 1);

            transform.localPosition = new Vector3(0, h / 2, 0);

            if (effectTimer < 0.2f)
            {
                effectTimer += Time.deltaTime;
            }
            else
            {
                effectTimer = 0;
                ParticleVEPool.Instance.Play(transform.position - new Vector3(Move * 0.5f, 0.5f, 0), 0.3f, 5,
                    new Vector2(-Move, Random.Range(0.2f, 0.5f)), new Vector3(0.1f, 0.1f, 0.1f),
                    new Vector3(0.2f, 0.2f, 0.2f), Color.white);
            }
        }

        if (state == AnimationState.Jumping)
        {
            float s = Math.Abs(rb.velocity.y);


            float h = (s * JumpSpeed - JumpRange);
            float w = -(s * JumpSpeed - JumpRange);

            transform.localScale = new Vector3(1 + w, 1 + h, 1);

            transform.localPosition = new Vector3(0, h / 2, 0);
        }

        if (state == AnimationState.Landing)
        {
            float pi = 3.1415f;

            if (timer <= 1)
            {
                timer += Time.deltaTime * LandingSpeed;
            }
            else
            {
                timer = 1;
                direction = -1;
                state = AnimationState.Idle;
            }

            float y = (float)Math.Cos((1.5f * pi) * timer + 0.5f * pi);

            float h = (y - 1f) * LandingRange;
            float w = -(y - 1f) * LandingRange;

            transform.localScale = new Vector3(1 + w, 1 + h, 1);

            transform.localPosition = new Vector3(0, h / 2, 0);
        }
    }
}
