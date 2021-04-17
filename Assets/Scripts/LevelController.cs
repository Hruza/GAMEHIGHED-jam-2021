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

    public GameObject menu;

    public GameObject choosePanel;

    public GameObject levelButton;

    void Start()
    {
        if (PlayerPrefs.HasKey("progress"))
            PlayerPrefs.GetInt("progress");
        else
            progressID = 0;

        foreach (Level level in levels)
        {
            GameObject button = Instantiate(levelButton, levelPanel);
            button.GetComponentInChildren<TextMeshProUGUI>().SetText(level.id.ToString());
            button.GetComponent<Button>().onClick.AddListener(delegate { SetLevel(level.id); });
        }
    }

    private enum State { inMenu, choosing, inGame }
    private State state;

    Level currentLevel;

    void SetLevel(int id)
    {
        Debug.Log("Starting level " + id.ToString());
        menu.SetActive(false);
        currentLevel = levels.Find(x => x.id == id);
        abilityPool = new List<PlayerCounts>(currentLevel.playerCounts);
        level = Instantiate(currentLevel.grid, Vector3.zero, Quaternion.identity);
        DrawAbilities();
    }

    public GameObject abilityButton;
    public GameObject playerObject;

    void DrawAbilities()
    {
        state = State.choosing;
        Cursor.visible = true;
        choosePanel.SetActive(true);
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

    static public GameObject player;

    static public GameObject level;

    void SelectAbility(Ability ability)
    {
        state = State.inGame;
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

    public void PlayerDied()
    {
        DrawAbilities();
    }

    public void PlayerLost()
    {
        Destroy(level);
        menu.SetActive(true);
        state = State.inMenu;
    }

    private List<PlayerCounts> abilityPool;
}