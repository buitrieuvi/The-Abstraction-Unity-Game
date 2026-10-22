using UnityEngine;
using UnityEngine.AI;

public class FollowPet : MonoBehaviour
{
    [Header("Cài đặt mục tiêu")]
    public Transform targetPlayer;       // Kéo nhân vật (Player) vào đây
    private NavMeshAgent agent;

    [Header("Khoảng cách di chuyển")]
    public float followDistance = 2.5f;  // Khoảng cách tối thiểu để pet bắt đầu di chuyển theo
    public float stopDistance = 1.5f;    // Khoảng cách pet sẽ dừng lại gần nhân vật
    public float teleportDistance = 15f; // Nếu đi quá xa (ví dụ kẹt vật cản), pet sẽ dịch chuyển thẳng tới cạnh player

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

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

    private void Update()
    {
        if (targetPlayer == null || agent == null) return;

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
}