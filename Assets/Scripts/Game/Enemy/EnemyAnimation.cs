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
        Landing,
        Accumulate,
        Spike,
    }

    public float BreathSpeed;
    public float BreathRange;
    
    public float WalkSpeed;
    public float WalkRange;
    
    public float JumpSpeed;
    public float JumpRange;
    
    public float LandingSpeed;
    public float LandingRange;
    
    public float AccumulateSpeed;
    public float AccumulateRange;
    
    public float SpikeSpeed;
    
    public AnimationState state;
    
    public int Move;
    public bool isGrounded;
    
    public float timer;
    private float t2;

    public int direction = 1;

    private Rigidbody2D rb;

    private float effectTimer;
    
    private MeleeEnemy meleeEnemy;

    public bool IsFrozen;
    
    void Awake()
    {
        rb = GetComponentInParent<Rigidbody2D>();
        meleeEnemy = GetComponentInParent<MeleeEnemy>();
        if (meleeEnemy)
        {
            meleeEnemy.StartAccumulate += Accumulate;
            meleeEnemy.StartSpike += Spike;
            meleeEnemy.OnSpikeEnd += SpikeEnd;
        }
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

    private void Accumulate()
    {
        state =  AnimationState.Accumulate;
        timer = 0;
    }

    private void Spike()
    {
        state =  AnimationState.Spike;
        timer = 0;
    }

    private void SpikeEnd()
    {
        state =  AnimationState.Idle;
        timer = 0;
    }
    
    
    void Update()
    {

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

        if (state is AnimationState.Accumulate)
        {
            timer += Time.deltaTime * AccumulateSpeed;
            float ctimer = Math.Clamp(timer, 0, 1);
            t2 = ctimer * (2 - ctimer);
            float h = (t2 - 0.5f) * AccumulateRange;
            float w = -(t2 - 0.5f) * AccumulateRange;

            transform.localScale = new Vector3(1 + w, 1 + h, 1);

            transform.localPosition = new Vector3(w/2, h / 2, 0);
            if (timer > 2)
            {
                timer = 0;
                state = AnimationState.Idle;
            }
        }

        if (state is AnimationState.Spike)
        {
            transform.localEulerAngles +=  new Vector3(0, 0, SpikeSpeed*Time.deltaTime);
            transform.localScale = Vector3.one;
            if (timer < 0.5)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer = 0;
                state = AnimationState.Idle;
            }
        }
        else
        {
            transform.localEulerAngles = Vector3.zero;
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

        if (IsFrozen)
        {
            transform.localEulerAngles = Vector3.zero;
            transform.localScale = Vector3.one;
        }
    }
}
