using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.MainMenu
{
    public class PlayerListItem : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TMP_Text _playerNameText;

        private Player _player;

        public void SetUp(Player player)
        {
            _playerNameText.text = player.NickName;
            _player = player;
        }

        // As room creator.
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if(_player == otherPlayer)
                Destroy(gameObject);
        }

        // As room attendee.
        public override void OnLeftRoom()
        {
            Destroy(gameObject);
        }
    }
}
