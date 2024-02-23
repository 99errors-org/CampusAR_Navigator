using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class cDistanceText : MonoBehaviour
{
    /*-------Constants----------*/
    private const float kMeterToMiles = 0.00062137f;
    private const float kMeterToKilometer = 0.001f;
    private const float kMeterToMeter = 1.0f;

    private const string kMeterAbbreviation = "m";
    private const string kKilometerAbbreviation = "km";
    private const string kMilesAbbreviation = "mi";

    /*------ Variables ---------*/

    [SerializeField]
    private TextMeshProUGUI mDistanceText;                                          // Text used to diplay ditance to destination
    private int mCurrentTargetNodeIndex = cUser_Manager.kNullTargetNodeIndex;       // Sets the target node index to -1 
    private float mDistanceToDestination;                                           // Stores the distance to the destination
    private string mSelectedDistanceUnit = kMeterAbbreviation;                      // Stores the selected unit abbreviation default is meter
    private float mCurrentUnitMultiplier = kMeterToMeter;                           // Stores the selected unit conversion rate default is meter
    // Start is called before the first frame update
    void Start()
    {
        mDistanceText.gameObject.SetActive(false);                                  // Sets the text to inactive when starting
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        mCurrentTargetNodeIndex = cUser_Manager.mInstance.GetTargetNodeIndex();     // Gets the current target nodes index 
        if (mCurrentTargetNodeIndex == cUser_Manager.kNullTargetNodeIndex)          // If the target node is not set it returns -1 
        {
            return;                                                                 // Doesn't run the updating code 
        }

        if (mDistanceText != null & mDistanceText.gameObject.activeSelf)
        {
            setDistance(cPathfinding.mInstance.GetDistanceToNextNode());                 // Sets the distance text to the distance and converts it to users prefrence
        }
        else
        {
            mDistanceText.gameObject.SetActive(true);                               // If the distanceText isnt active setting it active
        }

    }

    /* Converts the value from meters to users preferred unit */
    public void setUnit()
    {


        switch (cUser_Manager.mInstance.GetUserDistnacePrefrence())                 // Sets user preferred unit and converts it
        {
            case cUser_Manager.kDistanceUnit.m:
                mSelectedDistanceUnit = kMeterAbbreviation;
                mCurrentUnitMultiplier = kMeterToMeter;
                break;
            case cUser_Manager.kDistanceUnit.km:
                mSelectedDistanceUnit = kKilometerAbbreviation;
                mCurrentUnitMultiplier = kMeterToKilometer;
                break;
            case cUser_Manager.kDistanceUnit.mi:
                mSelectedDistanceUnit = kMilesAbbreviation;
                mCurrentUnitMultiplier = kMeterToMiles;
                break;

        }

    }

    /* Sets the distance text and converts the value from meters to users preferred unit */
    private void setDistance(float distanceInMeters)
    {
        mDistanceToDestination = distanceInMeters * mCurrentUnitMultiplier;
        mDistanceText.text = mDistanceToDestination.ToString() + mSelectedDistanceUnit;     // Setting the the text on the screen
    }
}
