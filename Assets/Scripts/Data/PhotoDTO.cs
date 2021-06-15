using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PhotoDTO
{
    public PhotoDTO()
    {
        Id = Guid.NewGuid();
        PhotoData = new byte[0];
        UtcTimeStamp = DateTime.UtcNow;
        MainIdentifiableObjects = new List<CapturableDTO>();
    }

    public PhotoDTO(byte[] photoData)
    {
        Id = Guid.NewGuid();
        PhotoData = photoData;
        UtcTimeStamp = DateTime.UtcNow;
        MainIdentifiableObjects = new List<CapturableDTO>();
    }

    public Guid Id;
    public byte[] PhotoData;
    public DateTime UtcTimeStamp;
    IList<CapturableDTO> MainIdentifiableObjects;

    public void AddIdentifiableObject(Capturable capturable)
    {
        Debug.Log($"Adding an identifiable object to image metadata with the name: '{capturable.Name}'.");
        MainIdentifiableObjects.Add(new CapturableDTO(capturable));
    }
}