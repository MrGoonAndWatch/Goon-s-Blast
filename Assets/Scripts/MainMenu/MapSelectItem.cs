using Assets.Scripts.MainMenu;
using TMPro;
using UnityEngine;

public class MapSelectItem : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _roomNameText;
    
    private bool _forEditor;
    private string _filepath;

    public void SetUp(string filepath, string displayName, bool forEditor)
    {
        _filepath = filepath;
        // TODO: Read file here then set game type via level data to replace hard coded 'VS'!
        _roomNameText.text = $"VS - {displayName}";
        _forEditor = forEditor;
    }

    public void OnClick()
    {
        RoomManager.SetMap(_filepath);
        if (_forEditor)
        {
            Launcher.OpenEditor();
        }
        else
        {
            Launcher.RefreshSelectedMap(_roomNameText.text);
            MenuManager.Instance.OpenMenu(MenuType.CreateRoom);
        }
    }
}
