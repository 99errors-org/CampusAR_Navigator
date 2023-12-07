using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TetraCreations.Attributes;
using TMPro;
using UnityEditor;
using UnityEngine;

public class cUser_Manager : MonoBehaviour
{
	/* -------- References -------- */

	/* Singleton */
    public static cUser_Manager         mInstance;                                              // Singleton instance, used to reference this class globally.

    /* User Interface */
    [SerializeField] private GameObject rInitPanel;                                             // The initialisation panel.
    [SerializeField] private TextMeshProUGUI rDebugText;                                        // Text used for debugging the initialisation screen.

    [SerializeField] private Transform  mCamera;                                                // Reference to the camera.

    /* -------- Constants -------- */

    private const float                 kLocationTimeout    = 20.0f;                            // The amount of seconds before the location services times out and starts again.

    public const int                    kNullTargetNodeIndex = -1;                              // The index when there is no target node selected
    /* -------- Variables -------- */

    /* GPS */
    private bool                        mCoroutine = false;

    public Vector2                      mUserLastLocation { get; private set; }                 // The users last GPS location, used for maintaining accuracy.
    public float                        mUserLastCompassRotation { get; private set; } = 0.0f;  // The users last compass bearing, this is stored to not overwhelm the phone.


    public enum kDistanceUnit { m, km, mi};                                                    // Enum for distance unit 
    private kDistanceUnit mUsersDistancePrefrence = kDistanceUnit.m;                           // Users distance unit prefrence
    /* Guiding */
    private int                         mTargetNodeIndex    = kNullTargetNodeIndex;             // The index of the target building/node, if -1 no node is selected.

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
        // Set the location and rotation of the user if debugging.
        if (Application.isEditor)
        {
            // Set debug location (Outside the Student Centre).
            mUserLastLocation = new Vector2(53.762764f, -2.707214f);

            // Set debug rotation (East)
            mUserLastCompassRotation = 90.0f;
        }

        // Initialise the location services.
        StartCoroutine(LocationCompassSetup());
    }

    private void FixedUpdate()
    {
        SetUserData();
    }

    /* -------- Coroutines -------- */

    /// <summary>
    /// Initialises the location services and compass.
    /// </summary>
    IEnumerator LocationCompassSetup()
    {
        // Check if in editor.
        if (Application.isEditor)
        {
            // Create the nodes in-world.
            cNode_Manager.mInstance.InstantiateNodes(new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude), -Input.compass.trueHeading);

            // Hide initialisation screen.
            rInitPanel.SetActive(false);

            // Exit coroutine.
            yield break;
        }

        mCoroutine = true;

        // Start location services.
        Input.location.Start();

        // Get current time.
        DateTime _initTime = DateTime.Now;

        // Check if timed out.
        while ((DateTime.Now - _initTime).TotalSeconds < kLocationTimeout)
        {
            // Check if location services started.
            if (Input.location.status == LocationServiceStatus.Running)
            {
                // Setup the compass.
                Input.compass.enabled = true;

                // Create the nodes in-world.
                cNode_Manager.mInstance.InstantiateNodes(new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude), -Input.compass.trueHeading);

                // Hide initialisation screen.
                rInitPanel.SetActive(false);
                break;
            }

            yield return null;
        }

        mCoroutine = false;
    }

    /* -------- Private Methods -------- */

    /// <summary>
    /// Makes a call to the Phones GPS functionality and gets the users current position.
    /// </summary>
    private void SetUserData()
    {
        // Check if location services is running.
        if (Input.location.status == LocationServiceStatus.Running || Application.isEditor)
        {
            
            if (!Application.isEditor) // In production.
            {
                // Check if the user is pointing their phone down.
                // Get camera angle.
                float _CamXAngle = mCamera.transform.eulerAngles.x % 360;

                if (_CamXAngle < 0)
                {
                    _CamXAngle += 360;
                }

                if (_CamXAngle > 80.0f && _CamXAngle < 100.0f)
                {
                    rDebugText.text = "Pointing Down";

                    // Set compass.
                    mUserLastCompassRotation = Mathf.Round(Mathf.LerpAngle(mUserLastCompassRotation, Input.compass.trueHeading, 10.0f * Time.deltaTime)) ;
                }
                else
                {
                    rDebugText.text = "Not Pointing Down";
                }

                // Set location.
                mUserLastLocation = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
            }

            // Call the node handler.
            cNode_Manager.mInstance.CorrectNodes(mUserLastLocation, -mUserLastCompassRotation);
        }
        else // Location Services not running.
        {
            switch (Input.location.status)  
            {
                case LocationServiceStatus.Initializing: // Initialising.
                {
                    // Do nothing.
                    break;
                }
                case LocationServiceStatus.Stopped:
                case LocationServiceStatus.Failed:
                {
                    // Restart the Location and Compass setup.
                    if (!mCoroutine)
                    {
                        // Stop the location service.
                        Input.location.Stop();

                        StartCoroutine(LocationCompassSetup());
                    }

                    break;
                }
            }
        }
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

    public int GetTargetNodeIndex()              // Returns the target nodes index if it is there or returns -1 if there is no target node
    {
        return mTargetNodeIndex;
    }

    public cNode GetTargetNode()
    {
        return cNode_Manager.mInstance.GetNodes[mTargetNodeIndex];
    }

    public kDistanceUnit GetUserDistnacePrefrence()
    {
        return mUsersDistancePrefrence;
    }
}