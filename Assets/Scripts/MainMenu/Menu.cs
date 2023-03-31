using UnityEngine;

namespace Assets.Scripts.MainMenu
{
    public class Menu : MonoBehaviour
    {
        public MenuType MenuType;
        [HideInInspector] public bool Opened;

        public void Open()
        {
            Opened = true;
            gameObject.SetActive(true);
        }

        public void Close()
        {
            Opened = false;
            gameObject.SetActive(false);
        }
    }

    public enum MenuType
    {
        Loading = 1,
        Title = 2,
        CreateRoom = 3,
        Room = 4,
        Error = 5,
        FindRoom = 6,
        StartRoom = 7,
        MapSelect = 8,
        Editor = 9,
        EditorMapSelect = 10,
    }
}