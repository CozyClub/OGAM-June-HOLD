using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/// <summary>
/// Defines data for a photo
/// </summary>
[Serializable]
public class PhotoCollectionDTO
{
    private static readonly string collectionPath = $"{Application.persistentDataPath}/SavedPhotos/PhotoCollection.pd";
    public static readonly string defaultPhotoDirectoryPath = "/SavedPhotos/";

    static short count = 0;
    static IDictionary<Guid, PhotoDTO> photos = new Dictionary<Guid, PhotoDTO>();

    public static void AddPhoto(PhotoDTO photo)
    {
        Debug.Log($"Adding new photo with id: {photo.Id}.");
        photos.Add(new KeyValuePair<Guid, PhotoDTO>(photo.Id, photo));

        count = (short)photos.Count();
    }

    public static PhotoDTO GetPhoto(Guid id)
    {
        // TODO error handling
        if (!photos.TryGetValue(id, out PhotoDTO photo)) return null;
        return photo;
    }

    public static IDictionary<Guid, PhotoDTO> GetPhotos()
    {
        return photos;
    }

    public static short GetPhotoCount()
    {
        return count;
    }

    public static bool SaveData()
    {
        bool success = false;

        using (FileStream file = File.Create(collectionPath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, photos);
            success = true;
        }

        return success;
    }

    public static IDictionary<Guid, PhotoDTO> LoadData()
    {
        Dictionary<Guid, PhotoDTO> photosFromSave = new Dictionary<Guid, PhotoDTO>();
        if (File.Exists(collectionPath))
        {
            using (FileStream fs = File.OpenRead(collectionPath))
            {
                BinaryFormatter bf = new BinaryFormatter();
                fs.Position = 0;
                photosFromSave = (Dictionary<Guid, PhotoDTO>)bf.Deserialize(fs);
            }
        }

        photos = photosFromSave;
        count = (short)photos.Count();

        return photosFromSave;
    }

    public static IEnumerable<PhotoDTO> PagePhotoCollection(int take = 0, int skip = 0)
    {
        IEnumerable<PhotoDTO> pagedPhotos = photos.Select(x => x.Value).OrderBy(x => x.UtcTimeStamp);

        if (skip > 0)
        {
            pagedPhotos = pagedPhotos.Skip(skip);
        }

        if (take > 0)
        {
            pagedPhotos = pagedPhotos.Take(take);
        }

        return pagedPhotos;
    }

    /// <summary>
    /// Initializes photo path directory if doesn't exist yet.
    /// </summary>
    public static void SetupPhotoDirectory()
    {
        // Set up photo directory
        string photoDirectoryPath = GetPhotoDirectoryPath();
        Debug.Log("Photo directory path is: " + photoDirectoryPath);

        DirectoryInfo dir = new DirectoryInfo(photoDirectoryPath);
        if (!dir.Exists)
        {
            Directory.CreateDirectory(photoDirectoryPath);
        }
    }

    public static string GetPhotoDirectoryPath()
    {
        return $"{Application.persistentDataPath}{defaultPhotoDirectoryPath}";
    }
}