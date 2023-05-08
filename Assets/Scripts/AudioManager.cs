using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    /* 
        This script was made to inherit all audio files at the start of the project. 
        As the application was built more and more this script was reduced to only hold the "Found Animal" sound
        This is the "Ding" that is heard when an animal is found
    */

    private AudioSource audioSource;

    public void PlayFoundSound() {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }
}