using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NullAbility", menuName = "ScriptableObjects/Abilities/NullAbility")]
public class Ability : ScriptableObject
{
    public Sprite sprite;
    public enum AbilityOutput{abilityFailed, abilityPerformed}

    public virtual void Perform(Vector3Int position,Vector3Int facing,Action<AbilityOutput> AbilityCallback){
        AbilityCallback(AbilityOutput.abilityFailed);
    }
}
