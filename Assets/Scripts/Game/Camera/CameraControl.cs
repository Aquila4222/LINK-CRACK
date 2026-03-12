using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private static CameraControl _instance;
    public static CameraControl Instance =>  _instance;
    
    public Transform PlayerTransform;
    private Rigidbody2D cameraRB;
    public float speed;

    void Awake()
    {
        _instance = this;
        cameraRB = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        cameraRB.velocity = (PlayerTransform.position-transform.position)*speed;
    }
    
    public void Shock(Vector3 position)
    {
        StartCoroutine(PlayShock(position));
    }

    IEnumerator PlayShock(Vector3 position)
    {
        float intensity = 1f / ((position - transform.position).magnitude + 1f);
        for (int i = 5; i > 0; i--)
        {
            transform.position+=(Vector3)(Vector2)Random.onUnitSphere.normalized * (intensity * i);
            yield return new WaitForSeconds(0.05f);
        }
    }
}
