using System.Collections;
using System.Collections.Generic;
using TetraCreations.Attributes;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class cUser_Manager : MonoBehaviour
{
	/* -------- References -------- */

	/* Singleton */
    public static cUser_Manager         mInstance;                                              // Singleton instance, used to reference this class globally.


    /* -------- Constants -------- */
    [Title("GPS Values")]
    [SnappedSlider(1.0f, 1.0f, 60.0f)]
    [SerializeField] private float      kGPSCallTimer = 5.0f;                                   // The amount of seconds in-between making GPS calls. (5s by default)
    public const int                    kNullTargetNodeIndex = -1;                              // -1 no node is selected
    /* -------- Variables -------- */

    /* GPS */
    public Vector2                      mUserLastLocation { get; private set; }                 // The users last GPS location, used for maintaining accuracy.
    public float                        mUserLastCompassRotation { get; private set; } = 0.0f;  // The users last compass bearing, this is stored to not overwhelm the phone.
    
    private float                       mLocationTimer = 1.0f;                                  // The timer used to make GPS location calls

    /* Guiding */
    private int                         mTargetNodeIndex = kNullTargetNodeIndex;                // The index of the target building/node, if -1 no node is selected.

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

        // If in editor, use a generic location.
        if (Application.isEditor)
        {
            // Generic user location set while in editor, this is the Square outside of Student Centre.
            mUserLastLocation = new Vector2(53.762764f, -2.707214f);
        }
    }

    private void Update()
    {
        GPSTimer();

        if (mTargetNodeIndex != kNullTargetNodeIndex) { TargetPathfinding(); }
        else
        {
            int i = 0;
            foreach (cNode node in cNode_Manager.mInstance.GetNodes)
            {
                if (node.GetBuildingName() == "Computing & Technology Building") { mTargetNodeIndex = i; break; }
                i++;
            }
        }
    }

    /* -------- Private Methods -------- */

    /// <summary>
    /// Checks the time between GPS calls and makes a new call if necessary.
    /// </summary>
    private void GPSTimer()
    {
        // Check timer.
        if (mLocationTimer <= 0.0f)
        {
            // Get the users location.
            GetUserLocation();

            // Reset timer.
            mLocationTimer = kGPSCallTimer;
        }
        else
        {
            mLocationTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Makes a call to the Phones GPS functionality and gets the users current position.
    /// </summary>
    private void GetUserLocation()
    {
        // User the phones GPS location to request the users location.

        // Get the users rotation based on their compass.

        // Position the nodes once user location has been received.
        cNode_Manager.mInstance.HandleNodes(mUserLastLocation, mUserLastCompassRotation);
    }

    /* -------- Public Methods -------- */

    /// <summary>
    /// Sets the target node that the user will be guided to. Set to -1 to disable guiding.
    /// </summary>
    /// <param name="_index">The index of the building/node from the list of nodes in cNode_Manager.</param>
    public void SetTargetNode(int _index)
    {
        mTargetNodeIndex = _index;
    }

    // Returns the target nodes index if it is there or returns -1 if there is no target node
    public int GetTargetNode()
    {
        return mTargetNodeIndex;
    }

    /// <summary>
    ///  Runs the pathfinding between the users current location and the target location.
    ///  Kept modular to help Iain's brain when we inevitably upgrade it. I am a simple man.
    /// </summary>
    public void TargetPathfinding()
    {
        float rotationAngle = cGPSMaths.GetAngle(mUserLastLocation, cNode_Manager.mInstance.GetNodes[mTargetNodeIndex].GetGPSLocation());
        cArrowManager.mInstance.RotateArrow(rotationAngle);
    }
}