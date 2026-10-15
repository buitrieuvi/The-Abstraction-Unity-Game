using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Cysharp.Threading.Tasks;


public sealed class ViewManager : MonoBehaviour
{
    [SerializeField] private RectTransform viewContainer;
    [SerializeField] private Image darkPanel;
    [SerializeField, Min(0.01f)] private float transitionDuration = 0.55f;

    [Inject] private InputManager inputManager { get; set; }
    [Inject] private DataManager dataManager { get; set; }

    [Inject] private DiContainer container;

    public bool IsTransitioning { get; private set; }

    private ViewBase viewBase;

    public ViewBase ViewBase => viewBase;

    public void Awake()
    {
        DOTween.defaultAutoKill = true;
    }

    public async UniTask<ViewBase> ShowView<T>(ControllerBase ctrl)
    where T : ViewBase
    {
        if (IsTransitioning)
            return null;

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
                Destroy(viewBase.gameObject);
                viewBase = null;
            }

            if (!closeCurrentView)
            {
                T view = container.InstantiatePrefabForComponent<T>(
                    prefab.gameObject, viewContainer);

                view.Open(ctrl);
                viewBase = view;
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

    //public void LoadView<T>(ControllerBase ctrl) where T : ViewBase
    //{
    //    if (IsTransitioning) return;
    //    if (darkPanel != null) darkPanel.DOKill();
    //    //if (ctrl == null)
    //    //{
    //    //    return;
    //    //}

    //    bool closeCurrentView = viewBase != null
    //        && viewBase.GetType() == typeof(T);
    //    T prefab = closeCurrentView ? null : dataManager.GetView<T>();
    //    if (!closeCurrentView && prefab == null)
    //    {
    //        Debug.LogWarning($"View prefab {typeof(T).Name} is not loaded.", this);
    //        return;
    //    }

    //    IsTransitioning = true;

    //    darkPanel.transform.SetAsLastSibling();
    //    darkPanel.gameObject.SetActive(true);
    //    darkPanel.DOFade(1, transitionDuration).From(0).OnComplete(() => 
    //    {
    //        T view = null;
    //        try
    //        {
    //            if (viewBase != null)
    //            {
    //                viewBase.Close();
    //                Destroy(viewBase.gameObject);
    //                viewBase = null;
    //            }

    //            if (!closeCurrentView)
    //            {

    //                view = container.InstantiatePrefabForComponent<T>(prefab.gameObject, viewContainer);
    //                view.Open(ctrl);
    //                viewBase = view;
    //            }
    //        }
    //        catch (Exception exception)
    //        {
    //            if (view != null)
    //            {
    //                try { view.Close(); }
    //                finally { Destroy(view.gameObject); }
    //            }
    //            Debug.LogException(exception, this);
    //        }
    //        finally
    //        {
    //            inputManager.IsInventoryOpen = viewBase is InventoryView;
    //            inputManager.SetCursorVisible(viewBase != null);
    //            darkPanel.DOFade(0, transitionDuration).OnComplete(() =>
    //            {
    //                IsTransitioning = false;
    //                darkPanel.gameObject.SetActive(false);
    //            });
    //        }
    //    });
    //}

    public void OnlyClose() 
    {
        if (IsTransitioning) return;
        if (viewBase == null) return;
        IsTransitioning = true;
        darkPanel.transform.SetAsLastSibling();
        darkPanel.gameObject.SetActive(true);
        darkPanel.DOFade(1, transitionDuration).From(0).OnComplete(() =>
        {
            try
            {
                viewBase.Close();
                Destroy(viewBase.gameObject);
                viewBase = null;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
            }
            finally
            {
                inputManager.IsInventoryOpen = false;
                inputManager.SetCursorVisible(false);
                darkPanel.DOFade(0, transitionDuration).OnComplete(() =>
                {
                    IsTransitioning = false;
                    darkPanel.gameObject.SetActive(false);
                });
            }
        });
    }

    public ViewBase GetView<T>() where T : ViewBase
    {
        return dataManager.GetView<T>();
    }


}
