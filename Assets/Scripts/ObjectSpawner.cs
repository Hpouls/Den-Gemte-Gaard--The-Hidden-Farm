using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject objectToSpawn; // The object to be spawned
    public Mesh meshToSpawnOn; // The mesh to spawn objects on

    public float spawnCollisionCheckRadius; //Distance the objects has to be apart for the objects to be able to spawn

    private GameObject[] spawnedObjects; // Array to store the spawned objects


    void Start() {
        // Initialize the array with the maximum number of objects. Set to 100 for security
        spawnedObjects = new GameObject[100]; 

    }

    //Method for spawning a number of objects. Must take an integer inputs to declare how many objects will be spawned
    public void SpawnXNumberOfObjects(int numberOfObjects) {
        //Destroys any missed objects and returns their value to null in the array
        DestroySpawnedObjects();

        //For the declared number of objects, spawn set objects
        for (int i = 0; i < numberOfObjects; i++)
        {
            SpawnObjectsOnVertices(); //Method for spawning a object
        }
    }

    //Method for spawning an objects on the vertices of the mesh. Should be changed to object (Singular) not objects (Plural).
    public void SpawnObjectsOnVertices() {

        Vector3[] vertices = meshToSpawnOn.vertices; // Get the mesh vertices in an array

        int randomIndex = Random.Range(0, vertices.Length); //Picks a random vertice from the array

        //Checks if the object overlaps with any spawned objects
        if (!Physics.CheckSphere(transform.TransformPoint(vertices[randomIndex]), spawnCollisionCheckRadius)) {
            //If it does not overlap, it intiates and spawns an object on the random vertice
            GameObject spawnedObject = Instantiate(objectToSpawn, transform.TransformPoint(vertices[randomIndex]), Quaternion.identity);
            AddSpawnedObject(spawnedObject); //Adds the object to the array of spawned objects
        } else {
            SpawnObjectsOnVertices(); //If the object overlaps, the method is run again to find a new spot for the object
            //WARNING Possible endless loop, if the spawnCollisionCheckRadius is too high
        }
    }

    //Method for finding the closest object for a given object
    //Used for finding the closest animal to the play object for playing sounds
    public GameObject FindClosestAnimal(GameObject primaryObject) {
        //Sets a variable for the the nearest target
        GameObject nearestTarget = null; 
        //Variable for the minimum distance found. Sets it to infinity so the first object always will be the closest
        float minimumDistance = Mathf.Infinity; 

        //Goes through all the spawnedobjects and finds the nearest one
        for (int i = 0; i < spawnedObjects.Length; i++) {
            if (spawnedObjects[i] != null){ //If there are any object on the position, check its distance to the object
                //Finds the distance between the primaryObject, and an object spawned
                float distance = Vector3.Distance(primaryObject.transform.position, spawnedObjects[i].transform.position);

                //If the distance is less than the minimum distance found, it becomes the new minimum distance
                if (distance <= minimumDistance) {
                    minimumDistance = distance; //Sets the new minimum distance
                    nearestTarget = spawnedObjects[i]; //Sets current spawned object to the nearest target 
                }
            }
        }
        return nearestTarget; //Returns the nearest target
    }

    //Method for adding a spawned object to the array of spawned objects
    public void AddSpawnedObject(GameObject spawnedObject) {
        // Add the object to the first available slot in the array
        for (int i = 0; i < spawnedObjects.Length; i++) {
            if (spawnedObjects[i] == null) {
                spawnedObjects[i] = spawnedObject; //object is set in the array
                break;
            }
        }
    }

    //Method for destroying all spawned objects
    public void DestroySpawnedObjects() {
        // Loop through the spawnedObjects array and destroy each object
        for (int i = 0; i < spawnedObjects.Length; i++) {
            if (spawnedObjects[i] != null) {
                Destroy(spawnedObjects[i]);
                // Set the array element to null to mark it as available
                spawnedObjects[i] = null; 
            }
        }
    }

    //Method for getting the number of spawned objects
    //Primarily used for checking if any animals is left
    public int getNumberOfSpawnedObjects() {
        int numberOfObjectSpawned = 0; //Variable for number of objects spawned

        //Goes through all the spawned objects
        for (int i = 0; i < spawnedObjects.Length; i++)
        {
            //Checks if there is a spawned object on the given index space 
            if (spawnedObjects[i] != null)
            {
                //Plusses the number of objects spawned with 1
                numberOfObjectSpawned = numberOfObjectSpawned + 1;
            }
        }
        return numberOfObjectSpawned; //Returns the number of objects spawned
    }

    //Gets an objects on a specific index space
    //Unsure if this is used
    public GameObject GetSpawnedObject(int position)
    {
        return spawnedObjects[position];
    }
}