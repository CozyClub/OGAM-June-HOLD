using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TakePic : MonoBehaviour
{
    public int photoMaxPerDay = 3; // low default num for testing

    /// <summary>
    /// Photo directory configuration - can make this configurable in the future
    /// </summary>
    public readonly string defaultPhotoDirectoryPath = "/Resources/Photos/";
    public PhotoFileFormat defaultPhotoFileFormat = PhotoFileFormat.PNG;

    int photoCount = 0;
    KeyCode PhotoKey;
    Texture2D image;
    KeyCtrl keys;
    public RawImage photoDisplay;
    GameObject photo;

    //This is just to see what you're aiming at when taking a picture
    public GameObject photoFrame;
    private Camera Camera;

    public string PhotoDirectoryPath => GetPhotoDirectoryPath();

    public void Start()
    {
        photo = photoDisplay.gameObject.transform.parent.gameObject;
        keys = KeyCtrl.keyctrl;
        //This is found under the gameobject named "GameCtrls". Default space
        PhotoKey = keys.photo.key;
        photo.SetActive(false);
        Camera = GetComponent<Camera>();
        photoFrame.SetActive(false);

        SetupPhotoDirectory();
    }

    private void LateUpdate()
    {
        // Hold down PhotoKey to see frame
        if (Input.GetKeyDown(PhotoKey))
        {
            photoFrame.SetActive(true);
        }

        // Release PhotoKey to take picture
        if (Input.GetKeyUp(PhotoKey))
        {
            Capture();
        }
    }

    /// <summary>
    /// Performs photo capture. Save the rendertexture as a png in resources folder to be able to reference it. Probably not the best way to do this
    /// </summary>
    public void Capture()
    {
        // Deactivates photo frame used for previewing photo before capture.
        photoFrame.SetActive(false);

        // Disallows photo capture if current photo count exceeds max.
        // To do - give warning that max photos have been taken.
        Debug.Log($"Photo #{photoCount} captured.");
        if (photoCount >= photoMaxPerDay)
        {
            Debug.LogWarning("Max photos taken, returning.");
            return;
        }

        RenderTexture myRenderTexture = RenderTexture.active;
        RenderTexture.active = Camera.targetTexture;

        Camera.Render();

        // Create new texture to width and height of photo camera.
        image = new Texture2D(Camera.targetTexture.width, Camera.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, Camera.targetTexture.width, Camera.targetTexture.height), 0, 0);
        image.Apply();
        RenderTexture.active = myRenderTexture;

        byte[] bytes = image.EncodeToPNG();

        // Write photo to photo directory path
        File.WriteAllBytes(PhotoDirectoryPath + photoCount + ".png", bytes);
        Debug.Log(PhotoDirectoryPath + photoCount + ".png");
        photoCount++;

        StartCoroutine(DisplayPhoto());
    }

    /// <summary>
    /// Little short preview of the photo taken
    /// </summary>
    public IEnumerator DisplayPhoto()
    {
        yield return new WaitForSeconds(1);

        photoDisplay.texture = image;
        photo.SetActive(true);

        yield return new WaitForSeconds(4);

        photo.SetActive(false);
        Destroy(image);

        yield return null;
    }

    /// <summary>
    /// Initializes photo path directory if doesn't exist yet.
    /// </summary>
    private void SetupPhotoDirectory()
    {
        // Set up photo directory
        string photoDirectoryPath = GetPhotoDirectoryPath();
        Debug.Log("Photo directory path is: " + photoDirectoryPath);

        DirectoryInfo dir = new DirectoryInfo(photoDirectoryPath);
        if (!dir.Exists)
        {
            Directory.CreateDirectory(photoDirectoryPath);
        }

        string fileExtension = defaultPhotoFileFormat.ToFileExtension();
        FileSystemInfo[] systemInfos = dir.GetFileSystemInfos($"*.{fileExtension}");
        Debug.Log("System info count is: " + systemInfos.Length);
        photoCount = systemInfos.Length;
    }

    private string GetPhotoDirectoryPath()
    {
        return Application.persistentDataPath + defaultPhotoDirectoryPath;
    }
}
