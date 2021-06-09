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
        PhotoFileFormat = null;
        UtcTimeStamp = DateTime.UtcNow;
        MainIdentifiableObjects = new List<CapturableDTO>();
    }

    public PhotoDTO(byte[] photoData, PhotoFileFormat photoFileFormat)
    {
        Id = Guid.NewGuid();
        PhotoData = photoData;
        PhotoFileFormat = photoFileFormat;
        UtcTimeStamp = DateTime.UtcNow;
        MainIdentifiableObjects = new List<CapturableDTO>();
    }

    public Guid Id;
    public byte[] PhotoData;
    PhotoFileFormat? PhotoFileFormat;
    DateTime UtcTimeStamp;
    IList<CapturableDTO> MainIdentifiableObjects;

    public void AddIdentifiableObject(Capturable capturable)
    {
        Debug.Log($"Adding an identifiable object to image metadata with the name: '{capturable.Name}'.");
        MainIdentifiableObjects.Add(new CapturableDTO(capturable));
    }
}