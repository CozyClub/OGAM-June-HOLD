using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScreen : MonoBehaviour
{
    public static LoadScreen instance;
    private static readonly string gameSceneName = "game";
    private static readonly string menuSceneName = "menu";

    private void Awake()
    {
        instance = this;
    }

    public static void LoadGameScene()
    {
        instance.StartCoroutine(LoadScene(gameSceneName));
    }

    public static void LoadMenuScene()
    {
        instance.StartCoroutine(LoadScene(menuSceneName));
    }

    static IEnumerator LoadScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}