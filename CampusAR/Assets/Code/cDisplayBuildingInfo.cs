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
    [SerializeField] private GameObject pNode_Building;                             // Prefab for the building nodes, used when generating the map.))

    private const float mkRequiredTime = 25.0f;
    private TextMeshPro _tmp_text;

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
        Invoke("DisplayBuildingText",mkRequiredTime); 
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
