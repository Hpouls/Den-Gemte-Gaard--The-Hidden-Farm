using UnityEngine;

public class ObjectSpawner : MonoBehaviour {
    public GameObject objectToSpawn; // The object to be spawned
    public Mesh meshToSpawnOn; // The mesh to spawn objects on
    //public int numberOfObjects;
    public float spawnCollisionCheckRadius;

    private GameObject[] spawnedObjects; // Array to store the spawned objects


    void Start() {
        spawnedObjects = new GameObject[100]; // Initialize the array with the maximum number of objects
        //SpawnXNumberOfObjects(numberOfObjects);
    }

    public void SpawnXNumberOfObjects(int numberOfObjects) {
        DestroySpawnedObjects();

        for (int i = 0; i < numberOfObjects; i++) {
            SpawnObjectsOnVertices();
        }
    }

    public void SpawnObjectsOnVertices() {
        // Get the mesh vertices
        Vector3[] vertices = meshToSpawnOn.vertices;

        int randomIndex = Random.Range(0, vertices.Length);

        if (!Physics.CheckSphere(transform.TransformPoint(vertices[randomIndex]), spawnCollisionCheckRadius)) {
            GameObject spawnedObject = Instantiate(objectToSpawn, transform.TransformPoint(vertices[randomIndex]), Quaternion.identity);
            AddSpawnedObject(spawnedObject);
        }
        else
        {
            SpawnObjectsOnVertices();
        }
   }


    public GameObject FindClosestAnimal(GameObject primaryObject) {
        GameObject nearestTarget = null;
        float minimumDistance = Mathf.Infinity;

        for (int i = 0; i < spawnedObjects.Length; i++) {
            if (spawnedObjects[i] != null)
            {
                float distance = Vector3.Distance(primaryObject.transform.position, spawnedObjects[i].transform.position);

                if (distance <= minimumDistance) {
                    minimumDistance = distance;
                    nearestTarget = spawnedObjects[i];
                }
            }
        }
        return nearestTarget;
    }

    public void AddSpawnedObject(GameObject spawnedObject) {
        // Add the object to the first available slot in the array
        for (int i = 0; i < spawnedObjects.Length; i++) {
            if (spawnedObjects[i] == null) {
                spawnedObjects[i] = spawnedObject;
                break;
            }
        }
    }

    public void DestroySpawnedObjects() {
        // Loop through the spawnedObjects array and destroy each object
        for (int i = 0; i < spawnedObjects.Length; i++) {
            if (spawnedObjects[i] != null) {
                Destroy(spawnedObjects[i]);
                spawnedObjects[i] = null; // Set the array element to null to mark it as available
            }
        }
    }

    public int getNumberOfSpawnedObjects() { 
        int numberOfObjectSpawned = 0;
        

        for (int i = 0; i < spawnedObjects.Length; i++) {
            //Debug.Log("Hello");
            if(spawnedObjects[i] != null) {
                numberOfObjectSpawned = numberOfObjectSpawned + 1;
            }
        }

        return numberOfObjectSpawned;
    }

    public GameObject GetSpawnedObject(int position) {
        return spawnedObjects[position];
    }
}