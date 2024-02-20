using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


class Path
{
    public List<int> mPath = new List<int>();               // Chain of nodes working from the users start position to the final target destination
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

        for (int i = 0; i < cNode_Manager.mInstance.mNodes[currentNode].GetConnectedNodes().Count; i++)
        {
            int k = i;
            if (cNode_Manager.mInstance.mNodes[currentNode].GetConnectedNodes()[k].GetNodeID()[0] == 'b' && cNode_Manager.mInstance.mNodes[targetNode] != cNode_Manager.mInstance.mNodes[currentNode].GetConnectedNodes()[k])
            {
                // Dont traverse through buildings, UNLESS building is the target node
                Debug.Log("Skipped a building in traversal");
            }
            else
            {
                // Find the distance from the connected node to the target
                float distanceFromNodeToTarget = Vector2.Distance(cNode_Manager.mInstance.mNodes[targetNode].GetGPSLocation(), cNode_Manager.mInstance.mNodes[currentNode].GetConnectedNodes()[k].GetGPSLocation());
                Debug.Log("Distance From Node To Target: " + distanceFromNodeToTarget.ToString() + ", {Target} " + cNode_Manager.mInstance.mNodes[targetNode].GetNodeID() + ", {Connected Node} " + cNode_Manager.mInstance.mNodes[currentNode].GetConnectedNodes()[k].GetNodeID());

                // Set the node with the shortest distance as the next node to travel to
                if (distanceFromNodeToTarget < shortestDistance || shortestDistance == -1)
                {
                    // Update the current shortest distance to beat
                    shortestDistance = distanceFromNodeToTarget;
                    nextNode = cNode_Manager.mInstance.mNodes.IndexOf(cNode_Manager.mInstance.mNodes[currentNode].GetConnectedNodes()[k]);
                }
            }          
        }

        Debug.Log("Next node (index): " + cNode_Manager.mInstance.mNodes[nextNode].GetNodeID());
        return nextNode;
    }

    public bool CreatePath(int startNode, int targetNode)
    {
        try
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
                    Debug.Log("Exit");
                    break;
                }
                else
                {
                    // Otherwise, add the next node to the list and continue pathing
                    path.Add(nextNode);
                }

                if (path.Count > 50)
                {
                    path.Clear();
                    return false;
                }
            }

            mPath = path;
            mPathPosition = 0;

            return true;
        }
        catch
        {
            return false;
        }        
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
    public int pathIndex = 0;
    public int buildingIndex = 0;

    /* Singleton */
    public static cPathfinding mInstance;
    public static float mNodeReachThreshold = 20;

    private List<int> mTourBuildingQueue = new List<int>();         // List of buildings the user wants to visit in
    private int mTourBuildingQueuePosition = 0;                     // Position of the user in the tour

    Path mCurrentPath = new Path();

    /* -------- Unity Methods -------- */

    private void Start()
    {
        // Brute force test. Test every path

        //foreach (cNode startNode in cNode_Manager.mInstance.mPathNodes)
        //{
        //    foreach (cNode endNode in cNode_Manager.mInstance.mPathNodes)
        //    {
        //        if (startNode == endNode) { continue; }

        //        Debug.Log("Testing path <startNode>" + startNode.GetNodeID() + " to <endNode>" + endNode.GetNodeID());
        //        if (mCurrentPath.CreatePath(cNode_Manager.mInstance.mNodes.IndexOf(startNode), cNode_Manager.mInstance.mNodes.IndexOf(endNode)))
        //        {
        //            string pathString = "";
        //            foreach (int node in mCurrentPath.mPath)
        //            {
        //                string pathID = cNode_Manager.mInstance.mNodes[node].GetNodeID();
        //                pathString += pathID;
        //                pathString += ", ";
        //            }
        //            Debug.Log("Test Passed - " + pathString);
        //        }
        //        else
        //        {
        //            Debug.LogError("Test Failed");
        //        }
        //    }
        //}

        //#########################
        // Manual Pathfinding Code

        //int startNode = 0;
        //int endNode = 2;

        //int _startNode = cNode_Manager.mInstance.mNodes.IndexOf(cNode_Manager.mInstance.mPathNodes[startNode]);
        //Debug.Log("Start Node (ID): " + cNode_Manager.mInstance.mNodes[_startNode].GetNodeID());
        //int _endNode = cNode_Manager.mInstance.mNodes.IndexOf(cNode_Manager.mInstance.mBuildingNodes[endNode]);
        //Debug.Log("End Node (ID): " + cNode_Manager.mInstance.mNodes[_endNode].GetNodeID());

        //mCurrentPath.CreatePath(_startNode, _endNode);
        //mCurrentPath.PrintPath();
    }

    void Update()
    {
        if (pathIndex < cNode_Manager.mInstance.mPathNodes.Count)
        {
            cNode startNode = cNode_Manager.mInstance.mPathNodes[pathIndex];
            cNode endNode = cNode_Manager.mInstance.mBuildingNodes[buildingIndex];

            Debug.Log("Testing path <startNode>" + startNode.GetNodeID() + " to <endNode>" + endNode.GetNodeID());
            if (mCurrentPath.CreatePath(cNode_Manager.mInstance.mNodes.IndexOf(startNode), cNode_Manager.mInstance.mNodes.IndexOf(endNode)))
            {
                string pathString = "";
                foreach (int node in mCurrentPath.mPath)
                {
                    string pathID = cNode_Manager.mInstance.mNodes[node].GetNodeID();
                    pathString += pathID;
                    pathString += ", ";
                }
                Debug.LogWarning("Test Passed - " + pathString);
            }
            else
            {
                Debug.LogError("Test Failed");
            }

            buildingIndex++;
            if (buildingIndex == cNode_Manager.mInstance.mBuildingNodes.Count)
            {
                buildingIndex = 0;
                pathIndex++;
            }
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
