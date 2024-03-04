using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class cDisplayBuildingInfo : MonoBehaviour
{
    private TextMeshPro _tmp_text;                              // Stores the text object of the current building in loop
    private float mTimer;                                       // Stores the time from to see if certain time has passed
    private float mDelayTime = 5.0f;                            // Time to pause between calling the building function

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        mTimer += Time.deltaTime;

        if (mTimer > mDelayTime) 
        {
            DisplayBuildingText();

            //Subtracting the time interval from the elapsed time accounts for any excess time that may
            //have passed since the timer became larger than the required delay.
            mTimer -= mDelayTime;
        }
    }


    // Checks if the building is near then sets building text to active
    private void DisplayBuildingText()
    {
        float _distanceFromUser;
        for (int i = 0; i < cNode_Manager.mInstance.mBuildingNodes.Count; i++)
        {
            GameObject _node = cNode_Manager.mInstance.mWorldNodes[i];

            // Current nodes text component
            _tmp_text = _node.gameObject.transform.Find("Text (TMP)").GetComponent<TextMeshPro>();

            // Current building
            cNode _tmp_building = cNode_Manager.mInstance.mNodes[i];

            // Distance from the user to the building
            _distanceFromUser = cGPSMaths.GetDistance(
                                                cUser_Manager.mInstance.mUserLastLocation, 
                                                _tmp_building.GetGPSLocation());
            if (_tmp_text != null)
            {
                // If the building is out of the radius hide the text
                if (_distanceFromUser >= cPathfinding.mNodeReachThreshold)
                {
                    if (_tmp_text.gameObject.activeSelf)
                    {
                        _tmp_text.gameObject.SetActive(false);
                    }
                }
                else
                {
                    _tmp_text.gameObject.SetActive(true);
                }
            }
            else
            {
                Debug.LogError("TextMeshPro component not found on the instantiated node.");
            }
        }
    }

}
