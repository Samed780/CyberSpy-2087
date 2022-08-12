using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource bg_Music;

    public AudioSource[] SFXs;

    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StopBGMusic()
    {
        bg_Music.Stop();
    }
    
    public void PlaySFX(int index)
    {
        SFXs[index].Stop();
        SFXs[index].Play();
    }
}
