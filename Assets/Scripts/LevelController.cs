using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    public List<Level> levels;

    private int progressID;

    void Start()
    {
        if(PlayerPrefs.HasKey("progress"))
            PlayerPrefs.GetInt("progress");
        else 
            progressID=0;
    }
}
