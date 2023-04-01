using System.IO;
using Photon.Pun;
using Assets.Scripts.Constants;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

    private string _selectedMapFilepath;
    private bool _officialMap;
    private GameConstants.ConfigSettings _configSettings;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning($"Found a second RoomManger instance, destroying original instance with instance id {Instance.GetInstanceID()}");
            Destroy(Instance.gameObject);
        }

        LoadSettings();

        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    private static string GetConfigSettingsFilepath()
    {
        return Path.Join(Application.persistentDataPath, GameConstants.ConfigSettingsFilename);
    }

    private void LoadSettings()
    {
        var configSettingsFilepath = GetConfigSettingsFilepath();
        if (File.Exists(configSettingsFilepath))
        {
            var configJson = File.ReadAllText(configSettingsFilepath);
            _configSettings = JsonConvert.DeserializeObject<GameConstants.ConfigSettings>(configJson);
        }
        else
            _configSettings = new GameConstants.ConfigSettings
            {
                Username = "Tony Swan"
            };
    }

    public static GameConstants.ConfigSettings GetConfigSettings()
    {
        if (Instance == null)
        {
            Debug.LogError("Cannot get config settings, no RoomManager instance found!");
            return null;
        }

        return Instance._configSettings;
    }

    public static void SaveSettings(GameConstants.ConfigSettings newSettings)
    {
        if (Instance == null)
            Debug.LogError("Cannot update settings data, no RoomManager instance was found!");
        else
            Instance._configSettings = newSettings;

        var configSettingsFilepath = GetConfigSettingsFilepath();
        
        var configJson = JsonConvert.SerializeObject(newSettings, Formatting.None);
        File.WriteAllText(configSettingsFilepath, configJson);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public static string GetMap()
    {
        if (Instance == null)
        {
            Debug.LogError("Cannot GetMap. No RoomManager instance found!");
            return null;
        }

        return Instance._selectedMapFilepath;
    }

    public static bool IsOfficialMap()
    {
        if (Instance == null)
        {
            Debug.LogError("Cannot Check if IsOfficialMap. No RoomManager instance found!");
            return false;
        }

        return Instance._officialMap;
    }

    public static void ClearMap()
    {
        SetMap("", false);
    }

    public static void SetMap(string mapFilepath, bool officialMap)
    {
        if (Instance == null)
        {
            Debug.LogError("Cannot SetMap. No RoomManager instance found!");
            return;
        }
        Instance._selectedMapFilepath = mapFilepath;
        Instance._officialMap = officialMap;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == (int) GameConstants.LevelIndexes.Game)
        {
            PhotonNetwork.Instantiate(GameConstants.SpawnablePrefabs.PlayerManager, Vector3.zero, Quaternion.identity);
        }
    }
}
