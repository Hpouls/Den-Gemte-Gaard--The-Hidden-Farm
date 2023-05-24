using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance; // Singleton instance of the GameManager
    public GameObject soundPlane;

    public GameObject[] animals; //public array for animal objects to be used
    public GameObject Intro, CowStage, SheepStage, HorseStage, ChickenStage, PigStage; //Might not do anything, else, Public gameobjects for the different stages the game goes through
    
    public int AnimalStage = 6; //Public int for choosing the certain stage the application starts at
    //6 = intro, 5 = CowStage, 4 = SheepStage, 3 = HorseStage, 2 = ChickenStage, 1 = PigStage

    private Vector3 position = new Vector3(0, 0, 0);

    public float seconds; //Might not do anythign, else, wait time inbetween audioclips

    public WiimoteIRObjectMovement wiimoteIRObjectMovement;


    void Start() {
        UpdateGameState(); //Starts the UpdateGameState funciton, might just put it in here
    }

    void Awake() {
        // Ensure that there is only one instance of the GameManager
        if (instance == null) {
            instance = this;
            //DontDestroyOnLoad(gameObject); //Does bad stuff, needs to be commented out :)
        } else {
            Destroy(gameObject);
        }

        AudioSettings.speakerMode = AudioSettings.driverCapabilities;
    }


    public GameObject FindClosestAnimal(GameObject primary) { //gets closest animal from "objectspawner" script
        GameObject closestAnimal = soundPlane.GetComponent<ObjectSpawner>().FindClosestAnimal(primary);
        return closestAnimal;
    }

    // Future works, setup switch case the same way as they are in the animalstage script, and implementing a way for it to progress 

    public void UpdateGameState() { //switch cases for going through the different stages in the application
        switch (AnimalStage) {
            case 6:
                Instantiate(Intro); // starts the array for intro within animalstage
                soundPlane.GetComponent<ObjectSpawner>().objectToSpawn = animals[4]; //spawns a certain animal from the animals array
                soundPlane.GetComponent<ObjectSpawner>().SpawnXNumberOfObjects(1); // spawns a specific amount of animals of the chosen type
                //Instantiate(CowStage, position, Quaternion.identity);
                AnimalStage --; //lower the AnimalStage so the switch case ensures wich stage it is on
                break;
            case 5: //cow
                soundPlane.GetComponent<ObjectSpawner>().objectToSpawn = animals[0];
                soundPlane.GetComponent<ObjectSpawner>().SpawnXNumberOfObjects(3);
                Instantiate(CowStage, position, Quaternion.identity); // instatiates the object set onto CowStage at a vector 0 (position & with the posibility to rotate
                AnimalStage --;
                break;
            case 4: //sheep
                soundPlane.GetComponent<ObjectSpawner>().objectToSpawn = animals[1];
                soundPlane.GetComponent<ObjectSpawner>().SpawnXNumberOfObjects(3);
                Instantiate(SheepStage, position, Quaternion.identity);
                AnimalStage --;
                break;
            case 3: // Horse
                soundPlane.GetComponent<ObjectSpawner>().objectToSpawn = animals[2];
                soundPlane.GetComponent<ObjectSpawner>().SpawnXNumberOfObjects(3);
                Instantiate(HorseStage, position, Quaternion.identity);
                AnimalStage --;
                break;
            case 2: // chicken
                soundPlane.GetComponent<ObjectSpawner>().objectToSpawn = animals[3];
                soundPlane.GetComponent<ObjectSpawner>().SpawnXNumberOfObjects(3);
                Instantiate(ChickenStage, position, Quaternion.identity);
                AnimalStage --;
                break;
            case 1: // pig
                soundPlane.GetComponent<ObjectSpawner>().objectToSpawn = animals[4];
                soundPlane.GetComponent<ObjectSpawner>().SpawnXNumberOfObjects(3);
                Instantiate(PigStage, position, Quaternion.identity);
                AnimalStage --;
                break;
            case 0: //last case reruns the "UpdateGameState" from "CowStage" and down again
                AnimalStage = 5;
                UpdateGameState();
                break;
            default:
                break;
        }
    }
}