using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class LevelController : MonoBehaviour
{
    static public LevelController instance;
    public List<Level> levels;
    public GameObject wholeMenu;
    public GameObject menuPanel;
    public GameObject choosePanel;    
    public GameObject finishPanel;
    public GameObject creditsPanel;
    public RectTransform chooseSubPanel;
    public GameObject levelPanel;
    public GameObject levelScrollBarContent;
    public GameObject levelButton;
    public GameObject abilityButton;
    static public GameObject player;
    static public GameObject level;

    private enum State { inMenu, choosing, inGame }
    private List<PlayerCounts> abilityPool;
    private State state;
    private int progressID;
    private Level currentLevel;
    private int[] progress;

    public List<PlayerPrefabs> playerPrefabs;

    private GameObject[] levelTiles;

    [System.Serializable]
    public struct PlayerPrefabs
    {
        public GameObject player;
        public Ability ability;
    }

    void HideAllPanels()
    {
        menuPanel.SetActive(false);
        levelPanel.SetActive(false);
        choosePanel.SetActive(false);
        finishPanel.SetActive(false);
    }
    void Start()
    {
        instance = this;
        progress = LoadProgress();
        levelTiles = new GameObject[levels.Count];
        foreach (Level level in levels)
        {
            GameObject button = Instantiate(levelButton, levelScrollBarContent.GetComponent<RectTransform>());
            button.GetComponentInChildren<TextMeshProUGUI>().SetText("Level " + (level.id + 1).ToString());
            button.GetComponent<Button>().onClick.AddListener(delegate { SetLevel(level.id); });
            levelTiles[level.id] = button;
        }
        UpdateLevels();
    }

    private void UpdateLevels()
    {
        for (var i = 0; i < levels.Count; i++)
        {
            levelTiles[i].GetComponent<LevelTile>().SetStarCount(progress[i]);
            levelTiles[i].GetComponent<Button>().interactable = (i > 0) ? (progress[i - 1] > 0) : true;
        }
    }

    void SetLevel(int id)
    {
        Debug.Log("Starting level " + id.ToString());

        currentLevel = levels.Find(x => x.id == id);
        abilityPool = new List<PlayerCounts>(currentLevel.playerCounts);
        level = Instantiate(currentLevel.grid, Vector3.zero, Quaternion.identity);
        CameraFollow.instance.target = level.transform;
        DrawAbilities();
    }

    void DrawAbilities()
    {
        HideAllPanels();
        wholeMenu.SetActive(false);
        choosePanel.SetActive(true);

        state = State.choosing;
        Cursor.visible = true;

        foreach (Transform child in chooseSubPanel)
        {
            Destroy(child.gameObject);
        }
        int totalcount = 0;
        foreach (PlayerCounts ability in abilityPool)
        {
            if (ability.count > 0)
            {
                GameObject button = Instantiate(abilityButton, chooseSubPanel);
                button.GetComponentInChildren<AbilityButton>().SetAbility(ability);
                button.GetComponent<Button>().onClick.AddListener(delegate { SelectAbility(ability.ability); });
                if(currentLevel.id==0 && totalcount>0 && ability.ability.name=="NoAbility") button.GetComponent<Button>().interactable=false;
                totalcount += ability.count;
            }
        }
        if (totalcount == 0)
        {
            LevelEnd(0);
        }
    }

    void SelectAbility(Ability ability)
    {
        state = State.inGame;

        GameObject playerObject = null;

        foreach (PlayerPrefabs playerPrefab in playerPrefabs)
        {
            if (playerPrefab.ability == ability)
            {
                playerObject = playerPrefab.player;
            }
        }

        player = Instantiate(playerObject, Vector3.zero, Quaternion.identity);
        player.GetComponent<Player>().ability = ability;
        player.GetComponent<Player>().PlayerDiedCallback += PlayerDied;
        CameraFollow.instance.target = player.transform;
        for (int i = 0; i < abilityPool.Count; i++)
        {
            if (abilityPool[i].ability == ability)
            {
                abilityPool[i] = new PlayerCounts { ability = ability, count = abilityPool[i].count - 1 };
            }
        }
        choosePanel.SetActive(false);
        Cursor.visible = false;
    }

    public void StartPressed()
    {
        HideAllPanels();
        levelPanel.SetActive(true);
    }

    public void ExitPressed()
    {
        Application.Quit();
    }

    public void CreditsPressed()
    {
        HideAllPanels();
        creditsPanel.SetActive(true);
    }

    public void ReturnToMenuPressed()
    {
        HideAllPanels();
        menuPanel.SetActive(true);
    }
    public void PlayerDied()
    {
        DrawAbilities();
    }

    public void LevelEnd(int stars)
    {
        Destroy(level);
        HideAllPanels();
        wholeMenu.SetActive(true);
        finishPanel.SetActive(true);
        string victory = (stars > 0 ) ? "VICTORY" : "DEFEAT";
        if(stars==0){
            AudioManager.Play("Lost");
        }
        else{
            AudioManager.Play("Victory");
        }
        finishPanel.GetComponent<FinishText>().changeText(victory, "LEVEL " + (currentLevel.id + 1).ToString());
        finishPanel.GetComponent<LevelTile>().SetStarCount(stars);
        if (player != null)
        {
            Destroy(player);
        }
        Cursor.visible = true;
        state = State.inMenu;
    }

    public void FinishOKPressed()
    {
        HideAllPanels();
        levelPanel.SetActive(true);
    }

    public void Win()
    {
        int totalcount = 0;
        foreach (PlayerCounts ability in abilityPool)
        {
            totalcount += ability.count;
        }
        int starCount = Mathf.Max(Mathf.Max(3 - (currentLevel.goal - totalcount), 1), progress[currentLevel.id]);
        progress[currentLevel.id] = starCount;
        SaveProgress();
        UpdateLevels();
        LevelEnd(starCount);
    }

    public int[] LoadProgress(string filePath = "saves/progress.dat")
    {
        int[] result = new int[levels.Count];
        if (File.Exists(filePath))
        {
            FileStream fs = new FileStream(filePath, FileMode.Open);

            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                int[] saved = (int[])bf.Deserialize(fs);
                for (var i = 0; i < levels.Count; i++)
                {
                    result[i] = saved[i];
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            finally
            {
                fs.Close();
            }
        }
        return result;
    }

    public void SaveProgress()
    {
        if (!Directory.Exists("saves")) Directory.CreateDirectory("saves");
        FileStream fs = new FileStream("saves/progress.dat", FileMode.Create);

        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, progress);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        finally
        {
            fs.Close();
        }
    }
}