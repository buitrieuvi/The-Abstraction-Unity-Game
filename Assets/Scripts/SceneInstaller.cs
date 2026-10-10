using Zenject;

public class SceneInstaller : MonoInstaller<SceneInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<PlayerController>()
            .FromComponentInHierarchy()
            .AsSingle()
            .NonLazy();
    }
}