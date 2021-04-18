using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Events;

public class TriggerTile : Tile
{
    public UnityEvent onStepTrigger;

    public Transform plate;

    public override void PlayerOnPosition(Vector3Int pos)
    {
        if (pos == position)
        {
            onStepTrigger.Invoke();
            if(plate != null) StartCoroutine(plateDrop()); 
        }
    }

    IEnumerator plateDrop()
    {
        Vector3 pos = plate.position;
        for (float t = 0; t < 0.5; t += Time.fixedDeltaTime)
        {
            plate.position = pos + 0.5f*((t * (0.5f - t)) * Vector3.down);
            yield return new WaitForFixedUpdate();
        }
        plate.position = pos;
        yield return null;
    }
}
