using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Sound {
    STEP,
    LEVER,
    PUSH,
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
    public void PlaySound(Sound sound) {
        switch (sound){
            case Sound.STEP:
                int randSoundNumber = Random.Range(0, 3);
                au.PlayOneShot(audioClips[randSoundNumber]);
                break;
            case Sound.LEVER:
                au.PlayOneShot(audioClips[3]);
                break;
            case Sound.PUSH:
                au.PlayOneShot(audioClips[4]);
                break;
        }
    }


}
