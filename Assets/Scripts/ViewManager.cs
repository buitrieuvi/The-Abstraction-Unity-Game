using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


public sealed class ViewManager : MonoBehaviour
{
    [SerializeField] private RectTransform viewContainer;
    [SerializeField] private Image darkPanel;
    [SerializeField, Min(0.01f)] private float transitionDuration = 1.05f;

    [Inject] private InputManager inputManager { get; set; }
    [Inject] private DataManager dataManager { get; set; }

    [Inject] private DiContainer container;

    public bool IsTransitioning { get; private set; }

    private ViewBase viewBase;

    public ViewBase ViewBase => viewBase;

    private Stack<ViewBase> stack = new();

    public void Awake()
    {
        DOTween.defaultAutoKill = true;
    }

    public async UniTask<ViewBase> ShowView<T>(ControllerBase ctrl) where T : ViewBase
    {
        if (IsTransitioning) return null;
        if (darkPanel != null)
            darkPanel.DOKill();

        bool closeCurrentView = viewBase != null
            && viewBase.GetType() == typeof(T);

        T prefab = closeCurrentView
            ? null
            : dataManager.GetView<T>();

        if (!closeCurrentView && prefab == null)
        {
            Debug.LogWarning(
                $"View prefab {typeof(T).Name} is not loaded.", this);
            return null;
        }

        IsTransitioning = true;

        try
        {
            darkPanel.transform.SetAsLastSibling();
            darkPanel.gameObject.SetActive(true);

            await darkPanel
                .DOFade(1, transitionDuration)
                .From(0)
                .AsyncWaitForCompletion();

            if (viewBase != null)
            {
                viewBase.Close();
                stack.Pop();
                Destroy(viewBase.gameObject);
                viewBase = null;
            }

            if (!closeCurrentView)
            {
                T view = container.InstantiatePrefabForComponent<T>(
                    prefab.gameObject, viewContainer);

                view.Open(ctrl);
                viewBase = view;
                stack.Push(viewBase);
            }

            return viewBase;
        }
        catch (Exception exception)
        {
            Debug.LogException(exception, this);
            return viewBase;
        }
        finally
        {
            inputManager.IsInventoryOpen = viewBase is InventoryView;
            inputManager.SetCursorVisible(viewBase != null);

            if (darkPanel != null)
            {
                await darkPanel
                    .DOFade(0, transitionDuration)
                    .AsyncWaitForCompletion();

                darkPanel.gameObject.SetActive(false);
            }

            IsTransitioning = false;
        }
    }

    //public async UniTask<ViewBase> ShowView<T>(ControllerBase ctrl)
    //where T : ViewBase
    //{
    //    if (IsTransitioning)
    //        return null;

    //    if (stack.Count == 0)
    //    {
    //        IsTransitioning = true;
    //        darkPanel.gameObject.SetActive(true);
    //        await darkPanel
    //            .DOFade(1, transitionDuration)
    //            .From(0)
    //            .AsyncWaitForCompletion();

    //        T prefab = dataManager.GetView<T>();
    //        T view = container.InstantiatePrefabForComponent<T>(
    //            prefab.gameObject, viewContainer);
    //        view.Open(ctrl);
    //        viewBase = view;
    //        stack.Push(viewBase);

    //        await darkPanel
    //        .DOFade(0, transitionDuration)
    //        .AsyncWaitForCompletion();
    //        darkPanel.gameObject.SetActive(false);
    //        IsTransitioning = false;


    //    }
    //    else 
    //    {
    //        if (typeof(MenuView).IsAssignableFrom(typeof(T)) || typeof(T) == viewBase.GetType()) 
    //        {

    //            IsTransitioning = true;
    //            darkPanel.gameObject.SetActive(true);
    //            await darkPanel
    //            .DOFade(1, transitionDuration)
    //            .From(0)
    //            .AsyncWaitForCompletion();

    //            stack.Pop();
    //            viewBase.Close();
    //            Destroy(viewBase.gameObject);
    //            viewBase = null;

    //            await darkPanel
    //            .DOFade(0, transitionDuration)
    //            .From(1)
    //            .AsyncWaitForCompletion();
    //            darkPanel.gameObject.SetActive(false);
    //            IsTransitioning = false;
    //        }
    //    }

    //    return viewBase;
    //}
}
