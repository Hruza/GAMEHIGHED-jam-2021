using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TreeAbility", menuName = "ScriptableObjects/Abilities/TreeAbility")]
public class TreeAbility : Ability
{
    public override void Perform(Vector3Int position, Vector3Int facing, Action<AbilityOutput> AbilityCallback)
    {
        Tile target = Grid.instance.GetTile(position+facing);
        if(target!=null && target.interaction == Tile.Interaction.tree){
            target.TileAction(position);
            AbilityCallback(AbilityOutput.abilityPerformed);
        }
        AbilityCallback(AbilityOutput.abilityFailed);
    }
}
