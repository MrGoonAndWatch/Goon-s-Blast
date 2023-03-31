using System.IO;
using Assets.Scripts.Constants;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    private Canvas GameOverCanvas;
    private TMP_Text WinningPlayerDisplay;

    private PhotonView _photonView;
    private PlayerController[] _playerInstances;
    private bool _refreshPlayerList = true;
    private bool _matchEnded;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _playerInstances = new PlayerController[0];

        GameOverCanvas = FindObjectOfType<Canvas>();
        WinningPlayerDisplay = FindObjectOfType<WinningPlayerTextbox>()?.gameObject.GetComponent<TMP_Text>();
        if(GameOverCanvas != null) GameOverCanvas.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (_photonView.IsMine)
        {
            LoadLevel();
            CreateController();
        }
        if (PhotonNetwork.IsMasterClient)
            InitializeOnePerGameItems();
    }

    private void Update()
    {
        if (!_photonView.IsMine) return;
        if (_refreshPlayerList) RefreshPlayerList();

        CheckForEndOfMatch();
    }

    private void LoadLevel()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            string levelDataJson;
            // TODO: Need support for campaign official maps.
            if (RoomManager.IsOfficialMap())
                levelDataJson = Resources.Load(Path.Join(GameConstants.LevelFilePaths.VsLevelResourceFolderPath, RoomManager.GetMap())).ToString();
            else
                levelDataJson = File.ReadAllText(RoomManager.GetMap());
            _photonView.RPC(nameof(LoadLevelFromData), RpcTarget.OthersBuffered, levelDataJson);
            // TODO: Make this a coroutine or async.
            LoadLevel(levelDataJson);
        }
    }

    [PunRPC]
    private void LoadLevelFromData(string levelDataJson)
    {
        // TODO: Make this a coroutine or async.
        LoadLevel(levelDataJson);
    }

    private void LoadLevel(string levelDataJson)
    {
        var levelData = JsonConvert.DeserializeObject<LevelData>(levelDataJson);
        var levelLoader = FindObjectOfType<LevelLoader>();
        if (levelLoader == null)
            Debug.LogError("Could not find a LevelLoader in the Game scene, cannot load the game!");
        else
            levelLoader.LoadLevel(levelData);
    }
    
    private void RefreshPlayerList()
    {
        var numPlayers = (int)PhotonNetwork.CurrentRoom.PlayerCount;
        var playersFound = FindObjectsOfType<PlayerController>();
        if (playersFound.Length != numPlayers) return;
        _playerInstances = playersFound;
        _refreshPlayerList = false;
    }

    // TODO: Alternate method for co-op mode where ends on win condition or all players dead.
    private void CheckForEndOfMatch()
    {
        if (_refreshPlayerList || _matchEnded || _playerInstances.Length <= 1) return;

        var playersRemaining = 0;
        var lastAliveName = "";
        for (var i = 0; i < _playerInstances.Length; i++)
        {
            if (_playerInstances[i].IsAlive())
            {
                playersRemaining++;
                lastAliveName = _playerInstances[i].GetName();
            }
        }

        if (playersRemaining <= 1)
        {
            var winningMessage = string.IsNullOrEmpty(lastAliveName) ? "DRAW" : $"{lastAliveName} WINS!";
            EndMatch(winningMessage);
        }
    }

    private void EndMatch(string endingMessage)
    {
        if (_matchEnded) return;

        for (var i = 0; i < _playerInstances.Length; i++)
        {
            _playerInstances[i].EndMatch();
        }
        
        if (WinningPlayerDisplay != null)
            WinningPlayerDisplay.text = endingMessage;
        if(GameOverCanvas != null)
            GameOverCanvas.gameObject.SetActive(true);
    }

    private void CreateController()
    {
        Debug.Log("Instantiated Player");
        var spawnPoint = GetSpawnPoint();
        PhotonNetwork.Instantiate(GameConstants.SpawnablePrefabs.PlayerController, spawnPoint, Quaternion.identity);
    }

    private static Vector3 GetSpawnPoint()
    {
        var spawnPoints = FindObjectsOfType<PlayerSpawn>();
        Vector3 spawnPoint;
        if (spawnPoints.Length == 0)
            spawnPoint = Vector3.zero;
        // TODO: More robust spawn point selection, try not to pick the same one for multiple players unless can't find a free one.
        else
            spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
        return spawnPoint;
    }

    private void InitializeOnePerGameItems()
    {
        var spawner = PhotonNetwork.Instantiate(GameConstants.SpawnablePrefabs.PowerupSpawner, Vector3.zero, Quaternion.identity).GetComponent<PowerupSpawner>();
        var destructables = FindObjectsOfType<Destructable>();
        for (var i = 0; i < destructables.Length; i++)
            destructables[i].SetPowerupSpawner(spawner);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        _refreshPlayerList = true;
    }

    public override void OnPlayerLeftRoom(Player oldPlayer)
    {
        _refreshPlayerList = true;
    }
}
