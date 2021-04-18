using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerTile : Tile
{
    public UnityEvent onStepTrigger;

    public override void PlayerOnPosition(Vector3Int pos)
    {
        if (pos == position){
            onStepTrigger.Invoke();
        }
    }
}
