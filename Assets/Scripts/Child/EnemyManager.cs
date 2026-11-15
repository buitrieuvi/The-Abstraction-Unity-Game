
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemyManager : MonoBehaviour
{
    [Inject] DataManager dataManager;

    public GameObject Prefabs;

    [Header("Enemy Group")]
    [SerializeField] private List<EnemyAI> enemies = new();

    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Group State")]
    [SerializeField]
    private EnemyAI.BehaviorState selectedState =
        EnemyAI.BehaviorState.Chase;

    private EnemyAI.BehaviorState lastState;

    public IReadOnlyList<EnemyAI> Enemies => enemies;

    public Transform Target => target;

    private void Awake()
    {
        enemies.Clear();

        for (int i = 0; i < 10; i++)
        {
            Instantiate(Prefabs, transform);
        }

        enemies.AddRange(
            GetComponentsInChildren<EnemyAI>(true)
        );

        lastState = selectedState;

        InitializeEnemies();
    }

    private void InitializeEnemies()
    {
        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;

            enemy.Initialize(this, target);
            enemy.SetState(selectedState);
            enemy.OnDead += () => 
            {
                dataManager.Money += 1;
                dataManager.MoneyCube = 1;
            };
        }
    }

    private void Update()
    {
        // Phát hiện thay đổi trạng thái Inspector
        if (selectedState != lastState)
        {
            ChangeAllState(selectedState);
        }

        // Cập nhật AI
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            EnemyAI enemy = enemies[i];

            if (enemy == null)
            {
                enemies.RemoveAt(i);
                continue;
            }

            enemy.Tick(enemies);
        }
    }

    public void ChangeAllState(
        EnemyAI.BehaviorState newState)
    {
        selectedState = newState;
        lastState = newState;

        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;

            enemy.SetState(newState);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;

        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;

            enemy.SetTarget(target);
        }
    }

    public void Register(EnemyAI enemy)
    {
        if (enemy == null || enemies.Contains(enemy))
            return;

        enemies.Add(enemy);
        enemy.Initialize(this, target);
        enemy.SetState(selectedState);
    }

    public void Unregister(EnemyAI enemy)
    {
        enemies.Remove(enemy);
    }
}
