using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private static bool isGamePaused = false;

    public static bool IsGamePaused => isGamePaused;

    void Start()
    {
        //Starts Timer
        TimeStamps.startTime(true);
    }
    void Update()
    {
        //Updates one tick the clock
        TimeStamps.ClockUpdate(Time.deltaTime);
    }

    public static void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f;
        TimeStamps.startTime(false);
    }

    public static void UnpauseGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f;
        TimeStamps.startTime(true);
    }
}