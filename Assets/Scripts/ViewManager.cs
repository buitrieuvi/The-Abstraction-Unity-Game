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

    private ViewBase viewBaseCurr;
    private Stack<ViewBase> stack = new();

    bool isClose = false;

    public void Awake()
    {
        DOTween.defaultAutoKill = true;
    }

    public async UniTask<ViewBase> ShowView<T>(ControllerBase ctrl) where T : ViewBase
    {
        if (IsTransitioning) return null;

        if (darkPanel != null)
            darkPanel.DOKill();

        if (viewBaseCurr != null)
        {
            if (viewBaseCurr.GetType() == typeof(T))
            {
                isClose = true;
            }
            else 
            {
                if (typeof(MenuView).IsAssignableFrom(typeof(T)))
                {
                    isClose = true;
                }
                else 
                {
                    return null;
                }
            }
        }
        else 
        {
            isClose = false;
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

            if (isClose)
            {
                viewBaseCurr.Close();
                stack.Pop();
                Destroy(viewBaseCurr.gameObject);
                viewBaseCurr = null;
                isClose = false;
            }
            else 
            {
                T prefab = dataManager.GetView<T>();

                T view = container.InstantiatePrefabForComponent<T>(prefab.gameObject, viewContainer);

                view.Open(ctrl);
                viewBaseCurr = view;
                stack.Push(viewBaseCurr);
            }
            return viewBaseCurr;
        }
        catch (Exception exception)
        {
            Debug.LogException(exception, this);
            return viewBaseCurr;
        }
        finally
        {
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
}
