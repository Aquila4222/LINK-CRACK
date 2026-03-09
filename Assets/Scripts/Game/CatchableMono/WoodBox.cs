using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodBox : CatchableMono
{
    protected override void OnCrash()
    {
        base.OnCrash();
        Debug.Log(gameObject.name + ": Crashed!");
        RingVEPool.Instance.Play(transform.position,0.15f,Color.white,0.8f,4f);
    }
}
