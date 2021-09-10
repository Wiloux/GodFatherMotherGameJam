using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource sfxsd;
    public AudioSource musicsd;

    public static SoundManager Instance;

    public AudioClip Breaksfx;

    private void Awake()
    {
        Instance = this;
    }

    public void PauseUnPauseMusic(bool pause = false)
    {
        if (pause)
        {
            musicsd.Pause();
        }
        else
        {
            musicsd.UnPause();
        }
    }
    public void PlaySoundEffect(AudioClip clip)
    {
        sfxsd.pitch = Random.Range(0.9f, 1.1f);
        sfxsd.PlayOneShot(clip);
    }

    public void PlaySoundEffectList(List<AudioClip> clips)
    {
        sfxsd.pitch = Random.Range(0.9f, 1.1f);
        sfxsd.PlayOneShot(clips[Random.Range(0, clips.Count)]);
    }

    public void PlayUISoundEffect(AudioClip clip)
    {
        sfxsd.PlayOneShot(clip);
    }
}
