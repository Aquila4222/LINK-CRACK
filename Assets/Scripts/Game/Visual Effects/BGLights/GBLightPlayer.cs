using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GBLightPlayer : MonoBehaviour
{
    private Camera Camera;

    private float timer;
    void Awake()
    {
        Camera = GetComponent<Camera>();
    }

    private void Spawn()
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

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            Spawn();
            timer = 0.2f;
        }
    }
}
