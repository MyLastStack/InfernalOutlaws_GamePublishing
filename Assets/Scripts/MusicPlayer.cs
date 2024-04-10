using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip[] songSelection;
    AudioSource src;

    public float fadeDuration = 2f;
    int currentSong = 0;

    [Range(0.01f, 1)] public float startVolume;

    private void Awake()
    {
        src = GetComponent<AudioSource>();
        currentSong = Random.Range(0, songSelection.Length);

        src.clip = songSelection[currentSong];
        src.Play();
    }

    private void Update()
    {
        if(src.isPlaying && src.time >= src.clip.length - fadeDuration)
        {
            float timeLeft = src.clip.length - src.time;
            src.volume = Mathf.Lerp(0, startVolume, timeLeft / fadeDuration);

            if(src.volume < 0.01f)
            {
                src.Stop();
                src.volume = startVolume;
                currentSong = Random.Range(0, songSelection.Length);
                src.clip = songSelection[currentSong];
                src.Play();
            }
        }
    }
}
