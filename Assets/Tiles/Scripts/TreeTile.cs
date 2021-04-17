using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeTile : Tile
{    
    public Transform tree;

    public float treeFallTime = 0.5f;
    public GameObject invisibleTile;
    public override void TileAction(Vector3Int from){
        interaction=Interaction.none;
        StartCoroutine(TreeFall(position-from));
    }

    IEnumerator TreeFall(Vector3Int dir){
        type = Grid.TileType.tile;
        Quaternion toRotation = Quaternion.Euler(90*dir.z,0,-90*dir.x); 
        for (float t = 0; t < 1; t += Time.fixedDeltaTime / treeFallTime)
        {
            tree.rotation=Quaternion.Lerp(Quaternion.identity,toRotation,t);
            yield return new WaitForFixedUpdate();
        }
        Grid.instance.AddTile(Instantiate(invisibleTile,position+dir,Quaternion.identity,LevelController.level.transform).GetComponent<Tile>());
        Grid.instance.AddTile(Instantiate(invisibleTile,position+2*dir,Quaternion.identity,LevelController.level.transform).GetComponent<Tile>());
        yield return null;
    }
}
