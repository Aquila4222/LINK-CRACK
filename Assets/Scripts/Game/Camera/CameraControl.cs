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

    private Vector3 _movePosition;
    private bool isFree;
    public float FreeMoveSpeed;
    
    public AudioSource BGMSource;
    private bool isBGMPlaying;
    
    
    void Awake()
    {
        _instance = this;
        cameraRB = GetComponent<Rigidbody2D>();
        transform.position = new Vector3(PlayerLife.SpawnPoint.x, PlayerLife.SpawnPoint.y, transform.position.z) ;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isFree = !isFree;
            _movePosition = PlayerTransform.position;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            _movePosition+=FreeMoveSpeed*Time.deltaTime*Vector3.right;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            _movePosition+=FreeMoveSpeed*Time.deltaTime*Vector3.left;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            _movePosition+=FreeMoveSpeed*Time.deltaTime*Vector3.up;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            _movePosition+=FreeMoveSpeed*Time.deltaTime*Vector3.down;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {

            if (isBGMPlaying)
            {
                BGMSource.Stop();
            }
            else
            {
                BGMSource.Play();
            }
            
            isBGMPlaying = !isBGMPlaying;
        }
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

        if (!isFree)
        {
            cameraRB.velocity = (PlayerTransform.position-transform.position) * (speed * s);
        }
        else
        {
            cameraRB.velocity = (_movePosition - transform.position) * (speed * s);
        }
      
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
