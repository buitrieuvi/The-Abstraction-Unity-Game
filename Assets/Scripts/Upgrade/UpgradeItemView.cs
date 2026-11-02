
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeItemView : ViewBase
{
    public int slotIndex;

    private void Start()
    {
        slotIndex = int.Parse(GetComponentInChildren<TextMeshProUGUI>().text);
    }

}
