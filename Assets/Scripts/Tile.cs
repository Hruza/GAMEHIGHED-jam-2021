using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector3Int position;

    public enum Interaction{none, tree, destroyable}

    public bool canBuildOn=true;

    public Interaction interaction;
    public Grid.TileType type = Grid.TileType.tile;

    void Awake()
    {
        position = new Vector3Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Mathf.RoundToInt(transform.position.z));
    }

    public virtual void TileAction(Vector3Int from){

    }
    
    private void Start() {
        Grid.instance.AddTile(this);    
    }
    public virtual void PlayerOnPosition(Vector3Int pos)
    {
        return;
    }

    private void OnDisable() {
        Grid.instance.RemoveTile(this,false);
    }
}
