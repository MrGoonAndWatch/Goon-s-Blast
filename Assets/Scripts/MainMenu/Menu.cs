using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.MainMenu
{
    public class Menu : MonoBehaviour
    {
        public MenuType MenuType;
        [HideInInspector] public bool Opened;

        [SerializeField] private EventSystem _eventSystem;
        [SerializeField] private GameObject _selectOnOpen;

        public void Open()
        {
            Opened = true;
            gameObject.SetActive(true);
            
            _eventSystem.SetSelectedGameObject(_selectOnOpen);
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
        SettingsMenu = 11,
        ControlsMenu = 12,
        MatchSettingsMenu = 13
    }
}