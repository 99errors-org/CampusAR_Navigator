using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cPathfinding : MonoBehaviour
{

    /* -------- Variables -------- */

    /* Singleton */
    public static cPathfinding mInstance;

    private cNode mCurrentTarget;                           // The node you are currently navigating to. This could be 1 of many nodes in a chain forming a path to the final target
    private cNode mFinalTarget { get { return cNode_Manager.mInstance.GetNodes[cUser_Manager.mInstance.GetTargetNode()]; } }                             // The final destination node
    private List<cNode> mCurrentPath = new List<cNode>();   // Chain of nodes working from the users start position to the final target destination


    /* -------- Unity Methods -------- */
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    /* -------- Private Methods -------- */
    
    /// <summary>
    /// Finds the next node to travel to from the current node.
    /// </summary>
    public void FindNextNode() 
    {

    }

    /* -------- Public Methods -------- */

    /// <summary>
    /// Runs the whole pathfinding algorithm. Callable from outside the class
    /// </summary>
    public void RunPathfinding()
    {
        //Find the angle between the user and target
        float rotationAngle = cGPSMaths.GetAngle(cUser_Manager.mInstance.mUserLastLocation, mCurrentTarget.GetGPSLocation());
        
        //Correct the angle for the direction the user is facing
        rotationAngle -= cUser_Manager.mInstance.mUserLastCompassRotation;
        
        //Set the arrow to face the angle towards the target
        cArrowManager.mInstance.RotateArrow(rotationAngle);
    }
}
