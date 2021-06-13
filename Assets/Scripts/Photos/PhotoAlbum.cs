using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

class PhotoAlbum : MonoBehaviour
{
    public RawImage albumPhoto, albumPhoto1, albumPhoto2, albumPhoto3;
    public Button exitAlbumButton, nextPageButton, backPageButton;
    public PhotoCamera photoCamera;
    private int currentPage;
    private int pageSize;

    public void Start()
    {
        currentPage = 0;
        pageSize = 4;

        exitAlbumButton.onClick.AddListener(ExitAlbumOnClick);
        nextPageButton.onClick.AddListener(NextPageOnClick);
        backPageButton.onClick.AddListener(BackPageOnClick);
        backPageButton.gameObject.SetActive(false);

        LoadAlbumPage();
    }

    private void LoadAlbumPage(int skip = 0)
    {
        List<PhotoDTO> photos = PhotoCollectionDTO.PagePhotoCollection(pageSize, skip).ToList();

        if(photos.ElementAt(0) != null)
        {
            Texture2D tex = new Texture2D((int)albumPhoto.rectTransform.rect.width, (int)albumPhoto.rectTransform.rect.height);
            tex.LoadImage(photos.ElementAt(0).PhotoData);
            albumPhoto.texture = tex;
        }

        if(photos.ElementAt(1) != null)
        {
            Texture2D tex1 = new Texture2D((int)albumPhoto1.rectTransform.rect.width, (int)albumPhoto1.rectTransform.rect.height);
            tex1.LoadImage(photos.ElementAt(1).PhotoData);
            albumPhoto1.texture = tex1;
        }

        if (photos.ElementAt(2) != null)
        {
            Texture2D tex2 = new Texture2D((int)albumPhoto2.rectTransform.rect.width, (int)albumPhoto2.rectTransform.rect.height);
            tex2.LoadImage(photos.ElementAt(2).PhotoData);
            albumPhoto2.texture = tex2;
        }

        if (photos.ElementAt(3) != null)
        {
            Texture2D tex3 = new Texture2D((int)albumPhoto3.rectTransform.rect.width, (int)albumPhoto3.rectTransform.rect.height);
            tex3.LoadImage(photos.ElementAt(3).PhotoData);
            albumPhoto3.texture = tex3;
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

        if(currentPage < maxPageNeed)
        {
            LoadAlbumPage(currentPage * pageSize);
        }

        if(currentPage == maxPageNeed)
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

        if(currentPage <= 2)
        {
            LoadAlbumPage((currentPage - 1) * pageSize);
        }

        if(currentPage == 1)
        {
            backPageButton.gameObject.SetActive(false);
        }

        if(currentPage == 2)
        {
            nextPageButton.gameObject.SetActive(true);
        }

        --currentPage;
    }
}
