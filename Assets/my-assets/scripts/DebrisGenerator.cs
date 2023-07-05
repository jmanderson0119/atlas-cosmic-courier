using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class exists for the purpose of generating environmental hazards in the level. 
 * It functions according to the following general steps:
 *   1. Create the debris core.
 *   2. Initialize the qualities of the debris core: texture, rigidbody, zero gravity, etc.
 *   3. For a randomly selected subset of the octants of the debris core, generate an debris module:
 *     1. Create the primitive cube and intialize its qualities (parents, texture, etc.).
 *     2. Randomly assign the center of the module to within the respective octant space.
 *     3. Assign a random scale to the length, width, and height.
 *   4. change the size of the core by a randomly assigned scaling factor.
 *   5. Randomly place the debris within the scene.
 */
public class DebrisGenerator : MonoBehaviour
{

    [Header("Render Setting")]
    [SerializeField] private Material debrisTexture; // surface material for the debriss

    [Header("Generation Settings")]
    [SerializeField] private int number; // number of debris to spawn into the level
    [SerializeField] private int coreLowerBound, coreUpperBound; // scaling bounds for the debris core object
    [SerializeField] private float moduleLowerBound, moduleUpperBound; // scaling bounds for the debris module objects; relative to debris core (keep between 0 and 1)

    /*
     * unsigned scaler representing the core scaling concentration (larger the value -> smaller average core scaling)
     * this value must lie between coreLowerBound and coreUpperBound
     */
    [Header("Scaling Setting")]
    [SerializeField] private float concentrationScaler;

    private float randScaler; // random scaling for the debris core object
    private float moduleX, moduleY, moduleZ; // randomized scaler and positioning factor for debris modules
    private int octantX, octantY, octantZ; // selects each octant of the debris core for each iteration of debris module generation

    // Start is called before the first frame update
    void Start()
    {
        debrisGeneration();
    }

    private void debrisGeneration()
    {
        for (int h = 0; h < number; h++)
        {
            // random scaling for the debris core
            randScaler = Random.Range(coreLowerBound, coreUpperBound) / concentrationScaler;

            // intialization of the debris core's key qualities
            GameObject debrisCore = GameObject.CreatePrimitive(PrimitiveType.Cube);
            debrisCore.GetComponent<Renderer>().material = debrisTexture;
            Rigidbody rigidBody = debrisCore.AddComponent<Rigidbody>();
            rigidBody.useGravity = false;
            rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rigidBody.interpolation = RigidbodyInterpolation.Interpolate;

            // generates each of the debris modules
            for (int i = 0; i < 8; i++)
            {
                // determines which octant to place the given module in
                octantX = (i < 4) ? -1 : 1;
                octantY = (i == 0 || i == 1 || i == 4 || i == 5) ? -1 : 1;
                octantZ = (i % 2 == 0) ? -1 : 1;

                // initialization of the debris module
                GameObject debrisModule = GameObject.CreatePrimitive(PrimitiveType.Cube);
                debrisModule.transform.SetParent(debrisCore.transform);
                debrisModule.GetComponent<Renderer>().material = debrisTexture;

                // randomized scaling of the module
                moduleX = Random.Range(moduleLowerBound, moduleUpperBound) * debrisCore.transform.localScale.x;
                moduleY = Random.Range(moduleLowerBound, moduleUpperBound) * debrisCore.transform.localScale.y;
                moduleZ = Random.Range(moduleLowerBound, moduleUpperBound) * debrisCore.transform.localScale.z;

                // randomized position and scaling applied to the module
                debrisModule.transform.position = new Vector3(octantX * moduleX, octantY * moduleY, octantZ * moduleZ);
                debrisModule.transform.localScale = new Vector3(Random.Range(2f * moduleX, 3f * moduleX), Random.Range(2f * moduleY, 3f * moduleY), Random.Range(2f * moduleZ, 3f * moduleZ));

                // setting up module collider

                BoxCollider collider = debrisModule.GetComponent<BoxCollider>();
                collider.size = debrisModule.transform.localScale;
            }

            // randomized position, scaling, and rotation applied to the debris
            debrisCore.transform.localScale = new Vector3(randScaler, randScaler, randScaler);
            debrisCore.transform.position = new Vector3(Random.Range(0, 1000), Random.Range(0, 600), Random.Range(0, 1500));
            debrisCore.transform.Rotate(Random.Range(0, 179), Random.Range(0, 179), Random.Range(0, 179));
        }
    }
}
