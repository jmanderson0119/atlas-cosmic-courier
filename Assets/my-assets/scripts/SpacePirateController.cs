using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script will control the behavior of the enemy bots, whose objective it is to destroy the player, 
/// avoid collisions, and smoothly loop around the player in a dogfighting style.
/// 
/// The bot will take into consideration its given destination: the player, the location in which they were instantiated relative to the player (spawn),
/// and the point in which the enemy should begin to turn around and head back to the spawn to pass by the player once more and fire (the pass-point).
/// 
/// Upon receiving this information, the enemy will be given a generated path and, in some cases, perform other actions
/// during flight, such as shooting at the player and rolling in space.
/// </summary>
public class SpacePirateController : MonoBehaviour
{
    #region Controller Data
    [Header("Objective Information")]
    [SerializeField] private GameObject player; // player data reference
    [SerializeField] private Vector3 destinationPoint; // where the enemy is trying to fly to


    [Header("Enemy Configuration")]
    [SerializeField] private Objective objective; // enemy objective
    [SerializeField] private GameObject bulletPrefab; // enemy projectile to fire at player
    private GameObject bullet; // bullet object holding prefab
    [SerializeField] private float rotationSpeed; // enemy rotation speed
    [SerializeField] private float throttleSpeed; // enemy throttle speed

    [Header("Pathfinding Configuration")]
    [SerializeField] private float nodeFieldSize; // size of the box cast being performed for each candidate node (200f)

    private List<Node> pathList; // path enemy is currently traversing
    private RaycastHit raycastData; // raycast information for candidate nodes
    private int nodeMarker = 0; // index of the node the enemy is to travel toward -- currentDestination index in pathList
    private bool shotsFired; // whether enemy has taken fire against player during dogfight loop
    private Vector3 targetDirection; // objective rotation for enemy
    private Quaternion targetRotation; // quaternion representation of targetDirection
    #endregion

    /// <summary>
    /// The Objective enum will be the indicator to the enemy that the destination has changed and the actions they should be performing have changes
    /// </summary>
    #region Objective Enum
    // objective states: defines a destination point, which further defines actions to perform during flight
    public enum Objective
    {
        GoToPlayer,
        GoToPassPoint,
        GoToSpawn,
        TurnAroundAtSpawn,
        TurnAroundAtPassPoint
    }
    #endregion

    /// <summary>
    /// NodeComparer will allow the sorted list used in Pathfinder() to order nodes by their f values - the move cost from start to finish
    /// </summary>
    #region Node Comparer Class
    // comparer type for candidateList, useful for getting Node with smallest f values quickly
    public class NodeComparer : IComparer
    {
        public int Compare(object firstNode, object secondNode)
        {
            return ((Node) firstNode).getF().CompareTo(((Node) secondNode).getF());
        }
    }
    #endregion

    /// <summary>
    /// Each node defines a region in space containing the following information:
    /// a parent node - the node that theoretically would come before this node in the generated path
    /// the center position - each node is a cube in space that extends from the center on all three axes by nodeFieldSize
    /// g, h, f - values that define movement cost from start to destination. H is define using the Euclidean heuristic (Euclidean distance formula)
    /// </summary>
    #region Node Class
    public class Node
    {
        // initialize Node parent, position, and move cost from start node to this node
        public Node(Node parent, Vector3 centerPosition)
        {
            this.parent = parent;
            this.centerPosition = centerPosition;
            this.g = parent.g + Vector3.Distance(parent.getCenterPosition(), centerPosition);
        }


        // a Node stores a g, h, f (g = move cost from start to this node, h = move cost from this node to destination, f = g + h) value, its parent, and its position
        private Node parent;
        private Vector3 centerPosition;
        private float g, h, f;


        // getter methods
        public Node getParent() => parent;
        public Vector3 getCenterPosition() => centerPosition;
        public float getG() => g;
        public float getH() => h;
        public float getF() => f;


        // setter methods
        public void setCenterPosition(Vector3 centerPosition) { this.centerPosition = centerPosition; }
        public void setH(float h) { this.h = h; }
        public void setF() { this.f = this.g + this.h; }
    }
    #endregion


    /// <summary>
    /// An A* search algorithm desgined to generate an optimal path from the enemy's current position to the given destionation while
    /// also considering the possibility of colliding with the debris field. It does this by generating nodes representing 3D sections of space
    /// surrounding the current position whose centers are closer to the objective than the current position, then filtering out those that contain 
    /// debris and those that are already either being considered for potential pathing or are included in the final path and have a better
    /// calculated move cost from start to finish.
    /// </summary>
    /// <param name="destinationPoint">3D vector defining the location in space the enemy is pathfinding toward</param>
    /// <returns>traversalList - a list of nodes that define the shortest path between the current position and destinationPoint</returns>
    #region Pathfinder
    public List<Node> Pathfinder(Vector3 destinationPoint)
    {
        // Initialize the open list of node candidates for the final path
        SortedList candidateList = new SortedList(new NodeComparer());
        
        candidateList.Add(0, new Node(null, transform.position));


        // Initialize the final path
        List<Node> traversalList = new List<Node>();


        // Initialize the counting digits for generating each node candidate to be added to candidateList
        int num1 = -1, num2 = -1, num3 = -1;


        // Initialize nodeList for inspecting nodes surrounding currentNode
        List<Node> nodeList = new List<Node>();


        // during each loop, the node corresponding to the shortest path will be added to traversalList
        while (candidateList.Count != 0)
        {
            // remove the node with the smallest f value
            Node currentNode = (Node) candidateList.GetByIndex(0);
            candidateList.RemoveAt(0);
            Debug.Log(currentNode.getCenterPosition());


            // create every candidate nodes and add to nodeList
            for (int i = 0; i < 25; i++)
            {
                // determine surrounding node to create and queue
                num3++;

                if (num3 > 1)
                {
                    num3 = -1;
                    num2++;
                }

                if (num2 > 1)
                {
                    num2 = -1;
                    num1++;
                }


                // node does not need to be created in origin node section -- this is where currentNode is
                if (num1 == 0 && num2 == 0 && num3 == 0)
                {
                    num3++;
                }


                // declare the surrounding node determined by num1, num2, and num3
                Node newNode = new Node(currentNode, currentNode.getCenterPosition() + new Vector3(num1 * nodeFieldSize * 4f, num2 * nodeFieldSize * 4f, num3 * nodeFieldSize * 4f));


                // box cast will check whether the node field is empty or not
                bool containsObstacles = Physics.BoxCast(newNode.getCenterPosition(), new Vector3(nodeFieldSize, nodeFieldSize, nodeFieldSize), newNode.getCenterPosition() - newNode.getParent().getCenterPosition(), out raycastData, Quaternion.identity);
           

                // if the box doesn't contains obstacles and is closer to the destination node than the current node, add it to nodeList
                if (!containsObstacles && Vector3.Distance(newNode.getCenterPosition(), destinationPoint) < Vector3.Distance(newNode.getParent().getCenterPosition(), destinationPoint))
                {
                    nodeList.Add(newNode);
                }


                /* 
                 * determine if candidate should be added to the candidateList or not. This will be based on the current contents of the candidateList
                 * and the traversalList as well as the g, h, and f values of the candidates
                 */
                foreach (Node nodeCandidate in nodeList)
                {
                    // each node candidate is considered a valid candidate for traversal until proven otherwise
                    bool isCandidate = true;


                    // if destinationPoint resides in the field represented by nodeCandidate, this node will be added to the traversalList and the search is over
                    if (Mathf.Abs(destinationPoint.x - nodeCandidate.getCenterPosition().x) < 400f && Mathf.Abs(destinationPoint.y - nodeCandidate.getCenterPosition().y) < 400f && Mathf.Abs(destinationPoint.z - nodeCandidate.getCenterPosition().z) < 400f)
                    {
                        nodeCandidate.setCenterPosition(destinationPoint); // set the node the player is going to traverse toward as the destinationPoint

                        traversalList.Add(nodeCandidate); // final node containing the enemy destination, completing the generated path

                        pathList = traversalList; // save the traversal list produced into the current path of the enemy

                        return traversalList; // output the optimal path
                    }


                    // compute the h and f values for the candidate node
                    nodeCandidate.setH(Vector3.Distance(nodeCandidate.getCenterPosition(), destinationPoint));
                    nodeCandidate.setF();


                    /*
                     * Two things have to be checked: if some other iteration of while has already added this candidate to the open list or closed list and that element has a smaller f, 
                     * it can be skipped; otherwise, the candidate can be added to the open list
                     */

                    // open list check
                    if (candidateList.Count > 0)
                    {
                        foreach (Node candidate in candidateList)
                        {
                            if ((candidate.getCenterPosition() == nodeCandidate.getCenterPosition() && candidate.getF() < nodeCandidate.getF()))
                            {
                                isCandidate = false;
                                break;
                            }
                        }
                    }

                    // closed list check
                    if (traversalList.Count > 0)
                    {
                        foreach(Node pathNode in traversalList)
                        {
                            if (pathNode.getCenterPosition() == nodeCandidate.getCenterPosition() && pathNode.getF() < nodeCandidate.getF())
                            {
                                isCandidate = false;
                                break;
                            }
                        }
                    }


                    // finally, if the node is still a valid candidate for pathing after the previous checks, it is added to candidateList
                    if (isCandidate)
                    {
                        candidateList.Add(nodeCandidate.getF(), nodeCandidate);
                    }
                }


                // currentNode can now be added to the traversal list
                traversalList.Add(currentNode);
            }
        }


        // the list theoretically should never empty (while-loop should run until node containing destinationPoint is found), but if this codepath is somehow reached, whatever is logged will be returned so I can observe what the pathfinder told the ship to do
        pathList = traversalList;
        return traversalList;
    }
    #endregion

    // setting the current enemy objective is a matter of check the current objective and setting according to the objective-loop: go to player, go to pass point, turn around, go to spawn, turn around.
    private void setObjective()
    {
        switch (objective)
        {
            // switch to next phase, start adapting to optimal paths every five seconds
            case Objective.GoToPlayer:
                objective = Objective.GoToPassPoint;
                InvokeRepeating("PathfinderWrapper", 0f, 5f);
                shotsFired = false;
                break;

            // switch to next phase, reset shotsFired, start adapting to optimal paths every five seconds
            case Objective.GoToPassPoint:
                objective = Objective.TurnAroundAtPassPoint;
                CancelInvoke("PathfinderWrapper");
                break;

            // switch to next phase, find updated optimal paths every five seconds
            case Objective.GoToSpawn:
                objective = Objective.TurnAroundAtSpawn;
                CancelInvoke("PathfinderWrapper");
                break;

            // switch to next phase, stop calling pathfinder()
            case Objective.TurnAroundAtSpawn:
                objective = Objective.GoToPlayer;
                InvokeRepeating("PathfinderWrapper", 0f, 5f);
                break;

            // switch to next phase, stop calling pathfinder()
            case Objective.TurnAroundAtPassPoint:
                objective = Objective.GoToSpawn;
                InvokeRepeating("PathfinderWrapper", 0f, 5f);
                break;
        }
    }
    
    // enemy will start traveling toward the next node in pathList
    private void setNextDestination()
    {
        if (nodeMarker < pathList.Count - 1)
            transform.rotation = Quaternion.Euler(pathList[++nodeMarker].getCenterPosition() - transform.position);
    }

    // set the position, rotation, and objective 
    void Start()
    {
        transform.position = player.transform.position + player.transform.up * 1000f + player.transform.forward * 4000f + player.transform.right * Random.Range(-500f, 500f);
        transform.rotation = Quaternion.Euler(player.transform.forward * -1);
        objective = Objective.GoToPlayer;
        PathfinderWrapper();
    }

    // Update is called once per frame
    void Update()
    {
        // perform actions + define traversal path by objective
        switch(objective)
        {
            case Objective.GoToPlayer:
                transform.position += transform.forward * throttleSpeed * Time.deltaTime; // fly forward

                // update node the enemy is traveling toward if sufficiently close to the current node set as the destination
                if (Vector3.Distance(transform.position, pathList[nodeMarker].getCenterPosition()) < 50f)
                {
                    setNextDestination();
                }

                // if the enemy is at the end of the path they are traversing, the objective must be updated
                if (pathList[nodeMarker].getCenterPosition() == destinationPoint && Vector3.Distance(pathList[nodeMarker].getCenterPosition(), destinationPoint) < 100f)
                {
                    setObjective();
                }
                
                // once the enemy is locked onto the player, they will start shooting
                if (Vector3.Angle(transform.forward, player.transform.position - transform.position) < 20f && shotsFired == false)
                {
                    StartCoroutine("Shoot");
                    shotsFired = true;
                }

                break;

            case Objective.GoToSpawn:

            case Objective.GoToPassPoint:
                transform.position += transform.forward * throttleSpeed * Time.deltaTime; // fly forward

                // update node the enemy is traveling toward if sufficiently close to the current node set as the destination
                if (Vector3.Distance(transform.position, pathList[nodeMarker].getCenterPosition()) < 50f)
                {
                    setNextDestination();
                }

                // if the enemy is at the end of the path they are traversing, the objective must be updated
                if (pathList[nodeMarker].getCenterPosition() == destinationPoint && Vector3.Distance(pathList[nodeMarker].getCenterPosition(), destinationPoint) < 100f)
                {
                    setObjective();
                }

                break;

            case Objective.TurnAroundAtSpawn:

                targetDirection = player.transform.forward * -1; // objective is to rotate until the enemy is facing the player

                targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up); // rotation for enemy to achieve

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime); // smoothly rotate
                transform.position += transform.forward * throttleSpeed * Time.deltaTime; // fly forward

                // update objective if the enemy is close enough to completely turned-around
                if (Vector3.Angle(transform.forward, player.transform.forward * -1) < 10f)
                {
                    setObjective();
                }

                break;

            case Objective.TurnAroundAtPassPoint:

                targetDirection = player.transform.forward; // objective is to rotate until the enemy is facing the player

                targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up); // rotation for enemy to achieve

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime); // smoothly rotate
                transform.position += transform.forward * throttleSpeed * Time.deltaTime; // fly forward

                // update objective if the enemy is close enough to completely turned-around
                if (Vector3.Angle(transform.forward, player.transform.forward) < 10f)
                {
                    setObjective();
                }

                break;
                
        }
    }

    IEnumerator Shoot()
    {
        for (int i = 0; i < Random.Range(1, 5); i++)
        {
            if (objective == Objective.GoToPlayer)
            {
                // instantiate and initialize
                bullet = Instantiate(bulletPrefab);
                Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();

                // set position and rotation relative to player display
                bullet.transform.position = transform.position + transform.forward * 50f;
                bullet.transform.rotation = transform.rotation * Quaternion.Euler(90f, 0f, 0f);

                // fire
                bulletRB.AddForce(transform.forward * 3000f, ForceMode.VelocityChange);

                // wait until the bullet is out of the player's view
                yield return new WaitForSeconds(4f);

                // "unspawn" bullet
                Destroy(bullet);

                // wait for a bit
                yield return new WaitForSeconds(2f);
            }
        }
    }

    // method wrapper for Pathfinder - calls the method given the current objective, which determines the destinationPoint
    void PathfinderWrapper()
    {
        switch (objective)
        {
            case Objective.GoToPlayer:
                Pathfinder(player.transform.position);
                break;
            case Objective.GoToPassPoint:
                Pathfinder(player.transform.position + player.transform.up * 1000f + player.transform.forward * -4000f + player.transform.right * Random.Range(-500f, 500f));
                break;
            case Objective.GoToSpawn:
                Pathfinder(player.transform.position + player.transform.up * 1000f + player.transform.forward * 4000f + player.transform.right * Random.Range(-500f, 500f));
                break;
        }
    }
}
