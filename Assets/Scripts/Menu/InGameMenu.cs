using UnityEngine;
using UnityEngine.EventSystems;

public class InGameMenu : MonoBehaviour
{
    public GameObject gameMenu;

    public void ExitMenuOnClick()
    {
        TimeManager.UnpauseGame();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

            Debug.Log("deactivating menu");
        if (gameMenu.activeSelf)
        {
            gameMenu.SetActive(false);
        }
    }

    #region Input-controlled actions

    public void LoadMenu()
    {
        TimeManager.PauseGame();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (!gameMenu.activeSelf)
        {
            gameMenu.SetActive(true);
        }
    }

    #endregion
}
