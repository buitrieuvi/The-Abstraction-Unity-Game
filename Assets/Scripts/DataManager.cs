using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

public sealed class DataManager : IInitializable, IDisposable
{
    public InventoryController PlayerInventory = new();

    private readonly List<ViewBase> views = new();
    private readonly List<RankSO> ranks = new();
    private readonly List<ItemSO> items = new();
    private readonly List<CategorySO> categories = new();

    private readonly List<AsyncOperationHandle> handles = new();
    private bool initialized;
    private bool disposed;

    public IReadOnlyList<ViewBase> Views => views;
    public IReadOnlyList<RankSO> Ranks => ranks;
    public IReadOnlyList<ItemSO> Items => items;
    public IReadOnlyList<CategorySO> Categories => categories;

    public bool IsLoaded { get; private set; }
    public bool AreRanksLoaded { get; private set; }
    public bool AreItemsLoaded { get; private set; }
    public bool AreCategoriesLoaded { get; private set; }

    public Exception LoadError { get; private set; }
    public Exception RankLoadError { get; private set; }
    public Exception ItemLoadError { get; private set; }
    public Exception CategoryLoadError { get; private set; }

    public event Action ViewsLoaded;
    public event Action RanksLoaded;
    public event Action ItemsLoaded;
    public event Action CategoriesLoaded;

    public T GetView<T>() where T : ViewBase => views.Find(view => view is T) as T;
    public ItemSO GetItem(string itemID) => items.Find(item => item != null && item.ItemID == itemID);

    public void Initialize()
    {
        if (initialized || disposed) return;
        initialized = true;

        Load<GameObject>("view", assets =>
        {
            foreach (GameObject prefab in assets)
                if (prefab != null && prefab.TryGetComponent(out ViewBase view))
                    views.Add(view);
            IsLoaded = true;
            ViewsLoaded?.Invoke();
        }, error => LoadError = error);

        Load<RankSO>("rank", assets =>
        {
            ranks.AddRange(assets);
            AreRanksLoaded = true;
            RanksLoaded?.Invoke();
        }, error => RankLoadError = error);

        Load<ItemSO>("item", assets =>
        {
            foreach (ItemSO item in assets)
                if (item != null && GetItem(item.ItemID) == null)
                    items.Add(item);
            AreItemsLoaded = true;
            ItemsLoaded?.Invoke();
        }, error => ItemLoadError = error);

        Load<CategorySO>("category", assets =>
        {
            categories.AddRange(assets);
        }, error => { });


        PlayerInventory.ChangedItem("0", 1);

        PlayerInventory.ChangedItem("1", 2);

        PlayerInventory.ChangedItem("2", 3);
    }
    
    private void Load<T>(string label, Action<IList<T>> onLoaded, Action<Exception> onError)
    {
        if (disposed) return;
        try
        {
            var handle = Addressables.LoadAssetsAsync<T>(label, null, false);
            handles.Add(handle);
            handle.Completed += operation =>
            {
                if (disposed) return;
                if (operation.Status == AsyncOperationStatus.Succeeded)
                {
                    onLoaded(operation.Result);
                    return;
                }

                onError(operation.OperationException);
                handles.Remove(operation);
                Addressables.Release(operation);
            };
        }
        catch (Exception exception)
        {
            onError(exception);
        }
    }

    public void Dispose()
    {
        if (disposed) return;
        disposed = true;

        foreach (AsyncOperationHandle handle in handles)
            if (handle.IsValid())
                Addressables.Release(handle);
        handles.Clear();
        views.Clear();
        ranks.Clear();
        items.Clear();
        categories.Clear();
        IsLoaded = AreRanksLoaded = AreItemsLoaded = AreCategoriesLoaded = false;
        ViewsLoaded = RanksLoaded = ItemsLoaded = CategoriesLoaded = null;

    }
}
