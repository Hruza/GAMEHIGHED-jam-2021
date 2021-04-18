using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour
{
    public TextMeshProUGUI counter;  
    public Image image;

    public void SetAbility(PlayerCounts ability){
        image.sprite=ability.ability.sprite;
        counter.SetText(ability.count.ToString());
    }
}
