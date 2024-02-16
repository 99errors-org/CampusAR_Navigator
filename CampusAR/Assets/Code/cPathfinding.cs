using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Unity.VisualScripting;


class Path
{
    private List<int> mPath = new List<int>();               // Chain of nodes working from the users start position to the final target destination
    private int mPathPosition = 0;                           // Index of how far into the list Path the user has traversed

    public Path() 
    {
        mPathPosition = 0;
    }
    public Path(List<int> newPath)
    {
        mPath = newPath;
        mPathPosition = 0;
    }

    public bool isEmpty()
    {
        if (mPath.Count == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public int GetCurrentNode()
    {
        if (!isEmpty() && mPathPosition < mPath.Count())
        {
            return mPath[mPathPosition];
        }
        else
        {
            return 0;
        }
    }
    public void CurrentNodeReached()
    {
        mPathPosition++;
    }

    // Finds the next node to travel to from the current node.
    private static int FindNextNode(int currentNode, int targetNode)
    {
        // Initialise node to return
        int nextNode = cUser_Manager.kNullTargetNodeIndex;

        // Shortest distance is set to -1 as a known null value. Distance (scalar) cannot be less that 0
        float shortestDistance = -1;

        //// For each node that is connected/accessible to the current node
        Debug.Log("Connected Node Count: " + cNode_Manager.mInstance.mNodes[currentNode].GetConnectedNodes().Count.ToString());

        string conNodeStr = "";
        foreach (cNode node in cNode_Manager.mInstance.mNodes[currentNode].GetConnectedNodes())
        {
            conNodeStr += node.GetNodeID() + ", ";
        }
        Debug.Log(conNodeStr);


        // looping through every connected node
        for (int i = 0; i < cNode_Manager.mInstance.mNodes[currentNode].GetConnectedNodes().Count; i++)
        {
            Debug.Log("Checking connected node: " + cNode_Manager.mInstance.mNodes[currentNode].GetConnectedNodes()[i].GetNodeID());
            // Find the distance from the connected node to the target
            Debug.Log("Target node: " + cNode_Manager.mInstance.mNodes[targetNode].GetNodeID());
            Vector3 targetLocation = cNode_Manager.mInstance.mNodes[targetNode].GetUnityLocation();

            Vector3 currConnectedNodeLocation = cNode_Manager.mInstance.mNodes[currentNode].GetConnectedNodes()[i].GetUnityLocation();

            Debug.Log("Target GPS: " + targetLocation.ToString());
            Debug.Log("current Connected Node GPS: " + currConnectedNodeLocation.ToString());

            float distanceFromNodeToTarget = cGPSMaths.GetUnityDistance(currConnectedNodeLocation, targetLocation);
            Debug.Log("Distance From Node To Target: " + distanceFromNodeToTarget.ToString() + ", " + cNode_Manager.mInstance.mNodes[currentNode].GetConnectedNodes()[i].GetNodeID());

            // Set the node with the shortest distance as the next node to travel to
            if (distanceFromNodeToTarget < shortestDistance || shortestDistance == -1)
            {
                // Update the current shortest distance to beat
                shortestDistance = distanceFromNodeToTarget;
                nextNode = cNode_Manager.mInstance.mNodes.IndexOf(cNode_Manager.mInstance.mNodes[currentNode].GetConnectedNodes()[i]);
            }
        }

        Debug.Log("Next node (index): " + cNode_Manager.mInstance.mNodes[nextNode].GetNodeID());
        return nextNode;
    }

    public void CreatePath(int startNode, int targetNode)
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
                Debug.Log("EXXXXXIIIIITTTT!!!!!");
                break;
            }
            else
            {
                // Otherwise, add the next node to the list and continue pathing
                path.Add(nextNode);
            }
        }

        mPath = path;
        mPathPosition = 0;
    }

    public void PrintPath()
    {
        string pathString = "";
        foreach (int node in mPath)
        {
            string pathID = cNode_Manager.mInstance.mNodes[node].GetNodeID();
            pathString += pathID;
            pathString += ", ";
        }
        Debug.Log("Path Chain: " + pathString);
    }
}

public class cPathfinding : MonoBehaviour
{
    /* -------- Variables -------- */

    public bool doOnce = true;

    /* Singleton */
    public static cPathfinding mInstance;
    public static float mNodeReachThreshold = 20;

    private List<int> mTourBuildingQueue = new List<int>();         // List of buildings the user wants to visit in
    private int mTourBuildingQueuePosition = 0;                     // Position of the user in the tour

    Path mCurrentPath = new Path();

    /* -------- Unity Methods -------- */

    void Update()
    {
        if (doOnce)
        {
            int startNode = 0;
            int endNode = 21;

            int _startNode = cNode_Manager.mInstance.mNodes.IndexOf(cNode_Manager.mInstance.mPathNodes[startNode]);
            Debug.Log("Start Node (ID): " + cNode_Manager.mInstance.mNodes[_startNode].GetNodeID());
            int _endNode = cNode_Manager.mInstance.mNodes.IndexOf(cNode_Manager.mInstance.mPathNodes[endNode]);
            Debug.Log("End Node (ID): " + cNode_Manager.mInstance.mNodes[_endNode].GetNodeID());

            mCurrentPath.CreatePath(_startNode, _endNode);
            mCurrentPath.PrintPath();

            doOnce = false;
        }
    }

    /* -------- Private Methods -------- */

    

    /* -------- Public Methods -------- */

    /// <summary>
    /// Runs version 2 of the pathfinding algorithm. Uses path nodes to create complex paths to the target
    /// </summary>
    /// <param name="startNode"></param>
    /// <param name="targetNode"></param>
    /// <returns></returns>
    public bool RunBuildingPathfinding(int targetNode)
    {
        // If path is empty, create a path to the target
        if (mCurrentPath.isEmpty())
        {
            mCurrentPath.CreatePath(GetClosestNode(), targetNode); 
        }   
        
        // Point the arrow towards the target node
        cArrowManager.mInstance.DirectArrow(mCurrentPath.GetCurrentNode());

        // If the user is within n meters of the node, start pathfinding to the next node
        if (DistanceToNextNode < mNodeReachThreshold)
        {
            if (mCurrentPath.GetCurrentNode() == targetNode)
            {
                // Target reached.
                return true;
            }
            else
            {
                // Move to pathfinding to the next node in the path
                mCurrentPath.CurrentNodeReached();
            }

        }

        return false;
    }

    public bool RunTourPathfinding()
    {
        // If there is a building tour queue
        if (mTourBuildingQueue.Count > 0)
        {
            // Run the pathfinding to the building in the queue
            if (RunBuildingPathfinding(mTourBuildingQueue[mTourBuildingQueuePosition])) 
            { 
                // When it reaches the target building in the queue, move to the next building in the queue
                mTourBuildingQueuePosition++; 
            }

            // If the user visits everything in the queue
            if (mTourBuildingQueuePosition > mTourBuildingQueue.Count)
            {
                // Exit pathfinding
                return true;
            }
        }

        return false;
    }

    // Returns the distance to the next node in the path
    public float DistanceToNextNode
    {
        get
        {
            return cGPSMaths.GetDistance(cUser_Manager.mInstance.mUserLastLocation, cNode_Manager.mInstance.mNodes[mCurrentPath.GetCurrentNode()].GetGPSLocation());
        }
    }

    // Returns the closest node to the users last GPS position
    public int GetClosestNode()
    {
        // Set initial values to known bad values
        int closestNode = -1;
        float closestDistance = 100000;

        for (int i = 0; i < cNode_Manager.mInstance.mNodes.Count; i++)
        {
            // For each node in the mNodes list, check its distance from the user
            float distanceToNode = cGPSMaths.GetDistance(cUser_Manager.mInstance.mUserLastLocation, cNode_Manager.mInstance.mNodes[i].GetGPSLocation());
            if (distanceToNode<closestDistance)
            {
                // If it's distance is the new closest distance, set this node to be the current closest node
                closestNode = i;
                closestDistance = distanceToNode;
            }
        }

        return closestNode;
    }
}
