using DG.Tweening;
using R3.Triggers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public sealed class PlayerSkills : MonoBehaviour
{
    [Inject] public InputManager InputManager { get; private set; }

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
        InputManager.InputActions.Player.Attack.performed += _ => Shoot();
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

        Vector3 direction = StartPoint.forward.normalized;
        projectile.transform.SetPositionAndRotation(
            StartPoint.position,
            Quaternion.LookRotation(direction)
        );

        projectile.SetActive(true);
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.transform.DOKill();
        pool.Enqueue(obj);
    }
}