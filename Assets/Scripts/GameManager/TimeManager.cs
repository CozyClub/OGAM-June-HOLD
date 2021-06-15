using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private static bool isGamePaused = false;

    public static bool IsGamePaused => isGamePaused;

    public static void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f;
    }

    public static void UnpauseGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f;
    }
}