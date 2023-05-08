using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalStage : MonoBehaviour {
    private GameObject gameManager; 
    private GameObject soundPlane; 

    public AudioClipScriptableObject[] intro; // array for AudioClipScriptableObjects to be played before animals are destroyed
    public AudioClipScriptableObject[] outro; // array for AudioClipScriptableObjects to be played after animals are destroyed

    private AudioSource audioSource; 

    public static SceneState State;

    public static bool animalFound; // bool for updating if an animal is found or not

    public float seconds; // public float for waiting amount of time before playing another audioclip

    public WiimoteIRObjectMovement wiimoteIRObjectMovement;


    // Start is called before the first frame update
    void Start() {
        soundPlane = GameObject.FindGameObjectWithTag("SoundPlane"); // might not do anything, else, asigns "SoundPlane" to soundPlane within script
        gameManager = GameObject.FindGameObjectWithTag("GameManager"); // might not do anything, else, asigns "GameManager" to gameManager within script
        animalFound = false;
        State = SceneState.Intro; // starts first scenestate 
        StartCoroutine(PlaySounds(State)); // starts coroutine 
    }

    private void Update() {
        if(soundPlane == null) {
            soundPlane = GameObject.FindGameObjectWithTag("SoundPlane"); // saftey-net function to make sure objects exist 
        }
        if (gameManager == null) {
            gameManager = GameObject.FindGameObjectWithTag("GameManager"); // saftey-net function to make sure objects exist 
        }

        int numberOfObjectsSpawned = soundPlane.GetComponent<ObjectSpawner>().getNumberOfSpawnedObjects(); // checks for objects within soundPlane and translates the amount of objects into an int
        if (numberOfObjectsSpawned <= 0) { // if there are no objects set animalFound to true 
            animalFound = true;
        }
    }

    public IEnumerator PlaySounds(SceneState State) { 
        while (true) { 
            audioSource = GetComponent<AudioSource>(); // gets audiosource
            SceneState newState = State;  // 

            switch (newState) { // switch case for the states each animalstage goes through
                case SceneState.Intro: // first case to play

                    for (int i = 0; i < intro.Length; i++) { // for loop to have the intro arrays lenght 
                        this.audioSource.clip = intro[i].AudioClip; //gets the audiosource from intro
                        this.audioSource.Play(); // plays the gotten audiosourec
                        yield return new WaitForSeconds(this.audioSource.clip.length); // wait untill audiosource has played its full amount
                    }
                    State = SceneState.FindAnimal;
                    break;

                case SceneState.FindAnimal:
                    while (animalFound == false) {  // checks to see if the animals are sitll on screen
                        //Debug.Log("Heye");
                        GameObject animal = gameManager.GetComponent<GameManager>().FindClosestAnimal(GameObject.Find("Cursor")); // finds the closest animal object in relation to the "Cursor"
                        AudioSource audioSource = animal.GetComponent<SoundPlayer>().GetAudioSource(); // gets audiosource on the closest animal
                        audioSource.Play(); // play audiosource

                        while (animal != null && audioSource.isPlaying) { //
                            yield return new WaitForSeconds(0.5f);
                        }

                        yield return new WaitForSeconds(seconds); // waits for amount of time before starting next sound
                    }
                    State = SceneState.Outro; // only goes to next state when "animalFound" is equal to true
                    break;

                case SceneState.Outro:
                    for (int i = 0; i < outro.Length; i++) { // for loop to have the outro arrays lenght 
                        this.audioSource.clip = outro[i].AudioClip; // gets audioclips from outro
                        this.audioSource.Play(); // plays audioclips 
                        yield return new WaitForSeconds(this.audioSource.clip.length); // wait until audio clips is finished
                    }
                    gameManager.GetComponent<GameManager>().UpdateGameState();
                    Destroy(gameObject); // might not do anything, else, destroys object

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
