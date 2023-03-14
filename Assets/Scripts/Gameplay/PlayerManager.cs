using System.IO;
using Assets.Scripts.Constants;
using Newtonsoft.Json;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerManager : MonoBehaviour
{
    private Canvas GameOverCanvas;
    private TMP_Text WinningPlayerDisplay;

    private PhotonView _photonView;
    private PlayerController[] _playerInstances;
    private bool _initializedPlayerList;
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
        if (!_initializedPlayerList) RefreshPlayerList();

        CheckForEndOfMatch();
    }

    private void LoadLevel()
    {
        // TODO: Get this data in some network friendly way (and not hard coded!).
        // Clients all download to /DownloadedLevels/ folder then load from there if not host?
        var levelDataJson = File.ReadAllText(Path.Join(Application.persistentDataPath, "CustomLevels", "TestLevel.level"));
        var levelData = JsonConvert.DeserializeObject<LevelData>(levelDataJson);
        var levelLoader = FindObjectOfType<LevelLoader>();
        if (levelLoader == null)
            Debug.LogError("Could not find a LevelLoader in the Game scene, cannot load the game!");
        else
            levelLoader.LoadLevel(levelData);
    }

    // TODO: Call this when new players join the room (if that's possible).
    private void RefreshPlayerList()
    {
        var numPlayers = (int)PhotonNetwork.CurrentRoom.PlayerCount;
        var playersFound = FindObjectsOfType<PlayerController>();
        if (playersFound.Length != numPlayers) return;
        _playerInstances = playersFound;
        _initializedPlayerList = true;
    }

    // TODO: Alternate method for co-op mode where ends on win condition or all players dead.
    private void CheckForEndOfMatch()
    {
        if (_matchEnded) return;
        if (_playerInstances.Length <= 1) return;

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
}
