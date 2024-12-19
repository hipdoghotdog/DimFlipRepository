using System;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Sound
{
    Step,
    Lever,
    Push,
    ArtifactTalk,
}

public class SoundManager : MonoBehaviour
{
    public AudioClip[] audioClips; // Array for sound effects
    public AudioClip backgroundMusic; // The background music clip

    public static SoundManager Instance;

    private AudioSource _soundEffectsSource; // For sound effects
    private AudioSource _musicSource; // For background music

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Add two AudioSources dynamically
            _soundEffectsSource = gameObject.AddComponent<AudioSource>();
            _musicSource = gameObject.AddComponent<AudioSource>();

            // Configure musicSource for looping background music
            _musicSource.loop = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayBackgroundMusic();
    }

    public void PlaySound(Sound sound, float volume = 1f)
    {
        switch (sound)
        {
            case Sound.Step:
                int randSoundNumber = Random.Range(0, 3);
                _soundEffectsSource.PlayOneShot(audioClips[randSoundNumber], volume);
                break;
            case Sound.Lever:
                _soundEffectsSource.PlayOneShot(audioClips[3], volume);
                break;
            case Sound.Push:
                _soundEffectsSource.PlayOneShot(audioClips[4], volume);
                break;
            case Sound.ArtifactTalk:
                int randSoundNumber2 = Random.Range(5, 9);
                _soundEffectsSource.PlayOneShot(audioClips[randSoundNumber2], volume);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(sound), sound, null);
        }
    }

    private void PlayBackgroundMusic(float volume = 0.5f)
    {
        if (backgroundMusic == null)
        {
            Debug.LogWarning("SoundManager: Background music clip is not assigned.");
            return;
        }

        _musicSource.clip = backgroundMusic;
        _musicSource.volume = volume;
        _musicSource.Play();
    }

    public void StopBackgroundMusic()
    {
        _musicSource.Stop();
    }
}
