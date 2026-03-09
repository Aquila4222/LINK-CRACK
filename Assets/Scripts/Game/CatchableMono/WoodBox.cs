using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodBox : CatchableMono
{
    protected override void OnCrash()
    {
        base.OnCrash();
        Debug.Log(gameObject.name + ": Crashed!");
    }
}
