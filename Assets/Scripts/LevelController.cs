using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class LevelController : MonoBehaviour
{
    static public LevelController instance;
    public List<Level> levels;
    public GameObject wholeMenu;
    public GameObject menuPanel;
    public GameObject choosePanel;    
    public GameObject finishPanel;
    public GameObject failPanel;
    public GameObject creditsPanel;
    public RectTransform chooseSubPanel;
    public GameObject pauseMenu;
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

    public List<PlayerObjectPrefabs> playerPrefabs;

    private GameObject[] levelTiles;

    private PlayerControls input;

    private bool showingPauseMenu;

    [System.Serializable]
    public struct PlayerObjectPrefabs
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
        failPanel.SetActive(false);
        creditsPanel.SetActive(false);
        pauseMenu.SetActive(false);
        showingPauseMenu=false;
    }

    
    void Start()
    {
        input=new PlayerControls();
        input.Menu.Back.performed+= ctx => Back();    
        input.Enable();

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

    public void Back(){
        if(state==State.inGame || state == State.choosing){
            showingPauseMenu = !showingPauseMenu;
            pauseMenu.SetActive(showingPauseMenu);
        }
        else if(state==State.inMenu){
            ReturnToMenuPressed();
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
                button.GetComponent<Tooltip>().text=ability.ability.tooltip;
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

        foreach (PlayerObjectPrefabs playerPrefab in playerPrefabs)
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

    public void ClearAndRetryLevelPressed(bool restart)
    {
        Destroy(level);
        Destroy(player);
        HideAllPanels();
        wholeMenu.SetActive(true);
        Cursor.visible=true;
        if(restart)
            RetryLevelPressed();
        else
            ReturnToMenuPressed();
    }

    public void RetryLevelPressed()
    {
        SetLevel(currentLevel.id);
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
        state=State.inMenu;
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
        
        if(stars==0){
            failPanel.SetActive(true);
            AudioManager.Play("Lost");            
            failPanel.GetComponent<FinishText>().changeText("LEVEL " + (currentLevel.id + 1).ToString());
            failPanel.GetComponent<LevelTile>().SetStarCount(stars);
        }
        else{
            finishPanel.SetActive(true);
            AudioManager.Play("Victory");
            finishPanel.GetComponent<FinishText>().changeText("LEVEL " + (currentLevel.id + 1).ToString());
            finishPanel.GetComponent<LevelTile>().SetStarCount(stars);
        }
        
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
        int starCount = Mathf.Clamp(3 - (currentLevel.goal - totalcount), 1,3);
        progress[currentLevel.id] = Mathf.Max(starCount, Mathf.Min(progress[currentLevel.id],3));;
        SaveProgress();
        UpdateLevels();
        LevelEnd(starCount);
    }

/*
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
    */

    public int[] LoadProgress(string filePath = "saves/progress.dat")
    {
        int[] result = new int[levels.Count];
        for (var i = 0; i < levels.Count; i++)
        {
            if(PlayerPrefs.HasKey("lvl"+i.ToString()))
            result[i] = PlayerPrefs.GetInt("lvl"+i.ToString());
        }
        return result;
    }

    public void SaveProgress()
    {
        for (var i = 0; i < levels.Count; i++)
        {
            PlayerPrefs.SetInt("lvl"+i.ToString(),progress[i]);
        }
    }
}