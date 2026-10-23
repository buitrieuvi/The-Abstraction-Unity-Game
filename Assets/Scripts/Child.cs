using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class Child : MonoBehaviour
{
    public enum PetState
    {
        FollowPlayer, // Đi theo sát Player
        Flocking,     // Tản bầy (giữ khoảng cách, không dồn cục)
        Wander        // Tự do di chuyển ngẫu nhiên mọi hướng
    }

    [Header("Trạng thái hiện tại")]
    public PetState currentState = PetState.FollowPlayer;

    [Header("Cài đặt mục tiêu")]
    public Transform targetPlayer;       // Kéo nhân vật (Player) vào đây
    private NavMeshAgent agent;

    [Header("Khoảng cách di chuyển")]
    public float followDistance = 2.5f;  // Khoảng cách tối thiểu để pet bắt đầu di chuyển theo
    public float stopDistance = 1.5f;    // Khoảng cách pet sẽ dừng lại gần nhân vật
    public float teleportDistance = 15f; // Nếu đi quá xa, pet dịch chuyển thẳng tới cạnh player

    [Header("Cấu hình Bầy Đàn (Flocking)")]
    public float neighborRadius = 2.5f;
    public float separationWeight = 1.2f;

    [Header("Cấu hình Tự do (Wander)")]
    public float wanderRadius = 8f;      // Bán kính vùng chọn điểm đi dạo ngẫu nhiên
    public float minWaitTime = 1f;       // Thời gian đứng chờ tối thiểu khi đến đích
    public float maxWaitTime = 3f;       // Thời gian đứng chờ tối đa
    private float wanderTimer;
    private bool isWaiting = false;
    private bool hasDestination = false;

    [Header("Animation")]
    public Animator Animator;
    private static readonly int AnimationSpeedParameter = Animator.StringToHash("speed");

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (Animator == null) Animator = GetComponent<Animator>();

        // Nếu chưa gán player trong Inspector, tự động tìm qua Tag "Player"
        if (targetPlayer == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                targetPlayer = playerObj.transform;
            }
        }

        if (agent != null)
        {
            agent.stoppingDistance = stopDistance;
        }
    }

    // Hàm thay đổi trạng thái được gọi từ Manager
    public void SetState(PetState newState)
    {
        currentState = newState;
        isWaiting = false;
        wanderTimer = 0f;
        hasDestination = false;

        if (agent != null)
        {
            agent.isStopped = false;
            agent.ResetPath();
        }
    }

    // Hàm điều phối di chuyển chính dựa trên trạng thái
    public void OnMove(List<Child> allChildren)
    {
        if (agent == null) return;

        switch (currentState)
        {
            case PetState.FollowPlayer:
                HandleFollowPlayer();
                break;
            case PetState.Flocking:
                HandleFlocking(allChildren);
                break;
            case PetState.Wander:
                HandleWander();
                break;
        }

        // Cập nhật Animator theo tốc độ thực tế của NavMeshAgent
        float currentSpeed = agent.velocity.magnitude;
        if (Animator != null)
        {
            Animator.SetFloat(AnimationSpeedParameter, currentSpeed);
        }
    }

    private void HandleFollowPlayer()
    {
        if (targetPlayer == null) return;

        float distance = Vector3.Distance(transform.position, targetPlayer.position);

        // Trường hợp bị kẹt hoặc đi quá xa, dịch chuyển tức thời về gần Player
        if (distance > teleportDistance)
        {
            agent.Warp(targetPlayer.position - targetPlayer.forward * 2f);
            return;
        }

        // Nếu khoảng cách lớn hơn khoảng cách đi theo thì đuổi theo
        if (distance > followDistance)
        {
            agent.SetDestination(targetPlayer.position);
            agent.isStopped = false;
        }
        else
        {
            // Đã đến gần thì dừng lại
            agent.isStopped = true;
        }
    }

    private void HandleFlocking(List<Child> allChildren)
    {
        if (targetPlayer == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, targetPlayer.position);
        if (distanceToPlayer > teleportDistance)
        {
            agent.Warp(targetPlayer.position - targetPlayer.forward * 2f);
            return;
        }

        // Tính lực tản bầy để các con không bị dồn cục
        Vector3 separationForce = Vector3.zero;
        int count = 0;

        foreach (var other in allChildren)
        {
            if (other != null && other != this)
            {
                float dist = Vector3.Distance(transform.position, other.transform.position);
                if (dist > 0 && dist < neighborRadius)
                {
                    Vector3 pushDir = transform.position - other.transform.position;
                    separationForce += pushDir.normalized / dist;
                    count++;
                }
            }
        }

        Vector3 flockingOffset = count > 0 ? (separationForce * separationWeight) : Vector3.zero;
        Vector3 targetPosition = targetPlayer.position + flockingOffset;

        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        if (distanceToPlayer > followDistance || distanceToTarget > stopDistance)
        {
            agent.SetDestination(targetPosition);
            agent.isStopped = false;
        }
        else
        {
            agent.isStopped = true;
        }
    }

    private void HandleWander()
    {
        agent.isStopped = false;

        // Nếu đang trong thời gian đứng chờ tại điểm đích
        if (isWaiting)
        {
            wanderTimer -= Time.deltaTime;
            agent.velocity = Vector3.zero;

            if (wanderTimer <= 0f)
            {
                isWaiting = false;
            }
            return;
        }

        // Nếu chưa có điểm đến hoặc đã đi tới gần điểm đến
        if (!hasDestination || (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.2f))
        {
            bool foundValidSpot = false;
            Vector3 targetPos = Vector3.zero;

            // Tìm điểm ngẫu nhiên xung quanh trên NavMesh
            for (int i = 0; i < 5; i++)
            {
                Vector2 randomDir2D = Random.insideUnitCircle * wanderRadius;
                Vector3 randomDirection = new Vector3(randomDir2D.x, 0, randomDir2D.y) + transform.position;

                if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
                {
                    targetPos = hit.position;
                    foundValidSpot = true;
                    break;
                }
            }

            if (foundValidSpot)
            {
                agent.SetDestination(targetPos);
                hasDestination = true;
            }
        }

        // Kiểm tra khi đến đích thì chuyển sang trạng thái đứng chờ ngẫu nhiên
        if (hasDestination && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.2f)
        {
            hasDestination = false;
            wanderTimer = Random.Range(minWaitTime, maxWaitTime);
            isWaiting = true;
        }
    }
}