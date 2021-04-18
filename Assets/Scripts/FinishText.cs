using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FinishText : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI level;

    public void changeText(string titleText, string levelText)
    {
        title.SetText(titleText);
        level.SetText(levelText);
    }
}
