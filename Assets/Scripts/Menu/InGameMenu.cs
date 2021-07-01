using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
    public GameObject gameMenu;
    public GameObject innerMenuPrefab;
    public Button photoAlbumButton, guidebookButton;

    private GameObject guideBookMenu, photoAlbumMenu;

    public void OpenGuidebookOnClick()
    {
        if (guideBookMenu == null || !guideBookMenu.activeSelf)
        {
            if(photoAlbumMenu != null && photoAlbumMenu.activeSelf)
            {
                guideBookMenu = InitializeInnerMenu(offset: true);
            }
            else
            {
                guideBookMenu = InitializeInnerMenu(offset: false);
            }
        }

        TextMeshProUGUI menuText = guideBookMenu.GetComponentInChildren<TextMeshProUGUI>();
        menuText.text = "guidebook";

        ScrollRect scrollRect = guideBookMenu.GetComponentInChildren<ScrollRect>();
    }

    public void OpenPhotoAlbumOnClick()
    {
        if (photoAlbumMenu == null || !photoAlbumMenu.activeSelf)
        {
            if (guideBookMenu != null && guideBookMenu.activeSelf)
            {
                photoAlbumMenu = InitializeInnerMenu(offset: true);
            }
            else
            {
                photoAlbumMenu = InitializeInnerMenu(offset: false);
            }
        }

        TextMeshProUGUI menuText = photoAlbumMenu.GetComponentInChildren<TextMeshProUGUI>();
        menuText.text = "photo album";
    }

    private GameObject InitializeInnerMenu(bool offset)
    {
        Vector3 menuTransform = innerMenuPrefab.transform.position;
        if (offset)
        {
            menuTransform = innerMenuPrefab.transform.position + new Vector3(10f, -10f, 0);
        }

        GameObject innerMenu = Instantiate(innerMenuPrefab, menuTransform, innerMenuPrefab.transform.rotation);
        innerMenu.transform.SetParent(gameMenu.transform, false);

        Button exitButton = innerMenu.GetComponentInChildren<Button>();
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
