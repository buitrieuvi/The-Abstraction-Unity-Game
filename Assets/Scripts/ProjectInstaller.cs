using Zenject;
using UnityEngine;

public sealed class ProjectInstaller : MonoInstaller
{
    [SerializeField] private LayerController layerController;
    [SerializeField] private ViewManager viewManagerPrefab;
    public override void InstallBindings()
    {
        Container.Bind<LayerController>().FromInstance(layerController).AsSingle();


        Container.BindInterfacesAndSelfTo<DataManager>()
            .AsSingle()
            .NonLazy();

        Container.BindInterfacesAndSelfTo<InputManager>()
            .AsSingle()
            .NonLazy();

        Container.Bind<ViewManager>()
            .FromComponentInNewPrefab(viewManagerPrefab)
            .UnderTransform(transform)
            .AsSingle()
            .NonLazy();
    }
}
