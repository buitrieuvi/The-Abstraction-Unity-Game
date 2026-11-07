
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCategoryItemView : InventoryCategoryView
{
    //public int slotIndex;

    public Image AvatarImage;
    public TextMeshProUGUI NumSlot;

    private void Start()
    {
        AvatarImage = GetComponentInChildren<Image>(true);
        NumSlot = GetComponentInChildren<TextMeshProUGUI>();
        //slotIndex = int.Parse(NumSlot.text);
        //CategoryName = slotIndex.ToString();
    }

}
