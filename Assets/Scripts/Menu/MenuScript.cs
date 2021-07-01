using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    //public Button startButton, settingsButton, quitButton, backButton;
    public Button fileButton;

    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject fileMenu;

    void Start()
    {
        //startButton.onClick.AddListener(StartOnClick);
        //settingsButton.onClick.AddListener(SettingsOnClick);
        //quitButton.onClick.AddListener(QuitOnClick);
        //backButton.onClick.AddListener(BackOnClick);
        fileButton.onClick.AddListener(FileOnClick);
    }

    public void StartOnClick()
    {
        Debug.Log("Starting game.");
        LoadScreen.LoadGameScene();
    }

    public void SettingsOnClick()
    {
        Debug.Log("Loading settings menu.");
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void QuitOnClick()
    {
        // TODO: confirmation modal
        Debug.Log("Quitting game.");
        Application.Quit();
    }

    public void BackOnClick()
    {
        Debug.Log("Loading main menu.");
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void FileOnClick()
    {
        if (!fileMenu.activeSelf)
        {
            fileMenu.SetActive(true);
        }
        else
        {
            fileMenu.SetActive(false);
        }
    }
}
