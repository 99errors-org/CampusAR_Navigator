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
    private cNode FindNextNode(cNode startNode, cNode targetNode) 
    {
        cNode nextNode = cNode.nullNode;

        float shortestDistance = -1;

        foreach (cNode connectedNode in targetNode.GetConnectedNodes())
        {
            float distanceFromNodeToTarget = cGPSMaths.GetDistance(targetNode.GetGPSLocation(), connectedNode.GetGPSLocation());
            if (distanceFromNodeToTarget < shortestDistance || shortestDistance == -1)
            {
                shortestDistance = distanceFromNodeToTarget;
                nextNode = connectedNode;
            }
        }

        return nextNode;
    }

    private List<cNode> FindNodePath(cNode startNode, cNode targetNode)
    {
        List<cNode> path = new List<cNode>();

        path.Add(startNode);
        while (true)
        {
            cNode nextNode = FindNextNode(path.Last(), targetNode);
            if (nextNode == targetNode) { path.Add(nextNode); break; }
            else
            {
                path.Add(nextNode);
            }
        }

        return path;
    }

    /* -------- Public Methods -------- */

    /// <summary>
    /// Runs the whole pathfinding algorithm and rotates the arrow to the correct position. Callable from outside the class
    /// </summary>
    public void RunPathfinding(cNode targetNode)
    {
        //Find the angle between the user and target
        float rotationAngle = cGPSMaths.GetAngle(cUser_Manager.mInstance.mUserLastLocation, targetNode.GetGPSLocation());
        
        //Correct the angle for the direction the user is facing
        rotationAngle -= cUser_Manager.mInstance.mUserLastCompassRotation;
        
        //Set the arrow to face the angle towards the target
        cArrowManager.mInstance.RotateArrow(rotationAngle);
    }
}
