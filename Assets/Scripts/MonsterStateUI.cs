using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls monster state via keyboard:
///   1 = Follow Player
///   2 = Go To Location A
///   3 = Go To Location B
/// </summary>
public class MonsterStateUI : MonoBehaviour
{
    [Tooltip("The MonsterAI component to control.")]
    public MonsterAI monster;

    void Start()
    {
        if (monster == null)
        {
            GameObject m = GameObject.FindWithTag("Monster");
            if (m != null)
                monster = m.GetComponent<MonsterAI>();
            else
                Debug.LogWarning("MonsterStateUI: No MonsterAI assigned and no GameObject with tag 'Monster' found.");
        }
    }

    void Update()
    {
        if (monster == null) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            monster.SetState(MonsterState.FollowPlayer);
        else if (Keyboard.current.digit2Key.wasPressedThisFrame)
            monster.SetState(MonsterState.GoToLocationA);
        else if (Keyboard.current.digit3Key.wasPressedThisFrame)
            monster.SetState(MonsterState.GoToLocationB);
    }
}
