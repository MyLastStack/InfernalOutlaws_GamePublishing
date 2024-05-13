using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip[] songSelection;
    AudioSource src;

    public float fadeDuration = 2f;
    int currentSong = 0;

    [Range(0.01f, 1)] public float startVolume;

    public float fadeSpeed;
    public float textDuration;
    Timer textDurationTimer;

    public TextMeshProUGUI text;

    private void Awake()
    {
        src = GetComponent<AudioSource>();
        currentSong = Random.Range(0, songSelection.Length);

        src.clip = songSelection[currentSong];
        src.Play();

        text.text = "Now Playing - " + src.clip.name;
        textDurationTimer = new Timer(textDuration);
    }

    private void Update()
    {
        textDurationTimer.Tick(Time.unscaledDeltaTime);
        if (!textDurationTimer.IsDone())
        {
            text.alpha = Mathf.Clamp01(text.alpha + Time.unscaledDeltaTime * fadeSpeed);
            if (text.alpha >= 1)
            {
                textDurationTimer.Unpause();
            }
        }
        else if (textDurationTimer.IsDone())
        {
            text.alpha = Mathf.Clamp01(text.alpha - Time.unscaledDeltaTime * fadeSpeed);
        }

        if (src.isPlaying && src.time >= src.clip.length - fadeDuration)
        {
            float timeLeft = src.clip.length - src.time;
            src.volume = Mathf.Lerp(0, startVolume, timeLeft / fadeDuration);

            if (src.volume < 0.01f)
            {
                src.Stop();
                src.volume = startVolume;
                currentSong = Random.Range(0, songSelection.Length);
                src.clip = songSelection[currentSong];
                src.Play();

                text.text = "Now Playing - " + src.clip.name;
                textDurationTimer.Reset();
                textDurationTimer.Pause();
            }
        }
    }
}
