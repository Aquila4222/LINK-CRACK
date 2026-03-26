using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPlayer : MonoBehaviour
{
    private Camera Camera;

    private float lightTimer;

    private float rainTimer;
    void Awake()
    {
        Camera = GetComponent<Camera>();
    }

    private void SpawnLight()
    {
        Color color;
        float c = Random.Range(-1f, 1f);
        if (c >= 0)
        {
            color = new Color(0, c, 1, 1);
        }
        else
        {
            color = new Color(-c, 0, 1, 1);
        }

        Vector3 LocalPos = transform.position+ new Vector3(Random.Range(-8f / 9 * 16, 8f / 9 * 16), Random.Range(-8, 8), 1);

        BGLightsPool.Instance.Play(LocalPos,Random.Range(2f,3f),Random.onUnitSphere.normalized*Random.Range(0f,5f),color,transform);
    }

    private void SpawnRain()
    {
        Vector3 LocalPos = transform.position+ new Vector3(Random.Range(-8f / 9 * 16, 8f / 9 * 16), 20, 1);
        Vector2 direction = new Vector2(-1,-10)+Random.insideUnitCircle.normalized*0.2f;
        Vector3 scale = new Vector3(0.05f*Random.Range(0f,1f),20f,1f);
        ParticleVEPool.Instance.Play((Vector2)LocalPos,0.2f,500,direction,scale,scale,new Color(1,1,1,0.1f));
    }

    void Update()
    {
        if (lightTimer > 0)
        {
            lightTimer -= Time.deltaTime;
        }
        else
        {
            SpawnLight();
            lightTimer = 0.2f;
        }

        // if (rainTimer > 0)
        // {
        //     rainTimer -= Time.deltaTime;
        // }
        // else
        // {
        //     rainTimer = 0.01f;
        //     SpawnRain();
        // }
    }
}
