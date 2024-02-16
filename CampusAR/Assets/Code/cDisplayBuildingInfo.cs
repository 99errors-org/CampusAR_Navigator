using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using Unity.Tutorials.Core.Editor;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;

public class cDisplayBuildingInfo : MonoBehaviour
{
    [SerializeField] private GameObject pNode_Building;          // Prefab for the building nodes, used when generating the map.))

    private TextMeshPro _tmp_text;
    private float mTimer;                                       // stores the time from to see if certain time has passed
    private float mDelayTime = 5.0f;                               // Time to pause between calling the building function

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
    void DisplayBuildingText()
    {
        float _distanceFromUser;
        for (int i = 0; i < cNode_Manager.mInstance.mBuildingNodes.Count; i++)
        {
            GameObject _node = cNode_Manager.mInstance.mWorldNodes[i];
            _tmp_text = _node.gameObject.transform.Find("Text (TMP)").GetComponent<TextMeshPro>();
            cNode _tmp_building = cNode_Manager.mInstance.mNodes[i];
            _distanceFromUser = cGPSMaths.GetDistance(cUser_Manager.mInstance.mUserLastLocation, _tmp_building.GetGPSLocation());
            if (_tmp_text != null)
            {
                // If the building is near display the text
                if (_distanceFromUser < cPathfinding.mInstance.mNodeReachThreshold)
                {
                    _tmp_text.gameObject.SetActive(true);  
                }
                else
                {
                    if (_tmp_text.gameObject.activeSelf)
                    {
                        _tmp_text.gameObject.SetActive(false);
                    }
                }    
            }
            else
            {
                Debug.LogError("TextMeshPro component not found on the instantiated node.");
            }
        }
    }

}
