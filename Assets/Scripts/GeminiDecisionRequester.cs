using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class GeminiDecisionRequester : MonoBehaviour
{
    [Header("Backend Settings")]
    public string backendUrl = "http://127.0.0.1:5000/monster-decision";

    [Header("Timing")]
    public float requestInterval = 30f;

    private float timer = 0f;
    private string csvPath;

    void Start()
    {
        // Find CSV automatically
        csvPath = System.IO.Path.Combine(
            Application.persistentDataPath,
            "game_log.csv"
        );

        Debug.Log("Gemini CSV Path: " + csvPath);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= requestInterval)
        {
            timer = 0f;
            StartCoroutine(RequestDecision());
        }
    }

    IEnumerator RequestDecision()
    {
        RequestBody body = new RequestBody
        {
            csv_path = csvPath
        };

        string json = JsonUtility.ToJson(body);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        UnityWebRequest req =
            new UnityWebRequest(backendUrl, "POST");

        req.uploadHandler =
            new UploadHandlerRaw(bodyRaw);

        req.downloadHandler =
            new DownloadHandlerBuffer();

        req.SetRequestHeader(
            "Content-Type",
            "application/json"
        );

        Debug.Log("Sending CSV to Gemini backend...");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Gemini response: " + req.downloadHandler.text);

            DecisionResponse result =
                JsonUtility.FromJson<DecisionResponse>(
                    req.downloadHandler.text
                );

            // Send decision to monster
            MonsterStateUI ui =
                FindFirstObjectByType<MonsterStateUI>();

            if (ui != null && result != null)
            {
                ui.SetAction(result.action);
            }
        }
        else
        {
            Debug.LogError("Request failed: " + req.error);
        }
    }

    [System.Serializable]
    public class RequestBody
    {
        public string csv_path;
    }

    [System.Serializable]
    public class DecisionResponse
    {
        public string action;
        public string reason;
    }
}