using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "CategorySO", menuName = "Installers/CategorySO")]
public class CategorySO : ScriptableObjectInstaller<CategorySO>
{
    public string CategoryID;
    public string CategoryName;

    public override void InstallBindings()
    {
    }
}