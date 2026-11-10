using Zenject;
using UnityEngine;

public sealed class ProjectInstaller : MonoInstaller
{
    [SerializeField] private LayerManager layerController;
    [SerializeField] private ViewManager viewManagerPrefab;
    public override void InstallBindings()
    {
        Application.targetFrameRate = 60;

        Screen.SetResolution(1920, 1080, false);
        Screen.fullScreenMode = FullScreenMode.Windowed;
        Screen.SetMSAASamples(8);

        QualitySettings.antiAliasing = 8;

        Container.Bind<LayerManager>().FromInstance(layerController).AsSingle();


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
