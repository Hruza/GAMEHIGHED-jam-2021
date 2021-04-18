using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WallAbility", menuName = "ScriptableObjects/Abilities/WallAbility")]
public class WallAbility : Ability
{
    public override void Perform(Vector3Int position, Vector3Int facing, Action<AbilityOutput> AbilityCallback)
    {
        Tile target = Grid.instance.GetTile(position+facing);
        if(target!=null && target.interaction == Tile.Interaction.destroyable){
            target.TileAction(position);
            AbilityCallback(AbilityOutput.abilityPerformed);
        }
        else
            AbilityCallback(AbilityOutput.abilityFailed);
    }
}
