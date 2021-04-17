using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragileTile : Tile
{
    bool activated=false;

    public GameObject breakParticles;

    public override void PlayerOnPosition(Vector3Int pos)
    {
        if (pos == position){
            activated=true;
        }
        else if(activated){
            Grid.instance.RemoveTile(this);
            Destroy(Instantiate(breakParticles,transform.position,transform.rotation,LevelController.level.transform),3);
        }
    }
}
