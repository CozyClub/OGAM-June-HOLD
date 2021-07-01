using UnityEngine;

public class GameCursor : MonoBehaviour
{
    public Sprite cursor;
    public Sprite pointer;

    public static Texture2D cursorTexture;
    public static Texture2D pointerTexture;

    void Start()
    {
        cursorTexture = GetTexture(cursor);
        pointerTexture = GetTexture(pointer);

        SetCursorToDefault();
    }

    public static void SetCursorToDefault()
    {
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }

    public static void SetCursorToPointer()
    {
        Cursor.SetCursor(pointerTexture, Vector2.zero, CursorMode.Auto);
    }

    Texture2D GetTexture(Sprite sprite)
    {
        Texture2D texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height, TextureFormat.ARGB32, false);
        Color[] pixels = sprite.texture.GetPixels(
            (int)sprite.rect.x,
            (int)sprite.rect.y,
            (int)sprite.rect.width,
            (int)sprite.rect.height);

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
}
