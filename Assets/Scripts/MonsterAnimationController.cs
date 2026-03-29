using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterAnimationController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        // Search only children, not the parent, since the model and its
        // Animator Controller live on the child GameObject.
        foreach (Animator anim in GetComponentsInChildren<Animator>())
        {
            if (anim.gameObject != gameObject)
            {
                animator = anim;
                break;
            }
        }
    }

    void Update()
    {
        animator.SetBool("IsMoving", !agent.isStopped);
    }
}
