using UnityEngine;
using Zenject;


[CreateAssetMenu(fileName = "ItemSO", menuName = "Installers/ItemSO/ItemSOFactory")]
public class ItemSOFactory : ItemSO
{

    public override void InstallBindings()
    {
    }

    public override void Run(ControllerBase ct)
    {
        MachineSpawnCubeController machineSpawnCube = ct as MachineSpawnCubeController;
        machineSpawnCube.Time = 1f;
        machineSpawnCube.AutoSpawm = true;
        base.Run(ct);
    }
}