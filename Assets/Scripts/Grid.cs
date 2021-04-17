using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Grid : MonoBehaviour
{
    static public Grid instance;

    private List<Tile> tiles;

    public void AddTile(Tile tile)
    {
        tiles.Add(tile);
        notifyTiles += tile.PlayerOnPosition;
    }

    public void RemoveTile(Tile tile)
    {
        notifyTiles-=tile.PlayerOnPosition;
        tiles.Remove(tile);
        Destroy(tile.gameObject);
    }


    public enum TileType { none, tile, barrier }

    public event Action<Vector3Int> notifyTiles;

    public TileType WhatIsThere(Vector3Int position)
    {
        foreach (Tile tile in tiles)
        {
            if (tile?.position == position)
            {
                return tile.type;
            }
        }
        return TileType.none;
    }

    public Tile GetTile(Vector3Int position){
        foreach (Tile tile in tiles)
        {
            if (tile?.position == position)
            {
                return tile;
            }
        }
        return null;
    }

    public void PlayerIsHere(Vector3Int pos){
        notifyTiles.Invoke(pos);
    }

    private void Awake()
    {
        instance = this;
        tiles = new List<Tile>();
    }
}
