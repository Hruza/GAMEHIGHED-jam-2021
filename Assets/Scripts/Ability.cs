using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Ability : ScriptableObject
{
    public enum AbilityOutput{abilityFailed, abilityPerformed}

    public abstract void Perform(Vector3Int position,Vector3Int facing,Action<AbilityOutput> AbilityCallback);
}
