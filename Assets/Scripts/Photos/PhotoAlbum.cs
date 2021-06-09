using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

class PhotoAlbum : MonoBehaviour
{
    static GameObject album;
    public RawImage albumPhoto, albumPhoto1, albumPhoto2, albumPhoto3;

    public void Start()
    {
        album = albumPhoto1.gameObject.transform.parent.gameObject;
        album.SetActive(true);

        LoadAlbumPage();
    }

    private void LoadAlbumPage()
    {
        IDictionary<Guid, PhotoDTO> photos = PhotoCollectionDTO.LoadData(4);
        Texture2D tex = new Texture2D((int)albumPhoto.rectTransform.rect.width, (int)albumPhoto.rectTransform.rect.height);
        tex.LoadImage(photos.ElementAt(0).Value.PhotoData);

        albumPhoto.texture = tex;
    }
}
