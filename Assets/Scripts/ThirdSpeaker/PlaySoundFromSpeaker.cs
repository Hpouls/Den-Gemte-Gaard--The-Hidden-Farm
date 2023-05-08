using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlaySoundFromSpeaker : MonoBehaviour {
    public AudioClip myClip;
    public AudioSource myAudioSource;

    void Start() {
        AudioClip speakerSpecificClip = myClip.CreateSpeakerSpecificClip(4, 2); // Plays sound from a third speakerspeaker
        //AudioClip speakerSpecificClip = myClip.CreateSpeakerSpecificClip(4, 3); // Plays sound from a third speakerspeaker. commented out as this funciton did not seem to work properly

        myAudioSource.clip = speakerSpecificClip;
        myAudioSource.Play();
    }
}