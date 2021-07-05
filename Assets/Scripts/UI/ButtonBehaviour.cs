using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        GameCursor.SetCursorToPointer();
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        GameCursor.SetCursorToDefault();
    }
}