using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>
{
    public AudioSource musicSource;

    public void PlaySfxRequest(string name)
    {
        var audio = Resources.Load<AudioClip>($"SFX/{name}");
        if (audio == null)
            return;
        AudioSource.PlayClipAtPoint(audio, Camera.main.transform.position);
    }

    public void PlayMusicRequest(string name)
    {
        var audio = Resources.Load<AudioClip>($"Music/{name}");
        if (audio == null)
            return;
        if (musicSource.clip != audio)
        {
            musicSource.clip = audio;
            musicSource.Play();
        }
    }
}
