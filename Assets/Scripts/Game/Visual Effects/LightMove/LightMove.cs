using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;


public class LightMove : MonoBehaviour
{
    public float MoveRange;
    public float Speed;
    private float timer;
    private Vector3 originalRotation;
    private float offset;

    private Light2D light2D;
    
    void Awake()
    {
        light2D = GetComponent<Light2D>();
        
        originalRotation = transform.localEulerAngles;
        offset = Random.Range(0f,6.28f);
        
        float c = Random.Range(-1f, 1f);
        if (c >= 0)
        {
            light2D.color = new Color(0, c, 1, 1);
        }
        else
        {
            light2D.color = new Color(-c, 0, 1, 1);
        }

    }
    private void Update()
    {
        timer += Time.deltaTime*Speed;
        float y = Mathf.Sin(timer+offset);
        
        transform.localEulerAngles = originalRotation+ new Vector3(0, 0, y*MoveRange);
    }
}
