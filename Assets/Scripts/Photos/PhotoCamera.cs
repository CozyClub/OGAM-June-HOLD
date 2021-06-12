using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class PhotoCamera : MonoBehaviour
{
    public PlayerMovement playerRef;
    public RenderTexture photoTexture;

    public int photoMaxPerDay = 10; // low default num for testing

    /// <summary>
    /// Photo directory configuration - can make this configurable in the future
    /// </summary>
    public readonly string defaultPhotoDirectoryPath = "/SavedPhotos/";
    public PhotoFileFormat defaultPhotoFileFormat = PhotoFileFormat.PNG;


    Texture2D image;
    public RawImage photoDisplay;
    GameObject photo;

    //This is just to see what you're aiming at when taking a picture
    public GameObject photoFrame;

    private CinemachineBrain cameraBrain;
    private CinemachineVirtualCamera cineCamera;

    public string PhotoDirectoryPath => GetPhotoDirectoryPath();

    public void Start()
    {
        photo = photoDisplay.gameObject.transform.parent.gameObject;

        photo.SetActive(false);
        cineCamera = GetComponent<CinemachineVirtualCamera>();
        cameraBrain = FindObjectOfType<CinemachineBrain>();
        photoFrame.SetActive(false);
        SetupPhotoDirectory();
        PhotoCollectionDTO.LoadData();
    }

    #region input controlled functions
    public void OpenCamera()
    {
        if (Time.timeScale == 0f || photoFrame.activeSelf) return;

        photoFrame.SetActive(true);
        cineCamera.Priority = 1000;
        playerRef.MovementMode = PlayerMovement.MovementType.MouseRotations;
    }

    public void CloseCamera()
    {
        if (Time.timeScale == 0f) return;

        photoFrame.SetActive(false);
        cineCamera.Priority = 5;
        playerRef.MovementMode = PlayerMovement.MovementType.LRRotations;
    }

    public void TakePicture()
    {
        if (Time.timeScale == 0f || !photoFrame.activeSelf) return;

        Capture();
    }
    #endregion

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
        PhotoDTO photo = new PhotoDTO(bytes, defaultPhotoFileFormat);

        IEnumerable<Capturable> capturables = GameObject.FindObjectsOfType<Capturable>();
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cameraBrain.OutputCamera);

        // We should create some kind of priority to grab like max 10 capturables - attempting to sort by distance from camera
        capturables = capturables.OrderBy(x => x.transform.position.z);
        IList<Plane> orderedPlanes = planes.OrderBy(x => x.distance).ToList();
        cameraBrain.OutputCamera.targetTexture = null;
        foreach (Capturable capturable in capturables)
        {
            if(capturable.IsVisibleOnCameraPlanes(orderedPlanes))
            {
                photo.AddIdentifiableObject(capturable);
            }
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

        CloseCamera();
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
