using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Zenject;

public sealed class PlayerHand : MonoBehaviour
{
    [Inject] InputManager inputManager;

    [Header("References")]
    [SerializeField] private GameObject GameObjectPrefab;
    [SerializeField] private Transform StartPoint;

    [Header("Pooling")]
    [SerializeField] private int poolSize = 10;

    [Header("Projectile")]
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float moveDistance = 10f;

    private readonly Queue<GameObject> pool = new();

    private void Awake()
    {
        inputManager.InputActions.Player.Attack.performed += _ => Shoot();
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(GameObjectPrefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public void Shoot()
    {
        if (pool.Count == 0)
            return;

        GameObject projectile = pool.Dequeue();

        // Lấy hướng trước mặt StartPoint
        Vector3 direction = StartPoint.forward.normalized;

        // Thiết lập vị trí và rotation
        projectile.transform.SetPositionAndRotation(
            StartPoint.position,
            Quaternion.LookRotation(direction)
        );

        projectile.SetActive(true);

        // Tính khoảng cách và thời gian di chuyển
        Vector3 targetPosition =
            StartPoint.position + direction * moveDistance;

        float duration = moveDistance / moveSpeed;

        // Di chuyển bằng DOTween
        projectile.transform.DOMove(targetPosition, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                ReturnToPool(projectile);
            });
    }

    private void ReturnToPool(GameObject obj)
    {
        obj.transform.DOKill();
        //obj.SetActive(false);
        pool.Enqueue(obj);
    }

    //private void OnDestroy()
    //{
    //    foreach (var obj in pool)
    //    {
    //        obj.transform.DOKill();
    //        Destroy(obj);
    //    }
    //}
}