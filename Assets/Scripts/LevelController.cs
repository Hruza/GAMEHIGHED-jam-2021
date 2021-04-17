using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelController : MonoBehaviour
{
    public List<Level> levels;

    private int progressID;

    public RectTransform levelPanel;

    public GameObject levelButton;

    void Start()
    {
        if(PlayerPrefs.HasKey("progress"))
            PlayerPrefs.GetInt("progress");
        else 
            progressID=0;
        
        foreach (Level level in levels)
        {
            GameObject button = Instantiate(levelButton,levelPanel);
            button.GetComponentInChildren<TextMeshProUGUI>().SetText(level.id.ToString());
            button.GetComponent<Button>().onClick.AddListener(delegate{SetLevel(level.id);});
        }
    }

    void SetLevel(int id){
        Debug.Log("Starting level "+id.ToString());

    }
}
