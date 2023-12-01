using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class cArrowManager : MonoBehaviour
{
    [SerializeField]
    private GameObject mArrowPrefab;                                                    // Stores the arrow prefab to be instantiated later

    private GameObject mArrow;                                                          // Stores the Arrow object not the prefab

    [SerializeField]
    private GameObject mCam;                                                            // Stores the camera object can't get the camera without it son dont remove it

    /*------ Constants ---------*/
    private const float kDistanceInfrontOfUser = 7.12f;                                 // How much distance in front of the user to place the arrow;

    private const float kArrowYPosition = -1.04f;                                       // position the arrow a bit to the ground
                   
    /*------ Variables ---------*/
    private int mCurrentTargetNodeIndex = cUser_Manager.kNullTargetNodeIndex;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mCurrentTargetNodeIndex = cUser_Manager.mInstance.GetTargetNode();                     // Gets the current target nodes index /If the target node is not set it returns -1 
        if (mCurrentTargetNodeIndex == cUser_Manager.kNullTargetNodeIndex)
        {
            return;                                                                     // Doesn't run the updating code 
        }
        
        // Checks if the arrow is instantiated
        if (mArrow == null)                                                        // Instantiate arrow if it isn't
        {
            mArrow = Instantiate(mArrowPrefab);                 
        }
    }

    void FixedUpdate()
    {
        // Keeps the arrow in front of the camera
        if (mArrow != null && mArrow.activeSelf)
        {
            mArrow.transform.position = mCam.transform.rotation * new Vector3(mCam.transform.position.x,
                                                                              mCam.transform.position.y + kArrowYPosition, 
                                                                              mCam.transform.position.z + kDistanceInfrontOfUser);
        }
    }

    // Sets the arrow to a rotated position
    public void RotateArrow(float rotationAngle)
    {
        mArrow.transform.rotation = Quaternion.Euler(0.0f, rotationAngle, 0.0f);
    }
}
