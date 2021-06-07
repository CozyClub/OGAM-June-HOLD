using System.Collections.Generic;
using UnityEngine;

public class KeyCtrl : MonoBehaviour
{
    public static KeyCtrl keyctrl;
    
    public KeyWithName startPhotoModeKey;
    public KeyWithName takePhotoKey;
    public KeyWithName exitPhotoModeKey;
    public KeyWithName interact;
    public KeyWithName secondaryInteract;
    public KeyWithName menu;
    public KeyWithName mainScrollLeft;
    public KeyWithName mainScrollRight;
    public KeyWithName secondScrollLeft;
    public KeyWithName secondScrollRight;
    public KeyWithName quit;
    public List<KeyWithName> AllCommands = new List<KeyWithName>();

    private void Awake()
    {
        if (keyctrl == null)
        {
            keyctrl = this;
        }
        else if (keyctrl != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        AllCommands.AddMore(startPhotoModeKey, takePhotoKey, exitPhotoModeKey, interact, secondaryInteract, menu, mainScrollLeft, mainScrollRight, secondScrollRight, secondScrollLeft, menu, quit);
    }
    
}

//Fixa nar jag har tid, fult nu
 public static class ListExtenstions
{
    public static void AddMore<T>(this List<T> list, params T[] stuff)
    {
        list.AddRange(stuff);
    }
}

[System.Serializable]
public class KeyWithName
{
    public string name;
    public KeyCode key;
}

