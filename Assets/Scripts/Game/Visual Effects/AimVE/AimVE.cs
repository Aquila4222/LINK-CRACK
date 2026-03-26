using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimVE : MonoBehaviour
{
    public Transform[] aims;

    private float timer;

    public float speed;

    private float p0 = 0.34f;
    private float p1 = 0.6f;
    
    private void Update()
    {
        if (InputController.Instance.isLinking)
        {
            if (timer <= 1)
            {
                timer += Time.unscaledDeltaTime * speed;
            }
            else
            {
                timer = 1;
            }
         
        }
        else
        {
            if (timer > 0)
            {
                timer -= Time.unscaledDeltaTime * speed;
            }
            else
            {
                timer = 0;
            }
        }
        foreach (var aim in aims)
        {
            aim.localPosition = new Vector3(aim.localPosition.x, (p0-p1)*timer + p1, aim.localPosition.z);
        }
    }
}
