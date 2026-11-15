
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using Zenject;
using UnityEngine.Events;

public class EnemyAI : MonoBehaviour
{
    public enum BehaviorState
    {
        Chase,
        Flocking,
        Wander
    }

    [Header("State")]
    [SerializeField]
    private BehaviorState currentState = BehaviorState.Chase;

    [Header("Components")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    private EnemyManager manager;
    private Transform target;

    private static readonly int SpeedHash =
        Animator.StringToHash("speed");

    [Header("Chase Settings")]
    [SerializeField] private float chaseDistance = 2.5f;
    [SerializeField] private float stopDistance = 1.5f;
    [SerializeField] private float teleportDistance = 25f;

    [Header("Flocking Settings")]
    [SerializeField] private float neighborRadius = 3f;
    [SerializeField] private float separationWeight = 2f;
    [SerializeField] private float flockingFollowWeight = 1f;

    [Header("Wander Settings")]
    [SerializeField] private float wanderRadius = 8f;
    [SerializeField] private float minWaitTime = 1f;
    [SerializeField] private float maxWaitTime = 3f;

    [Header("Navigation")]
    [SerializeField] private float destinationUpdateInterval = 0.2f;
    [SerializeField] private float destinationThreshold = 0.5f;

    private float destinationTimer;
    private float wanderTimer;

    private bool isWaiting;
    private bool hasDestination;

    private Vector3 lastDestination;

    [Header("Health")]
    [SerializeField] private int maxHP = 10;
    [SerializeField] private int currentHP = 10;

    private bool isDead;
    public UnityAction OnDead;

    public BehaviorState CurrentState => currentState;
    public int CurrentHP => currentHP;
    public bool IsDead => isDead;

    // =========================================
    // INITIALIZATION
    // =========================================

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponent<Animator>();

        currentHP = maxHP;
    }

    public void Initialize(
        EnemyManager owner,
        Transform player)
    {
        manager = owner;
        target = player;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // =========================================
    // STATE MACHINE
    // =========================================

    public void SetState(BehaviorState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;

        ResetMovement();
    }

    private void ResetMovement()
    {
        isWaiting = false;
        hasDestination = false;

        wanderTimer = 0f;
        destinationTimer = 0f;

        if (agent == null || !agent.enabled ||
            !agent.isOnNavMesh)
            return;

        agent.isStopped = false;
        agent.ResetPath();
    }

    // =========================================
    // AI UPDATE
    // =========================================

    public void Tick(IReadOnlyList<EnemyAI> allEnemies)
    {
        if (isDead || agent == null ||
            !agent.enabled || !agent.isOnNavMesh)
            return;

        destinationTimer -= Time.deltaTime;

        switch (currentState)
        {
            case BehaviorState.Chase:
                HandleChase();
                break;

            case BehaviorState.Flocking:
                HandleFlocking(allEnemies);
                break;

            case BehaviorState.Wander:
                HandleWander();
                break;
        }

        UpdateAnimation();
    }

    // =========================================
    // CHASE PLAYER
    // =========================================

    private void HandleChase()
    {
        if (target == null)
        {
            StopMovement();
            return;
        }

        float distance = Vector3.Distance(
            transform.position,
            target.position
        );

        // Quái vật quá xa Player
        if (distance > teleportDistance)
        {
            RecoverPosition();
            return;
        }

        agent.stoppingDistance = stopDistance;

        if (distance > chaseDistance)
        {
            agent.isStopped = false;

            UpdateDestination(target.position);
        }
        else
        {
            StopMovement();
        }
    }

    // =========================================
    // FLOCKING
    // =========================================

    private void HandleFlocking(
        IReadOnlyList<EnemyAI> allEnemies)
    {
        if (target == null)
            return;

        Vector3 separation = Vector3.zero;

        int neighborCount = 0;

        foreach (var other in allEnemies)
        {
            if (other == null ||
                other == this ||
                other.isDead)
                continue;

            Vector3 direction =
                transform.position - other.transform.position;

            direction.y = 0f;

            float distance = direction.magnitude;

            if (distance > 0.01f &&
                distance < neighborRadius)
            {
                separation += direction.normalized / distance;

                neighborCount++;
            }
        }

        Vector3 separationOffset = Vector3.zero;

        if (neighborCount > 0)
        {
            separationOffset =
                separation.normalized * separationWeight;
        }

        Vector3 targetDirection =
            target.position - transform.position;

        targetDirection.y = 0f;

        Vector3 desiredPosition =
            target.position -
            targetDirection.normalized * stopDistance +
            separationOffset;

        float distanceToPlayer = Vector3.Distance(
            transform.position,
            target.position
        );

        if (distanceToPlayer > teleportDistance)
        {
            RecoverPosition();
            return;
        }

        if (distanceToPlayer > chaseDistance ||
            separationOffset.sqrMagnitude > 0.01f)
        {
            agent.stoppingDistance = stopDistance;
            agent.isStopped = false;

            UpdateDestination(desiredPosition);
        }
        else
        {
            StopMovement();
        }
    }

    // =========================================
    // WANDER
    // =========================================

    private void HandleWander()
    {
        if (isWaiting)
        {
            wanderTimer -= Time.deltaTime;

            if (wanderTimer <= 0f)
            {
                isWaiting = false;
            }

            return;
        }

        agent.stoppingDistance = 0.3f;
        agent.isStopped = false;

        bool reachedDestination =
            !agent.pathPending &&
            agent.hasPath &&
            agent.remainingDistance <=
            agent.stoppingDistance + 0.2f;

        if (!hasDestination || reachedDestination)
        {
            if (reachedDestination)
            {
                isWaiting = true;

                wanderTimer = Random.Range(
                    minWaitTime,
                    maxWaitTime
                );

                hasDestination = false;

                StopMovement();
                return;
            }

            FindWanderPosition();
        }
    }

    private void FindWanderPosition()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 random = Random.insideUnitCircle *
                             wanderRadius;

            Vector3 position = transform.position +
                new Vector3(random.x, 0f, random.y);

            if (NavMesh.SamplePosition(
                position,
                out NavMeshHit hit,
                3f,
                NavMesh.AllAreas))
            {
                hasDestination = true;

                UpdateDestination(hit.position);

                return;
            }
        }

        // Không tìm được vị trí hợp lệ
        hasDestination = false;
    }

    // =========================================
    // NAVIGATION
    // =========================================

    private void UpdateDestination(Vector3 destination)
    {
        if (destinationTimer > 0f)
            return;

        if (lastDestination != Vector3.zero &&
            Vector3.Distance(lastDestination, destination)
            < destinationThreshold)
            return;

        if (agent.SetDestination(destination))
        {
            lastDestination = destination;
        }

        destinationTimer = destinationUpdateInterval;
    }

    private void StopMovement()
    {
        if (agent == null || !agent.enabled ||
            !agent.isOnNavMesh)
            return;

        agent.isStopped = true;
    }

    // =========================================
    // RECOVERY
    // =========================================

    private void RecoverPosition()
    {
        if (target == null)
            return;

        Vector2 random = Random.insideUnitCircle * 2f;

        Vector3 desiredPosition =
            target.position +
            new Vector3(random.x, 0f, random.y);

        if (NavMesh.SamplePosition(
            desiredPosition,
            out NavMeshHit hit,
            5f,
            NavMesh.AllAreas))
        {
            agent.Warp(hit.position);

            ResetMovement();
        }
    }

    // =========================================
    // ANIMATION
    // =========================================

    private void UpdateAnimation()
    {
        if (animator == null)
            return;

        float speed = agent.velocity.magnitude;

        animator.SetFloat(
            SpeedHash,
            speed,
            0.15f,
            Time.deltaTime
        );
    }

    // =========================================
    // HEALTH / DAMAGE
    // =========================================

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0)
            return;

        currentHP = Mathf.Max(0, currentHP - damage);

        transform.DOKill();

        transform.DOScale(
            Vector3.one * 0.7f,
            0.12f
        ).OnComplete(() =>
        {
            if (this != null && !isDead)
            {
                transform.DOScale(
                    Vector3.one * 0.5f,
                    0.12f
                );
            }
        });

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        if (manager != null)
            manager.Unregister(this);

        if (agent != null)
            agent.enabled = false;

        transform.DOKill();
        OnDead?.Invoke();
        gameObject.SetActive(false);

       
    }

    public void Heal(int amount)
    {
        if (isDead || amount <= 0)
            return;

        currentHP = Mathf.Min(
            maxHP,
            currentHP + amount
        );
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }

    private void OnTriggerEnter(Collider other)
    {
        Cube cube = other.GetComponent<Cube>();
        if (cube != null) 
        {
            TakeDamage(10);

        }


    }
}
