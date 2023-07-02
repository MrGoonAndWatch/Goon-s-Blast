using System;
using FMODUnity;
using UnityEngine;

public class GoonsBlastFmodAudioEvents : MonoBehaviour
{
    [Header("Sound Effects")]
    [SerializeField]
    private EventReference _explosionSound;
    [SerializeField]
    private EventReference _playerExplodeSound;

    [Header("Music")]
    [SerializeField]
    private EventReference[] _vsModeSongs;
    [SerializeField]
    private EventReference _menuSong;

    private static GoonsBlastFmodAudioEvents _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogWarning($"More than one instance of Fmod Audio Events found! Ignoring {gameObject.name}.");
            return;
        }

        _instance = this;
    }

    public static EventReference ExplosionSound => GetInstance()._explosionSound;
    public static EventReference PlayerExplodeSound => GetInstance()._playerExplodeSound;
    public static EventReference MenuSong => GetInstance()._menuSong;

    public static EventReference GetVsSong(int index)
    {
        if (index < 0 || GetInstance()._vsModeSongs.Length <= index)
        {
            Debug.LogError($"There is no song index {index}!");
            throw new IndexOutOfRangeException($"There is no song index {index}!");
        }

        return GetInstance()._vsModeSongs[index];
    }

    private static GoonsBlastFmodAudioEvents GetInstance()
    {
        if (_instance == null)
        {
            Debug.LogError("No GoonsBlastFmodAudioEvents instance found!");
            throw new NullReferenceException("No GoonsBlastFmodAudioEvents instance found!");
        }
        return _instance;
    }

}
