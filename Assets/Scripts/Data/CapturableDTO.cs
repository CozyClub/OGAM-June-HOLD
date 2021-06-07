using System;

[Serializable]
public class CapturableDTO
{
    public CapturableDTO(Capturable capturable)
    {
        Name = capturable.Name;
        CapturableType = capturable.CapturableType;
    }

    public string Name;
    public CapturableType CapturableType;
}

[Serializable]
public enum CapturableType
{
    None = 0,
    Flora = 1,
    Fauna = 2,
    Person = 3,
    Landscape = 4
}
