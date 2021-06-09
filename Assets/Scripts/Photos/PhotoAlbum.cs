using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

class PhotoAlbum : MonoBehaviour
{
    static GameObject album;
    public RawImage albumPhoto, albumPhoto1, albumPhoto2, albumPhoto3;
    public Button exitAlbumButton;

    public void Start()
    {
        exitAlbumButton.onClick.AddListener(ExitAlbumOnClick);
        album = albumPhoto1.gameObject.transform.parent.gameObject;

        LoadAlbumPage();
    }

    private void LoadAlbumPage()
    {
        IDictionary<Guid, PhotoDTO> photos = PhotoCollectionDTO.LoadData(4);

        Texture2D tex = new Texture2D((int)albumPhoto.rectTransform.rect.width, (int)albumPhoto.rectTransform.rect.height);
        tex.LoadImage(photos.ElementAt(0).Value.PhotoData);
        albumPhoto.texture = tex;

        Texture2D tex1 = new Texture2D((int)albumPhoto1.rectTransform.rect.width, (int)albumPhoto1.rectTransform.rect.height);
        tex1.LoadImage(photos.ElementAt(1).Value.PhotoData);
        albumPhoto1.texture = tex1;

        Texture2D tex2 = new Texture2D((int)albumPhoto2.rectTransform.rect.width, (int)albumPhoto2.rectTransform.rect.height);
        tex2.LoadImage(photos.ElementAt(2).Value.PhotoData);
        albumPhoto2.texture = tex2;

        Texture2D tex3 = new Texture2D((int)albumPhoto3.rectTransform.rect.width, (int)albumPhoto3.rectTransform.rect.height);
        tex3.LoadImage(photos.ElementAt(3).Value.PhotoData);
        albumPhoto3.texture = tex3;
    }

    private void ExitAlbumOnClick()
    {
        album.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PhotoCamera.FinishCamera();
    }
}
