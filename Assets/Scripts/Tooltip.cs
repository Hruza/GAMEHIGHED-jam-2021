using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public string text;
    public GameObject tooltip;

    private GameObject localTooltip;
    public void OnMouseEnter() {
        localTooltip=Instantiate(tooltip, transform);
        localTooltip.GetComponentInChildren<TextMeshProUGUI>().SetText(text);
    }
    public void OnMouseExit() {
        if(localTooltip!=null)
            Destroy(localTooltip);
    }
}
