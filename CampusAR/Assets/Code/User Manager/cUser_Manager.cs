using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private TextMeshProUGUI rDebug_CurrentNodeText;                            // Text used for debugging the initialisation screen.

    [SerializeField] private Transform  mCamera;                                                // Reference to the camera.

    /* -------- Constants -------- */

    private const float                 kLocationTimeout = 20.0f;                               // The amount of seconds before the location services times out and starts again.

    public const int                    kNullTargetNodeIndex = -1;                              // The index when there is no target node selected
    /* -------- Variables -------- */

    /* GPS */
    private bool                        mCoroutine = false;

    public Vector2                      mUserLastLocation { get; private set; }                 // The users last GPS location, used for maintaining accuracy.
    public float                        mUserLastCompassRotation { get; private set; } = 0.0f;  // The users last compass bearing, this is stored to not overwhelm the phone.

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
    }

    private void Start()
    {
        // Set the location and rotation of the user if debugging.
        if (Application.isEditor)
        {
            // Set debug location (Outside the Student Centre).
            mUserLastLocation = new Vector2(53.762764f, -2.707214f);

            // Set debug rotation (East)
            mUserLastCompassRotation = 0.0f;
        }

        // Initialise the location services.
        StartCoroutine(LocationCompassSetup());
    }

    private void FixedUpdate()
    {
        SetUserData();
        Debug_CurrentNode();
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
            cNode_Manager.mInstance.InstantiateNodes(mUserLastLocation, mUserLastCompassRotation);

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
                    //rDebugText.text = "Pointing Down";

                    // Set compass.
                    mUserLastCompassRotation = Mathf.Round(Mathf.LerpAngle(mUserLastCompassRotation, Input.compass.trueHeading, 10.0f * Time.deltaTime)) ;
                }
                else
                {
                    //rDebugText.text = "Not Pointing Down";
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

    private void Debug_CurrentNode()
    {
        // Check if pointing at a building.
        if (mTargetNodeIndex == kNullTargetNodeIndex)
        {
            rDebug_CurrentNodeText.text = "No Node Selected";
        }
        else
        {
            rDebug_CurrentNodeText.text = "Current: " + cNode_Manager.mInstance.mNodes[mTargetNodeIndex].GetNodeName() +
                                            "\n" + "Distance: " + 
                                            Mathf.Round(cGPSMaths.GetDistance(mUserLastLocation, cNode_Manager.mInstance.mNodes[mTargetNodeIndex].GetGPSLocation())) + 
                                            "m";
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

    /// <summary>
    /// Returns the target nodes index if it is there or returns -1 if there is no target node
    /// </summary>
    public int GetTargetNodeIndex()
    {
        return mTargetNodeIndex;
    }

    /// <summary>
    /// Returns the target node, as a cNode object.
    /// </summary>
    /// <returns></returns>
    public cNode GetTargetNode()
    {
        return cNode_Manager.mInstance.mNodes[mTargetNodeIndex];
    }

    /// <summary>
    /// Changes the currently selected node, increments down the list.
    /// </summary>
    /// <param name="_increment"></param>
    public void ChangeCurrentNode(bool _increment)
    {
        // Check if incrementing.
        if (_increment)
        {
            if (mTargetNodeIndex < (cNode_Manager.mInstance.mNodes.Count - 1))
            {
                mTargetNodeIndex++;
            }
        }
        else
        {
            if (mTargetNodeIndex >= 0)
            {
                mTargetNodeIndex--;
            }
        }
    }
}