using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelController : MonoBehaviour
{
    public List<Level> levels;
    public GameObject wholeMenu;    
    public GameObject menuPanel;
    public GameObject choosePanel;
    public GameObject levelPanel;
    public GameObject levelScrollBarContent;
    public GameObject levelButton;
    public GameObject abilityButton;
    public GameObject playerObject;
    static public GameObject player;
    static public GameObject level;   

    private enum State { inMenu, choosing, inGame }
    private List<PlayerCounts> abilityPool;
    private State state;
    private int progressID;
    private Level currentLevel;
    
    void HideAllPanels(){
        menuPanel.SetActive(false);
        levelPanel.SetActive(false);
        choosePanel.SetActive(false);
    }
    void Start()
    {
        if (PlayerPrefs.HasKey("progress"))
            PlayerPrefs.GetInt("progress");
        else
            progressID = 0;

        foreach (Level level in levels)
        {
            GameObject button = Instantiate(levelButton, levelScrollBarContent.GetComponent<RectTransform>());
            button.GetComponentInChildren<TextMeshProUGUI>().SetText("Level " + level.id.ToString());
            button.GetComponent<Button>().onClick.AddListener(delegate { SetLevel(level.id); });
        }
    }

    void SetLevel(int id)
    {
        Debug.Log("Starting level " + id.ToString());

        currentLevel = levels.Find(x => x.id == id);
        abilityPool = new List<PlayerCounts>(currentLevel.playerCounts);
        level = Instantiate(currentLevel.grid, Vector3.zero, Quaternion.identity);
        DrawAbilities();
    }

    void DrawAbilities()
    {       
        HideAllPanels();
        wholeMenu.SetActive(false);
        choosePanel.SetActive(true);

        state = State.choosing;
        Cursor.visible = true;

        foreach (Transform child in choosePanel.transform)
        {
            Destroy(child.gameObject);
        }
        int totalcount = 0;
        foreach (PlayerCounts ability in abilityPool)
        {
            if (ability.count > 0)
            {
                GameObject button = Instantiate(abilityButton, choosePanel.transform);
                button.GetComponentInChildren<TextMeshProUGUI>().SetText(ability.count.ToString());
                button.GetComponent<Button>().onClick.AddListener(delegate { SelectAbility(ability.ability); });
                totalcount += ability.count;
            }
        }
        if (totalcount == 0)
        {
            PlayerLost();
        }
    }

    void SelectAbility(Ability ability)
    {
        state = State.inGame;

        //TODO Varianty

        player = Instantiate(playerObject, Vector3.zero, Quaternion.identity);
        player.GetComponent<Player>().ability = ability;
        player.GetComponent<Player>().PlayerDiedCallback += PlayerDied;
        CameraFollow.instance.target=player.transform;
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
        Debug.LogError("NOT IMPLEMENTED CREDITS YET");
    }

    public void PlayerDied()
    {
        DrawAbilities();
    }

    public void PlayerLost()
    {
        Destroy(level);
        HideAllPanels();
        wholeMenu.SetActive(true);
        levelPanel.SetActive(true);

        state = State.inMenu;
    }
}