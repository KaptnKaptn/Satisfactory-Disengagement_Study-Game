using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Components")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private SerializableDict audioClipsSetup;
    private Dictionary<string, AudioClip> sfxClips;
    public AudioClip backgroundMusic;

    // Start is called before the first frame update
    void Start()
    {
        musicSource.clip = backgroundMusic;
        musicSource.Play();

        sfxClips = audioClipsSetup.ToDictionary();
    }

    public void PlaySFX(string clipName)
    {
        AudioClip clip = sfxClips[clipName];
        sfxSource.PlayOneShot(clip);
    }

    public void LoopSFX(string clipName)
    {
        AudioClip clip = sfxClips[clipName];
        sfxSource.clip = clip;
        sfxSource.Play();
    }

    public void StopLoopSFX()
    {
        sfxSource.Stop();
    }

    public void ChangeBackgroundMusic(AudioClip clip)
    {
        musicSource.clip = clip;
    }

    public void StopBackgroundMusic()
    {
        musicSource.Stop();
    }
}

[Serializable]
public class SerializableDict
{
    [SerializeField] SerializableDictItem[] items;

    public Dictionary<string, AudioClip> ToDictionary()
    {
        Dictionary<string, AudioClip> newDict = new Dictionary<string, AudioClip>();

        foreach (var item in items)
        {
            newDict.Add(item.name, item.clip);
        }

        return newDict;
    }
}

[Serializable]
public class SerializableDictItem
{
    public string name;
    public AudioClip clip;
}
