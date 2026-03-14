using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : MonoBehaviour,IHurt
{
    public int MaxBlood;
    
    private int blood;

    void Awake()
    {
        blood = MaxBlood;
    }
    
    public void Hurt(Vector2 repulseForce, float damage = 1)
    {
        TakeDamage();
        
    }

    private void TakeDamage()
    {
        for (int i = 0; i < 10; i++)
        {
            ParticleVEPool.Instance.Play(transform.position,0.2f*Random.Range(1,1.5f),10*Random.Range(1,1.5f),Random.onUnitSphere.normalized,new Vector3(0.01f,0.1f,0.1f),new Vector3(0.5f,0.5f,0.5f),Color.white,true);
        }
        if (blood > 1)
        {
            blood--;
        }
        else
        {
            GameController.Instance.Restart();
            gameObject.SetActive(false);
        }
    }
}
