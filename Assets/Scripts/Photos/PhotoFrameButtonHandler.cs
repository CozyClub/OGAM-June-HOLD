using UnityEngine;
using UnityEngine.UI;

public class PhotoFrameButtonHandler : MonoBehaviour
{
    //Make sure to attach these Buttons in the Inspector
    public Button keepButton, discardButton, viewAlbumButton;
    public GameObject album;
    public GameObject photoFrame;

    void Start()
    {
        keepButton.onClick.AddListener(KeepOnClick);
        discardButton.onClick.AddListener(DiscardOnClick);
        viewAlbumButton.onClick.AddListener(ViewAlbumOnClick);
    }

    void KeepOnClick()
    {
        PhotoDTO image = PhotoCamera.imageData;

        // Adding photo collection and saving. SaveData() saves the whole collection so we should probably move this out somewhere else.
        PhotoCollectionDTO.AddPhoto(image);
        PhotoCollectionDTO.SaveData();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        album.GetComponent<PhotoAlbum>().photoCamera.FinishCamera();
    }

    void DiscardOnClick()
    {
        // TODO add confirmation prompt
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        album.GetComponent<PhotoAlbum>().photoCamera.FinishCamera();
    }

    void ViewAlbumOnClick()
    {
        album.SetActive(true);
        photoFrame.SetActive(false);
    }
}