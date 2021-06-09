using System;
using System.Collections.Generic;
using System.Linq;
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
        Debug.LogWarning($"keep clicked");
        PhotoDTO image = PhotoCamera.imageData;

        // Adding photo collection and saving. SaveData() saves the whole collection so we should probably move this out somewhere else.
        PhotoCollectionDTO.AddPhoto(image);
        PhotoCollectionDTO.SaveData();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PhotoCamera.FinishCamera();
    }

    void DiscardOnClick()
    {
        Debug.LogWarning($"discard clicked");
        // TODO add confirmation prompt
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PhotoCamera.FinishCamera();
    }

    void ViewAlbumOnClick()
    {
        Debug.LogWarning($"loading photos. this many: {album.name}");
        album.SetActive(true);
        photoFrame.SetActive(false);
    }
}