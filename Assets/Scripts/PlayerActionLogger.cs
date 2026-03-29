using System.IO;
using UnityEngine;

public class PlayerActionLogger : MonoBehaviour
{
    public Transform player;
    public Transform monster;

    public float movementLogInterval = 2f;

    private float movementTimer;

    private string filePath;

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "game_log.csv");

        Debug.Log("CSV Path: " + filePath);

        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath,
                "timestamp,action,player_x,player_z,monster_x,monster_z,held_item,item_type,item_id,notes\n"
            );
        }
    }

    void Update()
    {
        movementTimer += Time.deltaTime;

        if (movementTimer >= movementLogInterval)
        {
            movementTimer = 0f;

            LogMovement();
        }
    }

    void LogMovement()
    {
        Vector3 p = player.position;
        Vector3 m = monster.position;

        WriteRow(
            Time.time,
            "Move",
            p.x,
            p.z,
            m.x,
            m.z,
            GetHeldItem(),
            "",
            "",
            ""
        );
    }

    public void LogPickup(string itemType, string itemId)
    {
        Vector3 p = player.position;
        Vector3 m = monster.position;

        WriteRow(
            Time.time,
            "Pickup",
            p.x,
            p.z,
            m.x,
            m.z,
            itemType,
            itemType,
            itemId,
            ""
        );
    }

    public void LogDrop(string itemType, string itemId)
    {
        Vector3 p = player.position;
        Vector3 m = monster.position;

        WriteRow(
            Time.time,
            "Drop",
            p.x,
            p.z,
            m.x,
            m.z,
            "None",
            itemType,
            itemId,
            ""
        );
    }

    public void LogDeposit(string itemType, string zoneName)
    {
        Vector3 p = player.position;
        Vector3 m = monster.position;

        WriteRow(
            Time.time,
            "Deposit",
            p.x,
            p.z,
            m.x,
            m.z,
            "None",
            itemType,
            "",
            zoneName
        );
    }

    void WriteRow(
        float timestamp,
        string action,
        float px,
        float pz,
        float mx,
        float mz,
        string heldItem,
        string itemType,
        string itemId,
        string notes
    )
    {
        string row =
            timestamp + "," +
            action + "," +
            px + "," +
            pz + "," +
            mx + "," +
            mz + "," +
            heldItem + "," +
            itemType + "," +
            itemId + "," +
            notes + "\n";

        File.AppendAllText(filePath, row);
    }

    string GetHeldItem()
    {
        PlayerPickup pickup = player.GetComponent<PlayerPickup>();

        if (pickup == null) return "None";

        return pickup.GetHeldItemType();
    }
}