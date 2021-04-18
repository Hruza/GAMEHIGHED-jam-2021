using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTile : Tile
{    
    public GameObject wall;

    public ParticleSystem particles;

    public Tile invisible;

    public override void TileAction(Vector3Int from){
        interaction=Interaction.none;
        wall.SetActive(false);
        type = Grid.TileType.tile;
        particles.Play();
    }
}
