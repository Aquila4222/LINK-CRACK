using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class SighJump : MonoBehaviour
{
    
    public float speed;
    public float range;

    public float timer;
    
    private Vector3 originalPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer  >= 0)
        {
            timer -= Time.deltaTime*speed;
            if (timer <= 1)
            {
                transform.position = new Vector3(originalPosition.x, originalPosition.y+timer*(1-timer)*range, originalPosition.z);
            }
            else
            {
                transform.position = originalPosition;
            }
        }
        else
        {
            timer = 2f;
        }
    }
}
