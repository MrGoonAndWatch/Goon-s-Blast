using Photon.Pun;
using Assets.Scripts.Constants;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

    private string _selectedMapFilepath;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning($"Found a second RoomManger instance, destroying original instance with instance id {Instance.GetInstanceID()}");
            Destroy(Instance.gameObject);
        }

        DontDestroyOnLoad(gameObject);
        Instance = this;
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

    public static void ClearMap()
    {
        SetMap("");
    }

    public static void SetMap(string mapFilepath)
    {
        if (Instance == null)
        {
            Debug.LogError("Cannot SetMap. No RoomManager instance found!");
            return;
        }
        Instance._selectedMapFilepath = mapFilepath;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == (int) GameConstants.LevelIndexes.Game)
        {
            PhotonNetwork.Instantiate(GameConstants.SpawnablePrefabs.PlayerManager, Vector3.zero, Quaternion.identity);
        }
    }
}
