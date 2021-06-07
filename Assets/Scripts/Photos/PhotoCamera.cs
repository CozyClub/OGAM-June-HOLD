using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PhotoCamera : MonoBehaviour
{
    public int photoMaxPerDay = 3; // low default num for testing

    /// <summary>
    /// Photo directory configuration - can make this configurable in the future
    /// </summary>
    public readonly string defaultPhotoDirectoryPath = "/SavedPhotos/";
    public PhotoFileFormat defaultPhotoFileFormat = PhotoFileFormat.PNG;

    KeyCode StartPhotoModeKey = KeyCode.Space;
    KeyCode TakePhotoKey = KeyCode.Mouse0;
    KeyCode ExitPhotoModeKey = KeyCode.Mouse1;

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
        StartPhotoModeKey = keys.startPhotoModeKey.key;
        TakePhotoKey = keys.takePhotoKey.key;
        ExitPhotoModeKey = keys.exitPhotoModeKey.key;

        photo.SetActive(false);
        Camera = GetComponent<Camera>();
        photoFrame.SetActive(false);

        SetupPhotoDirectory();
        PhotoCollectionDTO.LoadData();
    }

    private void LateUpdate()
    {
        this.EvaluateCameraControls();
    }

    private void EvaluateCameraControls()
    {
        // Show photo frame when StartPhotoModeKey is pressed
        if (Input.GetKeyDown(StartPhotoModeKey))
        {
            if (!photoFrame.activeSelf)
            {
                photoFrame.SetActive(true);
            }
        }

        if (photoFrame.activeSelf)
        {
            // Release PhotoKey to take picture
            if (Input.GetKeyDown(TakePhotoKey))
            {
                Capture();
            }

            // Exit photo frame if PhotoKeyCancel is pressed
            if (Input.GetKeyDown(ExitPhotoModeKey))
            {
                Debug.Log("Exiting photo frame");
                photoFrame.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Performs photo capture and save.
    /// </summary>
    public void Capture()
    {
        // Deactivates photo frame used for previewing photo before capture.
        photoFrame.SetActive(false);

        // Disallows photo capture if current photo count exceeds max.
        // To do - give warning that max photos have been taken.
        if (PhotoCollectionDTO.GetPhotoCount() >= photoMaxPerDay)
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
        PhotoDTO photo = new PhotoDTO(bytes, defaultPhotoFileFormat);

        // Get any capturable objects identified in the camera raycast
        Transform transform = Camera.transform;
        LayerMask mask = LayerMask.GetMask("Capturable");

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 20.0f, mask))
        {
            Capturable capturableComponent = hit.transform.gameObject.GetComponent<Capturable>();
            photo.AddIdentifiableObject(capturableComponent);
        }

        // Adding photo collection and saving. SaveData() saves the whole collection so we should probably move this out somewhere else.
        PhotoCollectionDTO.AddPhoto(photo);
        PhotoCollectionDTO.SaveData();

        StartCoroutine(this.DisplayPhoto());
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
        string photoDirectoryPath = this.GetPhotoDirectoryPath();
        Debug.Log("Photo directory path is: " + photoDirectoryPath);

        DirectoryInfo dir = new DirectoryInfo(photoDirectoryPath);
        if (!dir.Exists)
        {
            Directory.CreateDirectory(photoDirectoryPath);
        }
    }

    private string GetPhotoDirectoryPath()
    {
        return $"{Application.persistentDataPath}{defaultPhotoDirectoryPath}";
    }
}
