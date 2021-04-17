using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockAbility", menuName = "ScriptableObjects/Abilities/BlockAbility")]
public class BlockAbility : Ability
{
    public GameObject tile;
    public override void Perform(Vector3Int position, Vector3Int facing, Action<AbilityOutput> AbilityCallback)
    {
        Tile target = Grid.instance.GetTile(position);
        if (target != null && target.type == Grid.TileType.tile)
        {
            Grid.instance.AddTile(Instantiate(tile,position + Vector3Int.up,Quaternion.identity,LevelController.level.transform).GetComponent<Tile>());
            AbilityCallback(AbilityOutput.abilityPerformed);
        }
        else{
            AbilityCallback(AbilityOutput.abilityFailed);
        }
    }
}
