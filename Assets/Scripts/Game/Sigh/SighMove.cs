using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SighMove : MonoBehaviour
{

    public float range;
    public float speed;
    
    private Vector3 originalPosition;
    
    
    
    void Start()
    {
        originalPosition =  transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(originalPosition.x+Mathf.Sin(Time.time*speed)*range, originalPosition.y, originalPosition.z);
    }
}
