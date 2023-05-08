using System.Collections;
using UnityEngine;

public class SoundPlayer : MonoBehaviour {
    public AudioClipScriptableObject[] audioClip;
    public AudioSource audioSource;


    public AudioSource GetAudioSource() {
        int randomSound = Random.Range(0, audioClip.Length);
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioClip[randomSound].AudioClip;
        return audioSource;
    }

    /*public void PlaySound() {
        //int randomSound = Random.Range(0, audioClip.Length);
        //audioSource = GetComponent<AudioSource>();
        //audioSource.clip = audioClip[randomSound].AudioClip;
        audioSource.Play();

        /*while (toggle == false && isPlaying == false){
            
            isPlaying = true;
        }

        while (toggle == true && isPlaying == true)
        {
            audioSource.Stop();
            isPlaying = false;
        }       
    }*/

    /*void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            int randomSound = Random.Range(0, audioClip.Length);
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = audioClip[randomSound].AudioClip;
            audioSource.Play();
        }
    }*/
}