public static class PhotoExtensions
{
    public static string ToFileExtension(this PhotoFileFormat photoFileFormat)
    {
        switch (photoFileFormat)
        {
            case PhotoFileFormat.JPEG:
                {
                    return "jpg";
                }
            case PhotoFileFormat.PNG:
            default:
                {
                    return "png";
                }
        }
    }
}
