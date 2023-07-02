using System;
using System.Collections.Generic;
using Assets.Scripts.Constants;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class GoonsBlastAudioManager : MonoBehaviour
{

    private static GoonsBlastAudioManager _instance;

    private bool _initializedMusic;
    private EventInstance _musicInstance;
    private List<EventInstance> _eventInstances;

    [Range(0,1)]
    public float masterVolume = 0.5f;
    [Range(0, 1)]
    public float musicVolume = 0.5f;
    [Range(0,1)]
    public float sfxVolume = 0.5f;

    private Bus _masterBus;
    private Bus _musicBus;
    private Bus _sfxBus;

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogWarning($"More than one AudioManager instance found! Ignoring instance on {gameObject.name}.");
            return;
        }

        _instance = this;
        _eventInstances = new List<EventInstance>();

        _masterBus = RuntimeManager.GetBus("bus:/");
        _musicBus = RuntimeManager.GetBus("bus:/Music");
        _sfxBus = RuntimeManager.GetBus("bus:/SFX");
    }

    private void OnDestroy()
    {
        CleanUp();
    }

    public static void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    private static void InitializeMusic(EventReference song)
    {
        var instance = GetInstance();
        instance._initializedMusic = true;
        instance._musicInstance = CreateEventInstance(song);
        instance._musicInstance.start();
    }

    public static void ChangeMusic(EventReference newSong)
    {
        var instance = GetInstance();
        if (!instance._initializedMusic)
        {
            InitializeMusic(newSong);
            return;
        }

        instance._musicInstance.stop(STOP_MODE.IMMEDIATE);
        instance._musicInstance.release();
        instance._eventInstances.Remove(instance._musicInstance);
        instance._musicInstance = CreateEventInstance(newSong);
        instance._musicInstance.start();
    }

    public static EventInstance CreateEventInstance(EventReference eventReference)
    {
        var eventInstance = RuntimeManager.CreateInstance(eventReference);
        GetInstance()._eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public static void UpdateVolume(GameConstants.ConfigSettings settings)
    {
        var instance = GetInstance();
        if (settings.SoundMasterVolume.HasValue)
            instance._masterBus.setVolume(settings.SoundMasterVolume.Value);
        if (settings.SoundMusicVolume.HasValue)
            instance._musicBus.setVolume(settings.SoundMusicVolume.Value);
        if (settings.SoundSfxVolume.HasValue)
            instance._sfxBus.setVolume(settings.SoundSfxVolume.Value);
    }

    private static GoonsBlastAudioManager GetInstance()
    {
        if (_instance == null)
        {
            Debug.LogError("No GoonsBlastAudioManager instance found!");
            throw new NullReferenceException("No GoonsBlastAudioManager instance found!");
        }
        return _instance;
    }

    private void CleanUp()
    {
        foreach (var eventInstance in _eventInstances)
        {
            eventInstance.stop(STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }

        _musicInstance.stop(STOP_MODE.IMMEDIATE);
        _musicInstance.release();
    }
}
