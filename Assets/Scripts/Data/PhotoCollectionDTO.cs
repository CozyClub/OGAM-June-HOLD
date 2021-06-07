using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/// <summary>
/// Defines data for a photo
/// </summary>
[Serializable]
public class PhotoCollectionDTO
{
    private static readonly string collectionPath = $"{Application.persistentDataPath}/SavedPhotos/PhotoCollection.pd";

    static short count = 0;
    static IDictionary<Guid, PhotoDTO> photos = new Dictionary<Guid, PhotoDTO>();

    public static void AddPhoto(PhotoDTO photo)
    {
        Debug.Log($"Adding new photo with id: {photo.Id}.");
        photos.Add(new KeyValuePair<Guid, PhotoDTO>(photo.Id, photo));

        count = (short)photos.Count;
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

        using(FileStream file = File.Create(collectionPath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, photos);
            success = true;
        }

        return success;
    }

    public static void LoadData()
    {
        if (File.Exists(collectionPath))
        {
            using (FileStream fs = File.OpenRead(collectionPath))
            {
                BinaryFormatter bf = new BinaryFormatter();
                fs.Position = 0;
                photos = (Dictionary<Guid, PhotoDTO>)bf.Deserialize(fs);
            }
        }

        count = (short)photos.Count;
    }
}