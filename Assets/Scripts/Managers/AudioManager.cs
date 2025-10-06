using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource musicSource;
    
    [Header("SFX Clips")]
    public AudioClip moonJudgmentSFX;
    public AudioClip phenomenonSFX;
    public AudioClip favorChangeSFX;
    public AudioClip buttonClickSFX;
    public AudioClip roundChangeSFX;
    
    [Header("Music")]
    public AudioClip ambientNightMusic;
    
    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float sfxVolume = 0.8f;
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;
    
    private void Start()
    {
        SetupAudioSources();
        PlayBackgroundMusic();
    }
    
    private void SetupAudioSources()
    {
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
            sfxSource.loop = false;
        }
        
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
            musicSource.loop = true;
        }
    }
    
    public void PlayMoonJudgmentSFX()
    {
        if (moonJudgmentSFX != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(moonJudgmentSFX, sfxVolume);
        }
    }
    
    public void PlayPhenomenonSFX()
    {
        if (phenomenonSFX != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(phenomenonSFX, sfxVolume);
        }
    }
    
    public void PlayFavorChangeSFX()
    {
        if (favorChangeSFX != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(favorChangeSFX, sfxVolume * 0.7f);
        }
    }
    
    public void PlayButtonClickSFX()
    {
        if (buttonClickSFX != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(buttonClickSFX, sfxVolume * 0.5f);
        }
    }
    
    public void PlayRoundChangeSFX()
    {
        if (roundChangeSFX != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(roundChangeSFX, sfxVolume * 0.6f);
        }
    }
    
    private void PlayBackgroundMusic()
    {
        if (ambientNightMusic != null && musicSource != null)
        {
            musicSource.clip = ambientNightMusic;
            musicSource.Play();
        }
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
            sfxSource.volume = sfxVolume;
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
            musicSource.volume = musicVolume;
    }
    
    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }
    
    public void ResumeMusic()
    {
        if (musicSource != null && ambientNightMusic != null)
        {
            if (!musicSource.isPlaying)
                musicSource.Play();
        }
    }
    
    public void PlayCustomSFX(AudioClip clip, float volumeMultiplier = 1f)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume * volumeMultiplier);
        }
    }
}