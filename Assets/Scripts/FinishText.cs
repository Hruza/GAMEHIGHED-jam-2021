using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FinishText : MonoBehaviour
{
    public TextMeshProUGUI level;

    public void changeText(string levelText)
    {
        level.SetText(levelText);
    }
}
