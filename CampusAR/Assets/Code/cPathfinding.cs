using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class cPathfinding : MonoBehaviour
{
    /* -------- Variables -------- */

    /* Singleton */
    public static cPathfinding mInstance;

    private List<int> mTourBuildingQueue = new List<int>();         // List of buildings the user wants to visit in  

    private List<int> mCurrentPath = new List<int>();           // Chain of nodes working from the users start position to the final target destination
    private int mCurrentPathPosition = 0;                           // Index of how far into the list Path the user has traversed
    private int mNodeReachThreshold = 20;                           // How close the user must be to the node before the node is considered to be reached

    /* -------- Unity Methods -------- */

    void Update()
    {
        if (cUser_Manager.mInstance.GetTargetNodeIndex() != cUser_Manager.kNullTargetNodeIndex)
        {
            // If there is a target selected, pathfind to the target            
            cArrowManager.mInstance.DirectArrow(cUser_Manager.mInstance.GetTargetNodeIndex());
        }
        else
        {
            //If there is not a target selected, set C+T building as the target
            //This entire else statement is a temporary addition for testing purposes, to be deleted once we have a real mechanism to set the target node
            for (int nodeIndex = 0; nodeIndex < cNode_Manager.mInstance.mBuildingNodes.Count; nodeIndex++)
            {
                if (cNode_Manager.mInstance.mBuildingNodes[nodeIndex].GetNodeName() == "Test Node")
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
    private static int FindNextNode(int currentNode, int targetNode) 
    {
        // Initialise node to return
        int nextNode = cUser_Manager.kNullTargetNodeIndex;

        // Shortest distance is set to -1 as a known null value. Distance cannot be less that 0
        float shortestDistance = -1;

        // For each node that is connected/accessible to the current node
        for (int i = 0; i < cNode_Manager.mInstance.mNodes[currentNode].GetConnectedNodes().Count; i++)
        {
            // Find the distance from the connected node to the target
            float distanceFromNodeToTarget = cGPSMaths.GetDistance(cNode_Manager.mInstance.mNodes[targetNode].GetGPSLocation(), cNode_Manager.mInstance.mNodes[i].GetGPSLocation());

            // Set the node with the shortest distance as the next node to travel to
            if (distanceFromNodeToTarget < shortestDistance || shortestDistance == -1)
            {
                // Update the current shortest distance to beat
                shortestDistance = distanceFromNodeToTarget;
                nextNode = i;
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
    private static List<int> FindNodePath(int startNode, int targetNode)
    {
        // Create a list to store all the nodes in a path from the user to the target destination
        List<int> path = new List<int>();

        // Add the start node to the path list
        path.Add(startNode);

        // Repeat until the path is finished
        while (true)
        {
            // Find the next node to travel to
            int nextNode = FindNextNode(path.Last(), targetNode);

            // If the next node is the target, pathing is complete, leave the while loop
            if (nextNode == targetNode)
            {
                path.Add(nextNode);
                break;
            }
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
    /// Runs version 2 of the pathfinding algorithm. Uses path nodes to create complex paths to the target
    /// </summary>
    /// <param name="startNode"></param>
    /// <param name="targetNode"></param>
    /// <returns></returns>
    public bool PathfindingV2(int startNode, int targetNode)
    {
        // If path is empty, create a path to the target
        if (mCurrentPath.Count == 0)
        {
            FindNodePath(startNode, targetNode); 
            mCurrentPathPosition = 0;
        }   
        
        // Point the arrow towards the target node
        cArrowManager.mInstance.DirectArrow(mCurrentPath[mCurrentPathPosition]);

        // If the user is within n meters of the node, start pathfinding to the next node
        if (DistanceToNextNode < mNodeReachThreshold)
        {
            if (mCurrentPath[mCurrentPathPosition] == targetNode)
            {
                // Target reached.
                return true;
            }
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
            return cGPSMaths.GetDistance(cUser_Manager.mInstance.mUserLastLocation, cNode_Manager.mInstance.mNodes[mCurrentPath[mCurrentPathPosition]].GetGPSLocation());
        }
    }
}
