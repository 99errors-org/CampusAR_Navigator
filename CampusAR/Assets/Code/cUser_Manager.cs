using System.Collections;
using System.Collections.Generic;
using TetraCreations.Attributes;
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
    [SerializeField] private float      kGPSCallTimer = 5.0f;                                  // The amount of seconds in-between making GPS calls. (5s by default)
    
    /* -------- Variables -------- */

    public Vector2                      mUserLastLocation { get; private set; }                 // The users last GPS location, used for maintaining accuracy.
    public float                        mUserLastCompassRotation { get; private set; } = 0.0f;  // The users last compass bearing, this is stored to not overwhelm the phone.
    
    private float                       mLocationTimer = 1.0f;                                  // The timer used to make GPS location calls.

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
        cNode_Manager.mInstance.HandleNodes(mUserLastLocation, 0.0f);
    }

    /* -------- Public Methods -------- */
}
