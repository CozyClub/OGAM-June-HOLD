using UnityEngine;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
    public GameObject gameMenu;
    public GameObject innerMenuPrefab;
    public Button photoAlbumButton, guidebookButton;

    public void OpenGuidebookOnClick()
    {
        GameObject innerMenu = InitializeInnerMenu();
    }

    public void OpenPhotoAlbumOnClick()
    {
        GameObject innerMenu = InitializeInnerMenu();
        TextMesh menuText = innerMenu.GetComponent<TextMesh>();
        menuText.text = "photo album";
    }

    private GameObject InitializeInnerMenu()
    {
        GameObject innerMenu = Instantiate(innerMenuPrefab, gameMenu.transform);
        Button exitButton = innerMenu.GetComponent<Button>();
        exitButton.onClick.AddListener(delegate { ExitInnerMenuOnClick(innerMenu); });
        return innerMenu;
    }

    public void ExitInnerMenuOnClick(GameObject parent)
    {
        Destroy(parent);
    }

    public void ExitMainMenuOnClick()
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
