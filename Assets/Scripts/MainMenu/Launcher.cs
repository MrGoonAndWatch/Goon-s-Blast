using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Constants;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.MainMenu
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        public static Launcher Instance;

        [SerializeField] private TMP_InputField _roomNameInputField;
        [SerializeField] private TMP_Text _errorDisplay;
        [SerializeField] private TMP_Text _roomNameDisplay;
        [SerializeField] private Transform _roomListContent;
        [SerializeField] private GameObject _roomListItemPrefab;
        [SerializeField] private Transform _playerListContent;
        [SerializeField] private GameObject _playerListItemPrefab;
        [SerializeField] private TMP_InputField _usernameInput;
        [SerializeField] private GameObject _startGameButton;
        [SerializeField] private Transform _mapList;
        [SerializeField] private Transform _mapEditList;
        [SerializeField] private GameObject _mapSelectPrefab;
        [SerializeField] private TMP_Text _selectedMapLabel;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning($"A second instance on the Launcher script was found! Destroying the original instance with id {Instance.GetInstanceID()}...");
                Destroy(Instance.gameObject);
            }

            Instance = this;
            RefreshSelectedMap("(select a map)");

            // TODO: Save username and load in from file here first.
            OnChangeUsername();
        }

        private void Start()
        {
            MenuManager.Instance.OpenMenu(MenuType.Loading);
            Debug.Log("Starting Launcher");
            // Connects to Photon master server using /Photon/PhotonUnityNetworking/Resources/PhotonServerSettings
            PhotonNetwork.ConnectUsingSettings();
            StartCoroutine(StartRefreshLevelList());
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("On Connected To Master");
            // Need to be in lobby to find/create rooms.
            PhotonNetwork.JoinLobby();
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("Joined Lobby");
            MenuManager.Instance.OpenMenu(MenuType.Title);
        }

        public void CreateRoom()
        {
            if (string.IsNullOrEmpty(_roomNameInputField?.text))
            {
                Debug.Log("Room name was null/empty! Not creating room!");
                return;
            }

            if (string.IsNullOrEmpty(RoomManager.GetMap()))
            {
                Debug.Log("No map selected! Not creating room!");
                return;
            }
            MenuManager.Instance.OpenMenu(MenuType.Loading);
            PhotonNetwork.CreateRoom(_roomNameInputField.text);
        }

        // Also called on create room, but OnJoinedRoom is called in both cases.
        /*
        public override void OnCreatedRoom()
        {
        }
        */

        public override void OnJoinedRoom()
        {
            _roomNameDisplay.text = $"Room Name: {PhotonNetwork.CurrentRoom.Name}";

            for(var i = _playerListContent.childCount - 1; i >= 0; i--)
                Destroy(_playerListContent.GetChild(i).gameObject);
            var playersInRoom = PhotonNetwork.PlayerList;
            for (var j = 0; j < playersInRoom.Length; j++)
                OnPlayerEnteredRoom(playersInRoom[j]);

            RefreshStartGameButton();

            MenuManager.Instance.OpenMenu(MenuType.Room);
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            RefreshStartGameButton();
        }

        private void RefreshStartGameButton()
        {
            _startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogError($"Failed to create room w/ returnCode = '{returnCode}', message = {message}");
            _errorDisplay.text = $"Failed to create room: {message}";
            MenuManager.Instance.OpenMenu(MenuType.Error);
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
            MenuManager.Instance.OpenMenu(MenuType.Loading);
        }

        public override void OnLeftRoom()
        {
            MenuManager.Instance.OpenMenu(MenuType.Title);
        }

        public void ExitGame()
        {
            // TODO: Something better than this xD
            Application.Quit();
        }

        public void OnFindRooms()
        {
            PhotonNetwork.GetCustomRoomList(TypedLobby.Default, "");
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            for (var i = _roomListContent.childCount - 1; i >= 0; i--)
                Destroy(_roomListContent.GetChild(i).gameObject);

            for (var j = 0; j < roomList.Count; j++)
            {
                if (roomList[j].RemovedFromList)
                    continue;
                var item = Instantiate(_roomListItemPrefab, _roomListContent).GetComponent<RoomListItem>();
                item.SetUp(roomList[j]);
            }
        }

        private IEnumerator StartRefreshLevelList()
        {
            RefreshLevelList();
            yield return null;
        }

        public static void RefreshSelectedMap(string displayText)
        {
            if (Instance == null)
            {
                Debug.LogError("Cannot refresh selected map, Launcher instance is null.");
                return;
            }

            Instance._selectedMapLabel.text = displayText;
        }

        private void RefreshLevelList()
        {
            // TODO: Add folder support somehow... At a min need official files vs custom levels.
            var customLevelDir = Path.Join(Application.persistentDataPath, "CustomLevels");
            if (!Directory.Exists(customLevelDir))
            {
                Debug.LogError($"Couldn't load levels... Directory '{customLevelDir}' does not exist.");
                return;
            }

            for (var i = _mapList.childCount - 1; i >= 0; i--)
                Destroy(_mapList.GetChild(i).gameObject);
            for (var i = _mapEditList.childCount - 1; i >= 0; i--)
                Destroy(_mapEditList.GetChild(i).gameObject);

            var customLevels = Directory.GetFiles(customLevelDir);
            for (var i = 0; i < customLevels.Length; i++)
            {
                var displayName = Path.GetFileName(customLevels[i]);
                var mapSelectItem = Instantiate(_mapSelectPrefab, _mapList).GetComponent<MapSelectItem>();
                mapSelectItem.SetUp(customLevels[i], displayName, false);
                var editMapSelectItem = Instantiate(_mapSelectPrefab, _mapEditList).GetComponent<MapSelectItem>();
                editMapSelectItem.SetUp(customLevels[i], displayName, true);
            }
        }

        public void JoinRoom(RoomInfo roomInfo)
        {
            PhotonNetwork.JoinRoom(roomInfo.Name);
            MenuManager.Instance.OpenMenu(MenuType.Loading);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            var item = Instantiate(_playerListItemPrefab, _playerListContent).GetComponent<PlayerListItem>();
            item.SetUp(newPlayer);
        }

        public void OnChangeUsername()
        {
            if (string.IsNullOrEmpty(_usernameInput.text))
                return;
            PhotonNetwork.NickName = _usernameInput.text;
        }

        public void OnLevelEditorClick()
        {
            RoomManager.ClearMap();
            PhotonNetwork.Disconnect();
        }

        public static void OpenEditor()
        {
            PhotonNetwork.Disconnect();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            // TODO: This is a horrible assumption, make this logic better!
            if(cause == DisconnectCause.DisconnectByClientLogic)
                SceneManager.LoadScene((int)GameConstants.LevelIndexes.Editor);
        }

        public void StartGame()
        {
            PhotonNetwork.LoadLevel((int)GameConstants.LevelIndexes.Game);
        }
    }
}
