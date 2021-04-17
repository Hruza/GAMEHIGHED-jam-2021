using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishTile : Tile
{
    public override void PlayerOnPosition(Vector3Int pos)
    {
        if(pos==position){
            LevelController.instance.Win();
        }
    }
}
