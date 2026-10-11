using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "TagSO", menuName = "Installers/TagSO")]
public class TagSO : ScriptableObjectInstaller<TagSO>
{
    public string TagId;
    public string TagName;
    public string TagNameVn;

    public override void InstallBindings()
    {
    }
}