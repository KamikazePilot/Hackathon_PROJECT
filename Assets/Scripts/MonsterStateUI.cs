using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls monster state via keyboard:
///   1 = Follow Player
///   2 = Go To Location A
///   3 = Go To Location B
/// Also allows external scripts to set the same state.
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
        if (Keyboard.current == null) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            SetAction("1");
        else if (Keyboard.current.digit2Key.wasPressedThisFrame)
            SetAction("2");
        else if (Keyboard.current.digit3Key.wasPressedThisFrame)
            SetAction("3");
    }

    public void SetAction(string action)
    {
        if (monster == null) return;

        switch (action)
        {
            case "1":
                monster.SetState(MonsterState.FollowPlayer);
                Debug.Log("Monster action set to: FollowPlayer");
                break;

            case "2":
                monster.SetState(MonsterState.GoToLocationA);
                Debug.Log("Monster action set to: GoToLocationA");
                break;

            case "3":
                monster.SetState(MonsterState.GoToLocationB);
                Debug.Log("Monster action set to: GoToLocationB");
                break;

            default:
                Debug.LogWarning("Unknown action: " + action);
                break;
        }
    }
}