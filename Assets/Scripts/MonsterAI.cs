using UnityEngine;
using UnityEngine.AI;

public enum MonsterState
{
    FollowPlayer,
    GoToLocationA,
    GoToLocationB
}

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CapsuleCollider))]
public class MonsterAI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The player's Transform. If left empty, will auto-find by 'Player' tag.")]
    public Transform playerTarget;

    [Tooltip("The child GameObject that holds the mesh and animations.")]
    public Transform modelChild;

    [Header("Chase Settings")]
    [Tooltip("How far away the monster can detect the player and start chasing.")]
    public float detectionRange = 20f;

    [Header("Location A")]
    public Transform locationA;
    public Vector3 locationAModelScale = new Vector3(2f, 1f, 2f);
    public float locationACapsuleRadius = 1f;
    public float locationACapsuleHeight = 2f;

    [Header("Location B")]
    public Transform locationB;
    public Vector3 locationBModelScale = new Vector3(2f, 1f, 2f);
    public float locationBCapsuleRadius = 1f;
    public float locationBCapsuleHeight = 2f;

    [Header("Scale Transition")]
    [Tooltip("How fast the monster lerps to its target scale when at a location.")]
    public float scaleSpeed = 2f;

    // Defaults stored on Start so we can restore them when switching back to FollowPlayer
    private Vector3 defaultModelScale;
    private float defaultCapsuleRadius;
    private float defaultCapsuleHeight;

    private NavMeshAgent agent;
    private CapsuleCollider capsule;
    [SerializeField] private MonsterState currentState = MonsterState.FollowPlayer;
    private bool arrivedAtLocation = false;

    // Target scale values for the current state
    private Vector3 targetModelScale;
    private float targetCapsuleRadius;
    private float targetCapsuleHeight;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        capsule = GetComponent<CapsuleCollider>();

        defaultModelScale = modelChild != null ? modelChild.localScale : Vector3.one;
        defaultCapsuleRadius = capsule.radius;
        defaultCapsuleHeight = capsule.height;

        targetModelScale = defaultModelScale;
        targetCapsuleRadius = defaultCapsuleRadius;
        targetCapsuleHeight = defaultCapsuleHeight;

        // Reset any stale destination saved in the scene
        agent.ResetPath();

        if (playerTarget == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
                playerTarget = player.transform;
            else
                Debug.LogWarning("MonsterAI: No player target assigned and no GameObject with tag 'Player' found.");
        }

        if (modelChild == null)
        {
            // Auto-find the first child with a Renderer as the model
            Renderer childRenderer = GetComponentInChildren<Renderer>();
            if (childRenderer != null && childRenderer.gameObject != gameObject)
                modelChild = childRenderer.transform;
            else
                Debug.LogWarning("MonsterAI: Could not auto-find model child. Assign it manually.");
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case MonsterState.FollowPlayer:
                UpdateFollowPlayer();
                break;
            case MonsterState.GoToLocationA:
                UpdateGoToLocation(locationA);
                break;
            case MonsterState.GoToLocationB:
                UpdateGoToLocation(locationB);
                break;
        }

        // Smoothly lerp capsule and model scale toward target
        if (modelChild != null)
            modelChild.localScale = Vector3.Lerp(modelChild.localScale, targetModelScale, Time.deltaTime * scaleSpeed);

        capsule.radius = Mathf.Lerp(capsule.radius, targetCapsuleRadius, Time.deltaTime * scaleSpeed);
        capsule.height = Mathf.Lerp(capsule.height, targetCapsuleHeight, Time.deltaTime * scaleSpeed);
    }

    void UpdateFollowPlayer()
    {
        if (playerTarget == null) return;

        float dist = Vector3.Distance(transform.position, playerTarget.position);

        if (dist <= detectionRange)
        {
            if (dist > agent.stoppingDistance)
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

    void UpdateGoToLocation(Transform location)
    {
        if (location == null) return;

        if (!arrivedAtLocation)
        {
            agent.isStopped = false;
            agent.SetDestination(location.position);

            bool pathReady = !agent.pathPending && agent.hasPath;
            bool closeEnough = agent.remainingDistance <= 0.1f;

            if (pathReady && closeEnough)
            {
                agent.isStopped = true;
                arrivedAtLocation = true;
            }
        }
    }

    public void SetState(MonsterState newState)
    {
        currentState = newState;
        arrivedAtLocation = false;

        switch (newState)
        {
            case MonsterState.FollowPlayer:
                targetModelScale = defaultModelScale;
                targetCapsuleRadius = defaultCapsuleRadius;
                targetCapsuleHeight = defaultCapsuleHeight;
                agent.ResetPath();
                break;
            case MonsterState.GoToLocationA:
                targetModelScale = locationAModelScale;
                targetCapsuleRadius = locationACapsuleRadius;
                targetCapsuleHeight = locationACapsuleHeight;
                break;
            case MonsterState.GoToLocationB:
                targetModelScale = locationBModelScale;
                targetCapsuleRadius = locationBCapsuleRadius;
                targetCapsuleHeight = locationBCapsuleHeight;
                break;
        }
    }

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

        if (locationA != null) { Gizmos.color = Color.cyan;    Gizmos.DrawSphere(locationA.position, 0.4f); }
        if (locationB != null) { Gizmos.color = Color.magenta; Gizmos.DrawSphere(locationB.position, 0.4f); }
    }
}
