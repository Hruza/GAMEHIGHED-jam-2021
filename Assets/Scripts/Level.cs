using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Level")]
public class Level : ScriptableObject
{
    public int id;
    public List<PlayerCounts> playerCounts;
    public GameObject grid;
}

[System.Serializable]
public struct PlayerCounts{
    public int count;
    public Ability ability;
}