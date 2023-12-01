using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cPathfinding : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void RunPathfinding()
    {
        //Find the angle between the user and target
        float rotationAngle = cGPSMaths.GetAngle(cUser_Manager.mInstance.mUserLastLocation, cNode_Manager.mInstance.GetNodes[cUser_Manager.mInstance.GetTargetNode()].GetGPSLocation());
        
        //Correct the angle for the direction the user is facing
        rotationAngle -= cUser_Manager.mInstance.mUserLastCompassRotation;
        
        //Set the arrow to face the angle towards the target
        cArrowManager.mInstance.RotateArrow(rotationAngle);
    }
}
