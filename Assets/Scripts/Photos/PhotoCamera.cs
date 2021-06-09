using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PhotoCamera : MonoBehaviour
{
    public int photoMaxPerDay = 10; // low default num for testing

    /// <summary>
    /// Photo directory configuration - can make this configurable in the future
    /// </summary>
    PhotoFileFormat defaultPhotoFileFormat = PhotoFileFormat.PNG;

    KeyCode StartPhotoModeKey = KeyCode.Space;
    KeyCode TakePhotoKey = KeyCode.Mouse0;
    KeyCode ExitPhotoModeKey = KeyCode.Mouse1;

    KeyCtrl keys;
    public RawImage photoDisplay;

    /// <summary>
    /// Photo image game object
    /// </summary>
    static GameObject photo;
    static Texture2D image;

    /// <summary>
    /// The photo frame to show where robot is aiming camera.
    /// </summary>
    public GameObject photoFrame;
    public Camera Camera;

    /// <summary>
    /// Image data hydrated after photo is taken.
    /// </summary>
    public static PhotoDTO imageData;

    public string PhotoDirectoryPath => PhotoCollectionDTO.GetPhotoDirectoryPath();

    public void Start()
    {
        photo = photoDisplay.gameObject.transform.parent.gameObject;

        // Configure game input keys
        keys = KeyCtrl.keyctrl;
        StartPhotoModeKey = keys.startPhotoModeKey.key;
        TakePhotoKey = keys.takePhotoKey.key;
        ExitPhotoModeKey = keys.exitPhotoModeKey.key;

        photo.SetActive(false);
        Camera = GetComponent<Camera>();
        photoFrame.SetActive(false);

        PhotoCollectionDTO.SetupPhotoDirectory();
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
                RenderCapture();
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
    public void RenderCapture()
    {
        // Deactivates photo frame used for previewing photo before capture.
        photoFrame.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

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

        PhotoDTO photoDTO = new PhotoDTO(bytes, defaultPhotoFileFormat);

        IEnumerable<Capturable> capturables = GameObject.FindObjectsOfType<Capturable>();
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera);

        // We should create some kind of priority to grab like max 10 capturables - attempting to sort by distance from camera
        capturables = capturables.OrderBy(x => x.transform.position.z);
        IList<Plane> orderedPlanes = planes.OrderBy(x => x.distance).ToList();

        foreach (Capturable capturable in capturables)
        {
            if (capturable.IsVisibleOnCameraPlanes(orderedPlanes))
            {
                photoDTO.AddIdentifiableObject(capturable);
            }
        }

        imageData = photoDTO;
        DisplayPhotoPreview();

    }

    void DisplayPhotoPreview()
    {
        // Display photo in frame
        photoDisplay.texture = image;
        photo.SetActive(true);
    }

    public static void FinishCamera()
    {
        photo.SetActive(false);
        Destroy(image);
    }
}
