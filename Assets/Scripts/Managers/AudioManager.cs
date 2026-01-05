using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource Music_Source;
    [SerializeField] private AudioSource SFX_Source;

    [Header("BGMusic Clips")]
    [SerializeField] private AudioClip BackgroundMusic;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip ButtonHover;
    [SerializeField] private AudioClip ButtonClick;

    // -----------------------------------------------------------

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }


    // ------------------------------------------------------------ 

    void Start()
    {
        // Hacems que empiece a sonar la muisca del Menu
        Music_Source.clip = BackgroundMusic;
        Music_Source.Play();
    }

    // -------------------------------------------------------------
    public void PlaySFX(AudioClip clip, float sfxVolume = 0.5f)
    {
        SFX_Source.PlayOneShot(clip, sfxVolume);
    }

    // -------------------------------------------------------------
    public void PlaySFX_BtnHover(float sfxVolume = 0.5f)
    {
        SFX_Source.PlayOneShot(ButtonHover, sfxVolume);
    }

    // -------------------------------------------------------------
    public void PlaySFX_BtnClicked(float sfxVolume = 0.5f)
    {
        SFX_Source.PlayOneShot(ButtonClick, sfxVolume);
    }

}
