using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] musicTracks; // ۵ آهنگ مختلف
    private int currentTrack = 0;

    void Start()
    {
        PlayMusic();
    }

    void PlayMusic()
    {
        if (musicTracks.Length > 0)
        {
            audioSource.clip = musicTracks[currentTrack];
            audioSource.Play();
        }
    }

    public void NextTrack()
    {
        currentTrack = (currentTrack + 1) % musicTracks.Length;
        PlayMusic();
    }
}
