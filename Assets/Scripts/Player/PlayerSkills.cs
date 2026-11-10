using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class PlayerSkills : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject GameObjectPrefab;
    [SerializeField] private Transform StartPoint;

    [Header("Pooling")]
    [SerializeField] private int poolSize = 10;

    [Header("Projectile")]
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float moveDistance = 10f;

    private readonly Queue<GameObject> pool = new();

    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private LayerMask groundLayer;

    private void Awake()
    {
        InitializePool();
    }

    private void Update()
    {
        RotateToMouse();
    }

    private void RotateToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(
            Mouse.current.position.ReadValue()
        );

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer))
        {
            Vector3 direction = hit.point - transform.position;

            // Chỉ xoay trên trục Y
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.001f)
                return;

            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
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
        obj.SetActive(false);
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