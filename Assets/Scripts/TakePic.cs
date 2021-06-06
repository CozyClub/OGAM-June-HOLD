using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TakePic : MonoBehaviour
{
    //Just a count in the name for now, can change to something that relates better to gameplay probably
    public int fileCounter;
    //I put a maximum pictures so you can't take thousands of heavy pictures 
    public int fileMaxPerDay;

    int photocount = 0;
    KeyCode PhotoKey;
    private Camera Camera;
    Texture2D image;
    KeyCtrl keys;
    public RawImage photoDisplay;
    GameObject photo;
    //This is just to see what you're aiming at when taking a picture
    public GameObject pictureFrame;

    public void Start()
    {
        photo = photoDisplay.gameObject.transform.parent.gameObject;
        keys = KeyCtrl.keyctrl;
        //This is found under the gameobject named "GameCtrls". Default space
        PhotoKey = keys.photo.key;
        photo.SetActive(false);
        Camera = GetComponent<Camera>();
        pictureFrame.SetActive(false);
    }
    private void LateUpdate()
    {

        //Hold down to see frame, release to take picture
        if (Input.GetKeyDown(PhotoKey))
        {
            pictureFrame.SetActive(true);
        }

        if (Input.GetKeyUp(PhotoKey))
        {
            Capture();
           
        }
    }

    public void Capture()
    {
        //Save the rendertexture as a png in resources folder to be able to reference it. Probably not the best way to do this
        pictureFrame.SetActive(false);
        if (photocount >= fileMaxPerDay)
        { return; }

        RenderTexture myRenderTexture = RenderTexture.active;
        RenderTexture.active = Camera.targetTexture;

        Camera.Render();

        image = new Texture2D(Camera.targetTexture.width, Camera.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, Camera.targetTexture.width, Camera.targetTexture.height), 0, 0);
        image.Apply();
        RenderTexture.active = myRenderTexture;

        byte[] bytes = image.EncodeToPNG();


        File.WriteAllBytes(Application.dataPath + "/Resources/Photos/" + fileCounter + ".png", bytes);
        Debug.Log(Application.dataPath + "/Resources/Photos/" + fileCounter + ".png");
        fileCounter++;
        photocount++;
        StartCoroutine(DisplayPhoto(fileCounter));
    }


    //Little short preview of the picture you just took
    public IEnumerator DisplayPhoto(int path)
    {
        yield return new WaitForSeconds(1);
        photoDisplay.texture = image;

        photo.SetActive(true);
        yield return new WaitForSeconds(4);
        photo.SetActive(false);
        Destroy(image);
        yield return null;
    }    
}
