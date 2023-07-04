using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class exists for the purpose of generating environmental hazards in the level. 
 * It functions according to the following general steps:
 *   1. Create the asteroid core.
 *   2. Initialize the qualities of the asteroid core: texture, rigidbody, zero gravity, etc.
 *   3. For a randomly selected subset of the octants of the asteroid core, generate an asteroid module:
 *     1. Create the primitive cube and intialize its qualities (parents, texture, etc.).
 *     2. Randomly assign the center of the module to within the respective octant space.
 *     3. Assign a random scale to the length, width, and height.
 *   4. Distort the core, thereby distorting its modules, by a randomly assigned length, width, and height.
 *   5. Randomly place the asteroid within the scene.
 */
public class AsteroidGenerator : MonoBehaviour
{

    [SerializeField] private Material asteroidTexture; // surface material for the asteroids
    private float randLength, randWidth, randHeight; // random scaling for the asteroid core object
    private int octantX, octantY, octantZ; // selects each octant of the asteroid core for each iteration of asteroid module generation
    private float moduleX, moduleY, moduleZ; // random scaling for the asteroid module objects

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 600; i++)
        {
            AsteroidGeneration();
        }
    }

    private void AsteroidGeneration()
    {
        // random scaling for the asteroid core
        randLength = Random.Range(2, 12);
        randWidth = Random.Range(2, 12);
        randHeight = Random.Range(2, 12);

        // intialization of the asteroid core's key qualities
        GameObject asteroidCore = GameObject.CreatePrimitive(PrimitiveType.Cube);
        asteroidCore.GetComponent<Renderer>().material = asteroidTexture;
        Rigidbody rigidBody = asteroidCore.AddComponent<Rigidbody>();
        rigidBody.useGravity = false;
        rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // generates each of the asteroid modules
        for (int i = 0; i < 8; i++)
        {
            // this varies which octants are selected for module generation
            if (Random.Range(0, 3) != 1)
            {
                // determines which octant to place the given module in
                octantX = (i < 4) ? -1 : 1;
                octantY = (i == 0 || i == 1 || i == 4 || i == 5) ? -1 : 1;
                octantZ = (i % 2 == 0) ? -1 : 1;

                // initialization of the asteroid module
                GameObject asteroidModule = GameObject.CreatePrimitive(PrimitiveType.Cube);
                asteroidModule.transform.SetParent(asteroidCore.transform);
                asteroidModule.GetComponent<Renderer>().material = asteroidTexture;

                // randomized scaling of the module
                moduleX = Random.Range(0.25f, 0.75f);
                moduleY = Random.Range(0.25f, 0.75f);
                moduleZ = Random.Range(0.25f, 0.75f);

                // randomized position and scaling applied to the module
                asteroidModule.transform.position = new Vector3(octantX * moduleX * asteroidCore.transform.localScale.x, octantY * moduleY * asteroidCore.transform.localScale.y, octantZ * moduleZ * asteroidCore.transform.localScale.z);
                asteroidModule.transform.localScale = new Vector3(Random.Range(2 * moduleX, 3 * moduleX), Random.Range(2 * moduleY, 3 * moduleY), Random.Range(2 * moduleZ, 3 * moduleZ));
            }
        }

        // randomized position, scaling, and rotation applied to the asteroid
        asteroidCore.transform.localScale = new Vector3(randLength, randWidth, randHeight);
        asteroidCore.transform.position = new Vector3(Random.Range(0, 1000), Random.Range(0, 1000), Random.Range(0, 1000));
        asteroidCore.transform.Rotate(Random.Range(0, 179), Random.Range(0, 179), Random.Range(0, 179));
    }
}
