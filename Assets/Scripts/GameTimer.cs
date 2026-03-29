using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public float timeRemaining = 60f;   // starting time in seconds
    public TextMeshProUGUI timerText;
    public bool timerRunning = true;

    void Update()
    {
        if (timerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplay(timeRemaining);
            }
            else
            {
                timeRemaining = 0;
                timerRunning = false;
                UpdateTimerDisplay(timeRemaining);
                TimerEnded();
            }
        }
    }

    void UpdateTimerDisplay(float timeToDisplay)
    {
        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void TimerEnded()
    {
        Debug.Log("Time's up!");
        Time.timeScale = 0f;
        
        // Put end-game logic here
        // Example:
        // disable player movement
        // show game over screen
        // stop spawning
    }
}