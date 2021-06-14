using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public Button startButton, settingsButton, quitButton, backButton;

    public GameObject mainMenu;
    public GameObject settingsMenu;


    void Start()
    {
        startButton.onClick.AddListener(StartOnClick);
        settingsButton.onClick.AddListener(SettingsOnClick);
        quitButton.onClick.AddListener(QuitOnClick);
        backButton.onClick.AddListener(BackOnClick);
    }

    void StartOnClick()
    {
        LoadScreen.LoadGameScene();
    }

    void SettingsOnClick()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    void QuitOnClick()
    {
        // TODO: confirmation modal
        Debug.Log("Quitting game.");
        Application.Quit();
    }

    void BackOnClick()
    {
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
}
