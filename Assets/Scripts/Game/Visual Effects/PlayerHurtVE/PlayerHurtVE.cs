using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHurtVE : MonoBehaviour
{
    public SpriteRenderer[] spikes;
    public Vector3[] spikeOrigin;
    public SpriteRenderer screen;

    void Awake()
    {
        SetScreenAlpha(0);
        SetSpikesAlpha(0);
        spikeOrigin = new Vector3[spikes.Length];
        for (int i = 0; i < spikes.Length; i++)
        {
            spikeOrigin[i] = spikes[i].transform.localPosition;
        }

        SetSpikesTransform(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            HurtVE();
        }
    }
    
    public void HurtVE()
    {
        StartCoroutine(PlayHurtVE());
        CameraControl.Instance.Shock(transform.position+Vector3.one*10);
    }

    IEnumerator PlayHurtVE()
    {
        float timer = 0;
        while (timer < 0.1f)
        {
            float i = timer/0.1f;
            float j = i*(2-i);

            SetScreenAlpha((1 - j)*0.2f);
            SetSpikesAlpha(0.5f);
            SetSpikesTransform((1-j)*10);
            
            timer += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        SetScreenAlpha(0);
        timer = 0;
        while (timer < 0.1f)
        {
            float i = timer/0.1f;
            float j = i*(2-i);
            
            SetSpikesAlpha(0.5f*(1-j));
            
            timer += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        SetSpikesAlpha(0);
    }

    private void SetSpikesAlpha(float alpha)
    {
        foreach (var spike in spikes)
        {
            spike.color = new Color(spike.color.r, spike.color.g, spike.color.b, alpha);
        }
    }

    private void SetScreenAlpha(float alpha)
    {
        screen.color = new Color(screen.color.r, screen.color.g, screen.color.b, alpha);
    }

    private void SetSpikesTransform(float distance)
    {
        for (int i = 0; i < spikes.Length; i++)
        {
            spikes[i].transform.up =  - (Vector2)spikeOrigin[i];
            spikes[i].transform.localPosition = spikeOrigin[i] + spikes[i].transform.up * -distance;
        }
    }
}
