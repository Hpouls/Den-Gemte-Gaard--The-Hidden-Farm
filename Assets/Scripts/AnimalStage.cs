using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalStage : MonoBehaviour {
    private GameObject gameManager; 
    private GameObject soundPlane; 

    // array for AudioClipScriptableObjects to be played before animals are destroyed
    public AudioClipScriptableObject[] intro;
    // array for AudioClipScriptableObjects to be played after animals are destroyed 
    public AudioClipScriptableObject[] outro; 

    private AudioSource audioSource; 

    public static SceneState State;

    // bool for updating if an animal is found or not
    public static bool animalFound; 

    // public float for waiting amount of time before playing another audioclip
    public float seconds; 

    public WiimoteIRObjectMovement wiimoteIRObjectMovement;


    // Start is called before the first frame update
    void Start() {
        // might not do anything, else, asigns "SoundPlane" to soundPlane within script
        soundPlane = GameObject.FindGameObjectWithTag("SoundPlane"); 
        // might not do anything, else, asigns "GameManager" to gameManager within script
        gameManager = GameObject.FindGameObjectWithTag("GameManager"); 
        animalFound = false;
        // starts first scenestate 
        State = SceneState.Intro; 
        // starts coroutine 
        StartCoroutine(PlaySounds(State)); 
    }

    private void Update() {
        if(soundPlane == null) {
            // saftey-net function to make sure objects exist 
            soundPlane = GameObject.FindGameObjectWithTag("SoundPlane"); 
        }
        if (gameManager == null) {
            // saftey-net function to make sure objects exist
            gameManager = GameObject.FindGameObjectWithTag("GameManager");  
        }

        // checks for objects within soundPlane and translates the amount of objects into an int
        int numberOfObjectsSpawned = soundPlane.GetComponent<ObjectSpawner>().getNumberOfSpawnedObjects(); 
        // if there are no objects set animalFound to true 
        if (numberOfObjectsSpawned <= 0) { 
            animalFound = true;
        }
    }

    public IEnumerator PlaySounds(SceneState State) { 
        while (true) { 
            // gets audiosource
            audioSource = GetComponent<AudioSource>(); 
            SceneState newState = State;

            // switch case for the states each animalstage goes through
            switch (newState) { 
                // first case to play
                case SceneState.Intro: 

                    // for loop to have the intro arrays lenght 
                    for (int i = 0; i < intro.Length; i++) { 
                        // gets the audiosource from intro
                        this.audioSource.clip = intro[i].AudioClip;
                        // plays the gotten audiosource
                        this.audioSource.Play(); 
                        // wait untill audiosource has played its full amount
                        yield return new WaitForSeconds(this.audioSource.clip.length);
                    }
                    State = SceneState.FindAnimal;
                    break;

                case SceneState.FindAnimal:
                    // checks to see if the animals are sitll on screen
                    while (animalFound == false) {  
                        // finds closest animal object to "Cursor"
                        GameObject animal = 
                        gameManager.GetComponent<GameManager>().FindClosestAnimal(GameObject.Find("Cursor")); 
                        // gets audiosource on the closest animal
                        AudioSource audioSource = animal.GetComponent<SoundPlayer>().GetAudioSource(); 
                        // play audiosource
                        audioSource.Play(); 
                        while (animal != null && audioSource.isPlaying) {
                            yield return new WaitForSeconds(0.5f);
                        }
                        // waits for amount of time before starting next sound
                        yield return new WaitForSeconds(seconds); 
                    }
                    // only goes to next state when "animalFound" is equal to true
                    State = SceneState.Outro; 
                    break;

                case SceneState.Outro:
                    // for loop to have the outro arrays lenght
                    for (int i = 0; i < outro.Length; i++) {  
                        // gets audioclips from outro
                        this.audioSource.clip = outro[i].AudioClip; 
                        // plays audioclips 
                        this.audioSource.Play(); 
                        // wait until audio clips is finished
                        yield return new WaitForSeconds(this.audioSource.clip.length); 
                    }
                    gameManager.GetComponent<GameManager>().UpdateGameState();
                    // might not do anything, else, destroys object
                    Destroy(gameObject); 
                    break;
            }
        }
    }

    public enum SceneState {
        Intro,
        FindAnimal,
        Outro
    }
}
