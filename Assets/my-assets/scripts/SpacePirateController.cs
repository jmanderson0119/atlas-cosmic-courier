using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script will control the behavior of the enemy bots, whose objective it is to destroy the player, 
/// avoid collisions, and smoothly loop around the player in a dogfighting style.
/// 
/// The bot will take into consideration the destination point: the player, the location in which they were instantiated relative to the player (spawn),
/// and the point in which the enemy should begin to turn around and head back to the spawn to pass by the player once more and fire (the pass-point).
/// 
/// Upon receiving this information, the enemy will use an A* search designed procedure to reach their destination point and, in some cases, perform other actions
/// during flight, such as shooting at the player and rolling in space. 
/// 
/// The enemy at all stages will be lerping their pitch and yaw so as to smoothly transition between objective states and smoothly avoid collisions with other objects.
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
    [SerializeField] private float speed; // enemy speed
    [SerializeField] private float pitchSpeed; // enemy pitch speed
    [SerializeField] private float yawSpeed; // enemy yaw speed
    [SerializeField] private Vector3 nodeFieldSize; // size of the box cast being performed for each candidate node
    [SerializeField] private LayerMask layerMask; // layers that will not be detected by the box cast

    private List<Node> pathList; // A* path enemy is currently traversing
    private RaycastHit raycastData; // raycast information for candidate nodes
    #endregion

    #region Objective Enum
    // objective states: defines a destination point, which further defines actions to perform during flight
    public enum Objective
    {
        GoToPlayer,
        GoToPassPoint,
        GoToSpawn
    }
    #endregion

    #region Node Comparer Class
    // comparer type for candidateList, useful for getting Node with smallest f values quickly
    private class NodeComparer : IComparer<Node>
    {
        public int Compare(Node firstNode, Node secondNode)
        {
            return firstNode.getF().CompareTo(secondNode.getF());
        }
    }
    #endregion

    #region Node Class
    private class Node
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
        public void setH(float h) { this.h = h; }
        public void setF() { this.f = this.g + this.h; }
    }
    #endregion

    public List<Vector3> Pathfinder(Vector3 enemyPosition, Vector3 destinationPoint)
    {
        // Initialize the open list of node candidates for the final path
        SortedList<Node> candidateList = new SortedList<Node>(new NodeComparer());
        candidateList.Add(new Node(null, transform.position));

        // Initialize the final path
        List<Vector3> traversalList = new List<Vector3>();

        // Initialize the counting digits for generating each node candidate to be added to candidateList
        int num1 = -1, num2 = -1, num3 = -1;

        // Initialize nodeList for inspecting nodes surrounding currentNode
        List<Node> nodeList = new List<Node>();

        // during each loop, the node corresponding to the shortest path will be added to traversalList
        while (candidateList.Count != 0)
        {
            // remove the node with the smallest f value
            Node currentNode = candidateList.Dequeue();
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
                    // if destinationPoint resides in the field represented by nodeCandidate, this node will be added to the traversalList and the search is over
                    if (destinationPoint.x - nodeCandidate.getCenterPosition().x < 400f && destinationPoint.y - nodeCandidate.getCenterPosition().y < 400f && destinationPoint.z - nodeCandidate.getCenterPosition().z < 400f)
                    {
                        traversalList.Add(nodeCandidate.getCenterPosition()); // final node containing the enemy destination, completing the generated path
                        
                        return traversalList; // output the optimal path
                    }

                    nodeCandidate.setH(Vector3.Distance(nodeCandidate.getCenterPosition(), destinationPoint));
                    nodeCandidate.setF();
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }
}
