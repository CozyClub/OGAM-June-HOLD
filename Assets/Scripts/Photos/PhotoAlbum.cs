using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

class PhotoAlbum : MonoBehaviour
{
    public Button exitAlbumButton, nextPageButton, backPageButton;
    public PhotoCamera photoCamera;

    // PhotoDisplays for showing images on album page
    private List<RawImage> albumPhotos;
    private int currentPage;
    private int pageSize;

    public void Start()
    {
        currentPage = 0;
        pageSize = 4;

        albumPhotos = gameObject.GetComponentsInChildren<RawImage>().ToList();

        exitAlbumButton.onClick.AddListener(ExitAlbumOnClick);
        nextPageButton.onClick.AddListener(NextPageOnClick);
        backPageButton.onClick.AddListener(BackPageOnClick);
        backPageButton.gameObject.SetActive(false);

        LoadAlbumPage();
    }

    private void LoadAlbumPage(int skip = 0)
    {
        List<PhotoDTO> photos = PhotoCollectionDTO.PagePhotoCollection(pageSize, skip).ToList();

        for (int i = 0; i < albumPhotos.Count; ++i)
        {
            if (photos.ElementAtOrDefault(i) != null)
            {
                RawImage albumPhoto = albumPhotos[i];

                Texture2D tex = new Texture2D((int)albumPhoto.rectTransform.rect.width, (int)albumPhoto.rectTransform.rect.height);
                tex.LoadImage(photos[i].PhotoData);
                albumPhoto.texture = tex;
            }
        }
    }

    private void ExitAlbumOnClick()
    {
        gameObject.SetActive(false);
        photoCamera.FinishCamera();
    }

    private void NextPageOnClick()
    {
        short photoCount = PhotoCollectionDTO.GetPhotoCount();
        int maxPageNeed = photoCount / pageSize; // TODO: check on rounding
        if (currentPage >= maxPageNeed) return;

        if (currentPage < maxPageNeed)
        {
            LoadAlbumPage(currentPage * pageSize);
        }

        if (currentPage == maxPageNeed)
        {
            nextPageButton.gameObject.SetActive(false);
        }

        if (currentPage < 1)
        {
            backPageButton.gameObject.SetActive(true);
        }

        ++currentPage;
    }

    private void BackPageOnClick()
    {
        if (currentPage == 0) return;

        if (currentPage <= 2)
        {
            LoadAlbumPage((currentPage - 1) * pageSize);
        }

        if (currentPage == 1)
        {
            backPageButton.gameObject.SetActive(false);
        }

        if (currentPage == 2)
        {
            nextPageButton.gameObject.SetActive(true);
        }

        --currentPage;
    }
}
