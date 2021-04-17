using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTile : MonoBehaviour
{
    public RectTransform starPanel;  
    public GameObject starPrefab;

    public void SetStarCount(int count){
        foreach(Transform child in starPanel){
            Destroy(child.gameObject);
        }
        for (var i = 0; i < count; i++)
        {
            Instantiate(starPrefab,starPanel);
        }
    }
}
