using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WarningLinePool : ObjectPoolTemplate<WarningLinePool>
{
    public new void Awake()
    {
        warmCount = 5;
        poolSize = 30;
        objectPrefab = Resources.Load<GameObject>("Prefabs/WarningLine");
        
        base.Awake();
    }

    public void ShowWarningLine(Vector3 position,Vector3 direction,float time)
    {
        WarningLine line = GetObject().GetComponent<WarningLine>();
        if (line != null)
        {
            line.ShowWarningLine(position, direction,time);
        }
    }

   
}
