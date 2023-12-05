using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class cPathfinding : MonoBehaviour
{
    /* -------- Variables -------- */

    /* Singleton */
    public static cPathfinding mInstance;

    private List<cNode> mCurrentPath = new List<cNode>();   // Chain of nodes working from the users start position to the final target destination
    int mCurrentPathPosition = 0;                           // Index of how far into the list Path the user has traversed
    int mNodeReachThreshold = 20;                           // How close the user must be to the node before the node is considered to be reached

    /* -------- Unity Methods -------- */
    void Start()
    {
        
    }

    void Update()
    {
        if (cUser_Manager.mInstance.GetTargetNodeIndex() != cUser_Manager.kNullTargetNodeIndex) { RunPathfinding(cUser_Manager.mInstance.GetTargetNode()); } // If there is a target selected, pathfind to the target
        else
        {
            //If there is not a target selected, set C+T building as the target
            //This entire else statement is a temporary addition for testing purposes, to be deleted once we have a real mechanism to set the target node
            for (int nodeIndex = 0; nodeIndex < cNode_Manager.mInstance.mBuildingNodes.Count; nodeIndex++)
            {
                if (cNode_Manager.mInstance.mBuildingNodes[nodeIndex].GetBuildingName() == "Computing & Technology Building")
                {
                    cUser_Manager.mInstance.SetTargetNode(nodeIndex);
                    break;
                }
            }
        }
    }

    /* -------- Private Methods -------- */

    /// <summary>
    /// Finds the next node to travel to from the current node.
    /// </summary>
    /// <param name="currentNode"></param>
    /// <param name="targetNode"></param>
    /// <returns></returns>
    private cNode FindNextNode(cNode currentNode, cNode targetNode) 
    {
        // Initialise node to return
        cNode nextNode = cNode.nullNode;

        // Shortest distance is set to -1 as a known null value. Distance cannot be less that 0
        float shortestDistance = -1;

        // For each node that is connected/accessible to the current node
        foreach (cNode connectedNode in currentNode.GetConnectedNodes())
        {
            // Find the distance from the connected node to the target
            float distanceFromNodeToTarget = cGPSMaths.GetDistance(targetNode.GetGPSLocation(), connectedNode.GetGPSLocation());

            // Set the node with the shortest distance as the next node to travel to
            if (distanceFromNodeToTarget < shortestDistance || shortestDistance == -1)
            {
                // Update the current shortest distance to beat
                shortestDistance = distanceFromNodeToTarget;
                nextNode = connectedNode;
            }
        }

        return nextNode;
    }

    /// <summary>
    /// Finds a path of nodes from the start position to the end postion. Returns a list of nodes
    /// </summary>
    /// <param name="startNode"></param>
    /// <param name="targetNode"></param>
    /// <returns></returns>
    private List<cNode> FindNodePath(cNode startNode, cNode targetNode)
    {
        // Create a list to store all the nodes in a path from the user to the target destination
        List<cNode> path = new List<cNode>();

        // Add the start node to the path list
        path.Add(startNode);

        // Repeat until the path is finished
        while (true)
        {
            // Find the next node to travel to
            cNode nextNode = FindNextNode(path.Last(), targetNode);

            // If the next node is the target, pathing is complete, leave the while loop
            if (nextNode == targetNode) { path.Add(nextNode); break; }
            else
            {
                // Otherwise, add the next node to the list and continue pathing
                path.Add(nextNode);
            }
        }

        return path;
    }

    /* -------- Public Methods -------- */

    /// <summary>
    /// Runs the whole pathfinding algorithm and rotates the arrow to the correct position. Callable from outside the class
    /// </summary>
    /// <param name="targetNode"></param>
    public void RunPathfinding(cNode targetNode)
    {
        //Find the angle between the user and target
        float rotationAngle = cGPSMaths.GetAngle(cUser_Manager.mInstance.mUserLastLocation, targetNode.GetGPSLocation());
        
        //Correct the angle for the direction the user is facing
        rotationAngle -= cUser_Manager.mInstance.mUserLastCompassRotation;
        
        //Set the arrow to face the angle towards the target
        cArrowManager.mInstance.RotateArrow(rotationAngle);
    }

    public bool PathfindingV2(cNode startNode, cNode targetNode)
    {
        // If path is empty, create a path to the target
        if (mCurrentPath.Count == 0) { FindNodePath(startNode, targetNode); }   
        
        // Point the arrow towards the target node
        RunPathfinding(mCurrentPath[mCurrentPathPosition]);

        // If the user is within n meters of the node, start pathfinding to the next node
        if (DistanceToNextNode < mNodeReachThreshold)
        {
            if (mCurrentPath[mCurrentPathPosition] == targetNode) { return true; }  // Target reached
            else
            {
                // Move to pathfinding to the next node in the path
                mCurrentPathPosition++;
            }

        }

        return false;
    }

    // Returns the distance to the next node in the path
    public float DistanceToNextNode
    {
        get
        {
            return cGPSMaths.GetDistance(cUser_Manager.mInstance.mUserLastLocation, mCurrentPath[mCurrentPathPosition].GetGPSLocation());
        }
    }
}
