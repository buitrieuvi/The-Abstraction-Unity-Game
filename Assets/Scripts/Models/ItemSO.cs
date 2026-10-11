using System.Collections.Generic;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Installers/ItemSO")]
public class ItemSO : ScriptableObjectInstaller<ItemSO>
{
    public string ItemID;
    public string ItemName;
    public Sprite Avatar;
    public RankSO ItemRank;
    public List<CategorySO> ItemCategorys;

    public override void InstallBindings()
    {
    }
}