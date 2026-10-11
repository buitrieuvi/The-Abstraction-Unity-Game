using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "RankSO", menuName = "Installers/RankSO")]
public class RankSO : ScriptableObjectInstaller<RankSO>
{
    public string RankName;
    public Color Color;
    public override void InstallBindings()
    {
    }
}