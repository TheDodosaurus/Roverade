using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubbleItemScript : ItemScript
{
    public int removeCost = 100;

    private void Awake()
    {
    }

    public override void Initialise(Vector3Int pos)
    {
        
        base.ItemInitialise(
            pos,
            new ItemAction[] {
                new ItemAction("Destroy", "Destroy the rubble.", removeCost, true, MDestroy)
            },
            false
        );
    }

    // M: initial callback
    // R: callback after rover reaches item
    public void MDestroy()
    {
        GameManager.gm.CallRover(pos, 1f, RDestroy);
        
    }

    public void RDestroy()
    {
        Destroy(gameObject);
    }
}
