using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RomoteEnemyBarragePool : ObjectPoolTemplate<RomoteEnemyBarragePool>
{

    public override void Awake()
    {
        objectPrefab = Resources.Load<GameObject>("Prefabs/Box");
        warmCount = 100;
        poolSize = 100;
        
        base.Awake();
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
