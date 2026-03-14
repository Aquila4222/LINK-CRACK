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
        transform.position = new Vector3(PlayerTransform.position.x, PlayerTransform.position.y, transform.position.z) ;
    }

    void FixedUpdate()
    {
        float s;
        if (Time.timeScale != 0)
        {
            s = 1/Time.timeScale;
        }
        else
        {
            s = 1;
        }
        cameraRB.velocity = (PlayerTransform.position-transform.position) * (speed * s);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    public void Shock(Vector3 position)
    {
        StartCoroutine(PlayShock(position));
        //StartCoroutine(SlowTime(((Vector2)position - (Vector2)transform.position).magnitude));
        SEPool.Instance.PlaySE("Shock",1f);
    }

    IEnumerator PlayShock(Vector3 position)
    {
        float intensity = 1f / ((position - transform.position).magnitude + 1f);
        for (int i = 5; i > 0; i--)
        {
            transform.position+=(Vector3)(Vector2)Random.onUnitSphere.normalized * (intensity * i);
            yield return new WaitForSecondsRealtime(0.05f);
        }
    }
    
    IEnumerator SlowTime(float distance)
    {
        yield return new WaitForSecondsRealtime(0.03f);
        float intense = 0;
        if (8 - distance > 0)
        {
            intense = (8-distance)/8;
        }
        else
        {
            intense = 0;
        }
        Time.timeScale = 0.2f;
        yield return new WaitForSecondsRealtime(0.2f*intense);
        Time.timeScale = 1;
    }
}
