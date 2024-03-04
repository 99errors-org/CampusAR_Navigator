using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TetraCreations.Attributes;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class cUser_Manager : MonoBehaviour
{
	/* -------- References -------- */

	/* Singleton */
    public static cUser_Manager                 mInstance;                                              // Singleton instance, used to reference this class globally.

    /* User Interface */

        /* Init Screen */
    [SerializeField] private GameObject         rInitPanel;                                             // The Initialisation panel.
    [SerializeField] private TextMeshProUGUI    rDebug_CurrentNodeText;                                 // Text used for debugging the initialisation screen.

        /* Calibration Screen */
    [SerializeField] private Transform          rCam;                                                   // Reference to the camera.

    [SerializeField] private GameObject         rCalibrationScreen;                                     // The Calibration panel.
    [SerializeField] private TextMeshProUGUI    rCalibrationText;                                       // The text displayed on the calibration screen.
    [SerializeField] private Sprite[]           rPhoneStateSprites;                                     // Array of sprites, which are used to indicate the phone state.
    [SerializeField] private Image              rPhoneStateImage;                                       // The Image that displays the phones current state.

    /* -------- Constants -------- */

    public const int                            kNullTargetNodeIndex = -1;                              // The index when there is no target node selected

    private const float                         kLocationTimeout = 20.0f;                               // The amount of seconds before the location services times out and starts again.
    private const float                         kCalibrationTime = 3.0f;                                // The amount of time it takes to calibrate the north offset.   
    
    /* -------- Variables -------- */

    /* GPS */
    private bool                                mCoroutine = false;

    public Vector2                              mUserLastLocation { get; private set; }                 // The users last GPS location, used for maintaining accuracy.
    public float                                mUserLastCompassRotation { get; private set; } = 0.0f;  // The users last compass bearing, this is stored to not overwhelm the phone.


    public enum kDistanceUnit { m, km, mi};                                                    // Enum for distance unit 
    private kDistanceUnit mUsersDistancePrefrence = kDistanceUnit.m;                           // Users distance unit prefrence
    /* Guiding */
    private int                                 mTargetNodeIndex = kNullTargetNodeIndex;                // The index of the target building/node, if -1 no node is selected.

    /* Calibration */
    public float                                mNorthOffset { get; private set; } = 0.0f;              // The last north offset.

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
            cNode_Manager.mInstance.InstantiateNodes(mUserLastLocation);

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
                // Create the nodes in-world.
                cNode_Manager.mInstance.InstantiateNodes(new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude));

                // Calibrate the compass.
                StartCoroutine(CalibrateNorth());

                break;
            }

            yield return null;
        }

        mCoroutine = false;
    }

    /// <summary>
    /// Calibrates the phones compass and gets the north bearing in relation to the Unity In World Forward axis.
    /// </summary>
    IEnumerator CalibrateNorth()
    {
        // Open the calibration screen.
        rCalibrationScreen.SetActive(true);

        // Start the counter.
        DateTime _initTime = DateTime.Now;

        // Check if gyroscope is supported.
        if (SystemInfo.supportsGyroscope)
        {
            // Enable Gyroscope.
            Input.gyro.enabled = true;
        }

        // Enable the compass
        Input.compass.enabled = true;

        // While the timer is above 0.
        while ((DateTime.Now - _initTime).TotalSeconds < kCalibrationTime)
        {
            // Check if user is pointing down.
            if (CheckPhoneIsFlat())
            {
                // Set the image.
                rPhoneStateImage.sprite = rPhoneStateSprites[0];

                // Set the text.
                rCalibrationText.text = "Phone calibrating, please wait.";

            }
            else // Phone is not facing down.
            {
                // Set the image.
                rPhoneStateImage.sprite = rPhoneStateSprites[1];

                // Set the text.
                rCalibrationText.text = "Hold the phone flat to calibrate!";

                // Reset the timer.
                _initTime = DateTime.Now;
            }

            yield return null;
        }

        // Once the timer is 0, set the north.
        mNorthOffset = Input.compass.trueHeading;

        // Close the calibration screen.
        rCalibrationScreen.SetActive(false);
    }

    /* -------- Private Methods -------- */

    /// <summary>
    /// Checks if the phone is flat.
    /// </summary>
    private bool CheckPhoneIsFlat()
    {
        // Check if the phone supports Gyroscope.
        if (SystemInfo.supportsGyroscope)
        {
            if (Input.gyro.attitude.x > -0.1f && Input.gyro.attitude.x < 0.1f)
            {
                return true;
            }
        }
        else // Gyro not supported, use AR camera.
        {
            // Get camera angle.
            float _CamXAngle = rCam.transform.eulerAngles.x % 360;

            if (_CamXAngle < 0)
            {
                _CamXAngle += 360;
            }

            if (_CamXAngle > 80.0f && _CamXAngle < 100.0f)
            {
                return true;
            }
        }

        return false;
    }

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
                

                // Set location.
                mUserLastLocation = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
            }

            // Call the node handler.
            cNode_Manager.mInstance.CorrectNodes(mUserLastLocation);
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

    // returns users last location to diplay building information
    public Vector2 GetUserLocation()
    {
        return mUserLastLocation;
    }
    public kDistanceUnit GetUserDistnacePrefrence()
    {
        return mUsersDistancePrefrence;
    }
}