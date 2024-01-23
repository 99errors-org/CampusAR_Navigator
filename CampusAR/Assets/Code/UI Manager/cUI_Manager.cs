using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class cUI_Manager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI rBuildingNameField;
    private cNode currentBuildingNode; // Reference to the current cNode_Building instance


    // Start is called before the first frame update
    void Start()
    {
        rBuildingNameField.text = currentBuildingNode.GetBuildingName();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBuildingNameField();
    }

    void UpdateBuildingNameField()
    {
        // Check if the currentBuildingNode is not null
        if (currentBuildingNode != null)
        {
            // Update the text content of the TextMeshPro field with the building name
            rBuildingNameField.text = "Building Name: " + currentBuildingNode.GetBuildingName();
        }
        else
        {
            // Handle the case when there is no currentBuildingNode selected
            rBuildingNameField.text = "No building selected";
        }
    }
    public void HandleFloatingActionButton()
    {
        Debug.Log("You have clicked the floating button!");
    }
    public void HandleSelectBuildingButton()
    {
        Debug.Log("You have clicked the SelectBuilding button!");
    }
    public void HandleSelectTourButton()
    {
        Debug.Log("You have clicked the SelectTour button!");
    }
    public void HandleCreateTourButton()
    {
        Debug.Log("You have clicked the CreateTour button!");
    }

    public void HandleSettingsButton()
    {
        Debug.Log("You have clicked the Settings button!");
    }
}