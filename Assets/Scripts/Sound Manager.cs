using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Sound {
    STEP,
    LEVER,
    PUSH,
    ARTIFACT_TALK,
}

public class SoundManager : MonoBehaviour
{

    public AudioClip[] audioClips;

    public static SoundManager instance;
    private AudioSource au;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    void Start() {
        au = GetComponent<AudioSource>();
    }

    // Add volume scaling later
    public void PlaySound(Sound sound, float volume = 1f) {
        switch (sound){
            case Sound.STEP:
                int randSoundNumber = Random.Range(0, 3);
                au.PlayOneShot(audioClips[randSoundNumber], volume);
                break;
            case Sound.LEVER:
                au.PlayOneShot(audioClips[3], volume);
                break;
            case Sound.PUSH:
                au.PlayOneShot(audioClips[4], volume);
                break;
            case Sound.ARTIFACT_TALK:
                int randSoundNumber2 = Random.Range(5, 9);
                au.PlayOneShot(audioClips[randSoundNumber2], volume);
                break;
        }
    }


}
