using System.IO;
using UnityEngine;

public class PlayerActionLogger : MonoBehaviour
{
    public static PlayerActionLogger Instance;

    private string filePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        filePath = Path.Combine(Application.persistentDataPath, "player_action_log.csv");

        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath,
                "timestamp,frame,action,player_x,player_y,player_z,item_type,item_id,held_item,extra\n");
        }

        Debug.Log("CSV saved at: " + filePath);
    }

    public void LogAction(
        string action,
        Vector3 playerPosition,
        string itemType = "",
        string itemId = "",
        string heldItem = "",
        string extra = "")
    {
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

        string row =
            $"{Escape(timestamp)}," +
            $"{Time.frameCount}," +
            $"{Escape(action)}," +
            $"{playerPosition.x:F2}," +
            $"{playerPosition.y:F2}," +
            $"{playerPosition.z:F2}," +
            $"{Escape(itemType)}," +
            $"{Escape(itemId)}," +
            $"{Escape(heldItem)}," +
            $"{Escape(extra)}\n";

        File.AppendAllText(filePath, row);
    }

    private string Escape(string value)
    {
        if (string.IsNullOrEmpty(value))
            return "";

        value = value.Replace("\"", "\"\"");
        return $"\"{value}\"";
    }
}