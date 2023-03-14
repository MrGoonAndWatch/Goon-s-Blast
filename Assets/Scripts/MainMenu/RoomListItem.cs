using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.MainMenu
{
    public class RoomListItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text _roomNameText;

        private RoomInfo _roomInfo;

        public void SetUp(RoomInfo roomInfo)
        {
            _roomNameText.text = roomInfo.Name;
            _roomInfo = roomInfo;
        }

        public void OnClick()
        {
            Launcher.Instance.JoinRoom(_roomInfo);
        }
    }
}
