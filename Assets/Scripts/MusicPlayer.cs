using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip[] songSelection;
    AudioSource src;

    private void Awake()
    {
        src = GetComponent<AudioSource>();
        int currentSong = Random.Range(0, songSelection.Length);

        src.clip = songSelection[currentSong];
        src.Play();
    }
}
