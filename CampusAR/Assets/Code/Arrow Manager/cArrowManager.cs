using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class cArrowManager : MonoBehaviour
{

    /* Singleton */
    public static cArrowManager mInstance;                                              // Singleton instance, used to reference this class globally.

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

    private void Awake()
    {
        // Setup the singleton instance.
        if (mInstance == null)
        {
            mInstance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        mCurrentTargetNodeIndex = cUser_Manager.mInstance.GetTargetNodeIndex();          // Gets the current target nodes index 
        if (mCurrentTargetNodeIndex == cUser_Manager.kNullTargetNodeIndex)          // If the target node is not set it returns -1 
        {
            return;                                                                 // Doesn't run the updating code 
        }

        if (mArrow == null)                                                         // Checks if the arrow is instantiated                              
        {
            mArrow = Instantiate(mArrowPrefab);                                     // Instantiate arrow if it isn't
        }
    }

    void FixedUpdate()
    {
        
        if (mArrow != null && mArrow.activeSelf)                                    // Checks if the arrow is instantiated and is set to visible
        {
            // Keeps the arrow in front of the camera
            mArrow.transform.position = mCam.transform.rotation * new Vector3(mCam.transform.position.x,
                                                                              mCam.transform.position.y + kArrowYPosition,
                                                                              mCam.transform.position.z + kDistanceInfrontOfUser);
        }
    }

    // Sets the arrow to a rotated position
    public void RotateArrow(float rotationAngle)
    {
        mArrow.transform.rotation = Quaternion.Euler(new Vector3(0.0f, rotationAngle, 0.0f));   // sets the rotation of the arrow to a specific angle does not rotate it by that much
    }

    /// <summary>
    /// Rotates the arrow to the correct position. Callable from outside the class
    /// </summary>
    /// <param name="targetNode"></param>
    public static void DirectArrow(cNode targetNode)
    {
        //Find the angle between the user and target
        float rotationAngle = cGPSMaths.GetAngle(cUser_Manager.mInstance.mUserLastLocation, targetNode.GetGPSLocation());

        //Correct the angle for the direction the user is facing
        rotationAngle -= cUser_Manager.mInstance.mUserLastCompassRotation;

        //Set the arrow to face the angle towards the target
        cArrowManager.mInstance.RotateArrow(rotationAngle);
    }
}
