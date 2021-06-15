using Cinemachine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class PhotoCamera : MonoBehaviour
{
    public PlayerMovement playerRef;
    public RenderTexture photoTexture;

    int photoMaxPerDay = 12;

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

    private CinemachineBrain cameraBrain;
    private CinemachineVirtualCamera cineCamera;

    /// <summary>
    /// Image data hydrated after photo is taken.
    /// </summary>
    public static PhotoDTO imageData;

    public string PhotoDirectoryPath => PhotoCollectionDTO.GetPhotoDirectoryPath();

    public void Start()
    {
        photo = photoDisplay.gameObject.transform.parent.gameObject;

        photo.SetActive(false);
        cineCamera = GetComponent<CinemachineVirtualCamera>();
        cameraBrain = FindObjectOfType<CinemachineBrain>();
        photoFrame.SetActive(false);

        PhotoCollectionDTO.SetupPhotoDirectory();
        PhotoCollectionDTO.LoadData();
    }

    #region input controlled functions
    public void OpenCamera()
    {
        if (TimeManager.IsGamePaused || photoFrame.activeSelf) return;

        photoFrame.SetActive(true);
        cineCamera.Priority = 1000;
        playerRef.MovementMode = PlayerMovement.MovementType.MouseRotations;
    }

    public void TakePicture()
    {
        if (TimeManager.IsGamePaused || !photoFrame.activeSelf) return;

        RenderCapture();
    }
    #endregion

    /// <summary>
    /// Performs photo capture and save.
    /// </summary>
    void RenderCapture()
    {
        // Deactivates photo frame used for previewing photo before capture.
        photoFrame.SetActive(false);

        // Disallows photo capture if current photo count exceeds max.
        // To do - give warning that max photos have been taken.
        if (PhotoCollectionDTO.GetPhotoCount() >= photoMaxPerDay)
        {
            Debug.LogWarning("Max photos taken, returning.");
            // closing camera for now?
            CloseCamera();
            return;
        }

        RenderTexture myRenderTexture = RenderTexture.active;
        cameraBrain.OutputCamera.targetTexture = photoTexture;
        RenderTexture.active = cameraBrain.OutputCamera.targetTexture;
        cameraBrain.OutputCamera.Render();

        // Create new texture to width and height of photo camera.
        image = new Texture2D(cameraBrain.OutputCamera.targetTexture.width, cameraBrain.OutputCamera.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, cameraBrain.OutputCamera.targetTexture.width, cameraBrain.OutputCamera.targetTexture.height), 0, 0);
        image.Apply();
        RenderTexture.active = myRenderTexture;

        byte[] bytes = image.EncodeToPNG();

        PhotoDTO photoDTO = new PhotoDTO(bytes);

        IEnumerable<Capturable> capturables = GameObject.FindObjectsOfType<Capturable>();
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cameraBrain.OutputCamera);

        // We should create some kind of priority to grab like max 10 capturables - attempting to sort by distance from camera
        capturables = capturables.OrderBy(x => x.transform.position.z);
        IList<Plane> orderedPlanes = planes.OrderBy(x => x.distance).ToList();
        cameraBrain.OutputCamera.targetTexture = null;
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
        TimeManager.PauseGame();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Display photo in frame
        photoDisplay.texture = image;
        photo.SetActive(true);
    }

    public void CloseCamera()
    {
        TimeManager.UnpauseGame();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        photo.SetActive(false);
        Destroy(image);

        photoFrame.SetActive(false);
        cineCamera.Priority = 5;
        playerRef.MovementMode = PlayerMovement.MovementType.LRRotations;
    }
}
