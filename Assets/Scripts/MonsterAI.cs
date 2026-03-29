using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Simple monster that chases the player using the NavMesh.
/// Attach this script to a GameObject that also has a NavMeshAgent component.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class MonsterAI : MonoBehaviour
{
    [Header("Chase Settings")]
    [Tooltip("The player's Transform. If left empty, will auto-find by 'Player' tag.")]
    public Transform playerTarget;

    [Tooltip("How far away the monster can detect the player and start chasing.")]
    public float detectionRange = 20f;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        // Speed, stopping distance, angular speed, and acceleration are set
        // directly on the NavMeshAgent component in the Inspector.

        // Auto-find player if not assigned in Inspector
        if (playerTarget == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerTarget = player.transform;
            }
            else
            {
                Debug.LogWarning("MonsterAI: No player target assigned and no GameObject with tag 'Player' found.");
            }
        }
    }

    void Update()
    {
        if (playerTarget == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer > agent.stoppingDistance)
            {
                agent.isStopped = false;
                agent.SetDestination(playerTarget.position);
            }
            else
            {
                agent.isStopped = true;
            }
        }
        else
        {
            agent.isStopped = true;
        }
    }

    // Visualize detection range in the Unity Editor (visible in Scene view)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        NavMeshAgent a = GetComponent<NavMeshAgent>();
        if (a != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, a.stoppingDistance);
        }
    }
}
