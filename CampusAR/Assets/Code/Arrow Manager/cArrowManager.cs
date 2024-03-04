using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class cArrowManager : MonoBehaviour
{

    /* -------- Singleton -------- */
    public static cArrowManager mInstance;                                              // Singleton instance, used to reference this class globally.

    /* -------- Prefabs -------- */
    [SerializeField]
    private GameObject mArrowPrefab;                                                    // Stores the arrow prefab to be instantiated later

    /* -------- References -------- */
    private GameObject mArrow;                                                          // Stores the Arrow object not the prefab

    [SerializeField] private GameObject mCam;                                           // Stores the camera object can't get the camera without it son dont remove it

    /* -------- Constants -------- */
    private const float kDistanceInfrontOfUser = 1.0f;                                  // How much distance in front of the user to place the arrow;

    private const float kArrowYPosition = -0.4f;                                        // position the arrow a bit to the ground 

    /* -------- Unity Methods -------- */
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

    private void Start()
    {
        // Check if the arrow doesn't exist.
        if (mArrow == null)                           
        {
            // Get the correct position for the arrow.
            Vector3 _arrowPos = mCam.transform.rotation * new Vector3(mCam.transform.position.x,
                                                                      mCam.transform.position.y + kArrowYPosition,
                                                                      mCam.transform.position.z + kDistanceInfrontOfUser);

            // Instantiate the arrow and parent it to the camera.
            mArrow = Instantiate(mArrowPrefab, _arrowPos, Quaternion.identity, mCam.transform);
        }
    }

    void FixedUpdate()
    {
        // Deactivate the arrow if unused.
        mArrow.SetActive(!cPathfinding.mInstance.mCurrentPath.isEmpty());
    }

    /* -------- Public Methods -------- */

    /// <summary>
    /// Rotates the arrow to the correct position. Callable from outside the class
    /// </summary>
    public void DirectArrow(int targetNodeIndex)
    {
        cNode targetNode = cNode_Manager.mInstance.mNodes[targetNodeIndex];

        //Find the angle between the user and target
        float rotationAngle = cGPSMaths.GetAngle(cUser_Manager.mInstance.mUserLastLocation, targetNode.GetGPSLocation());

        //Correct the angle for the direction the user is facing
        rotationAngle -= cUser_Manager.mInstance.mUserLastCompassRotation;

        //Set the arrow to face the angle towards the target
        mArrow.transform.rotation = Quaternion.Euler(new Vector3(0.0f, (-rotationAngle) + 90.0f, 0.0f));
    }
}