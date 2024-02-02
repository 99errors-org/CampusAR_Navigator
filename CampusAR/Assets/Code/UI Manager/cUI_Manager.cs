using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class cUI_Manager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI rBuildingNameField;
    [SerializeField] private RectTransform rBuildingListContext;
    [SerializeField] private RectTransform rBuildingDrawer;

    [SerializeField] private GameObject pBuildingListButton;

    private cNode currentBuildingNode; // Reference to the current cNode_Building instance
    private bool mListPopulated = false;    // Whether the building list has been populated.

    private bool mOpenBuildingDrawer = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateBuildingNameField();

        PopulateBuildingList();

        AnimationController();
    }

    /* ---- Private Methods ---- */

    private void UpdateBuildingNameField()
    {
        // Check if current building node is selected.
        rBuildingNameField.gameObject.SetActive(currentBuildingNode != null);

        // Check if the currentBuildingNode is not null
        if (currentBuildingNode != null)
        {
            // Update the text content of the TextMeshPro field with the building name
            rBuildingNameField.text = "Building Name: " + currentBuildingNode.GetBuildingName();
        }
    }

    private void PopulateBuildingList()
    {
        if (cNode_Manager.mInstance != null && !mListPopulated)
        {
            // Set the length of the scrollview content.
            rBuildingListContext.sizeDelta = new Vector2(rBuildingListContext.sizeDelta.x, cNode_Manager.mInstance.mNodes.Count * pBuildingListButton.GetComponent<RectTransform>().sizeDelta.y);
        
            // Create building nodes.
            for (int i = 0; i < cNode_Manager.mInstance.mNodes.Count; i++)
            {
                // Instantiate.
                GameObject _building = Instantiate(pBuildingListButton, rBuildingListContext);

                // Position.
                _building.GetComponent<RectTransform>().localPosition = new Vector2(_building.GetComponent<RectTransform>().sizeDelta.x * 0.5f, -(_building.GetComponent<RectTransform>().sizeDelta.y * 0.5f + _building.GetComponent<RectTransform>().sizeDelta.y * i));

                // Set Values.
                _building.transform.Find("BuildingTag (TMP)").GetComponent<TextMeshProUGUI>().text = "Nuts"; // <--- Change this.
                _building.transform.Find("BuildingName (TMP)").GetComponent<TextMeshProUGUI>().text = cNode_Manager.mInstance.mNodes[i].GetBuildingName();
                _building.transform.Find("Distance (TMP)").GetComponent<TextMeshProUGUI>().text = "69 m"; // <--- Change this.
            }

            mListPopulated = true;
        }
        else if (cNode_Manager.mInstance != null && mListPopulated)
        {
            // Update the building Distance.
        }
    }

    private void AnimationController()
    {
        float _speed = 10.0f;

        if (mOpenBuildingDrawer && rBuildingDrawer.position.y < rBuildingDrawer.sizeDelta.y)
        {
            rBuildingDrawer.position = Vector2.Lerp(rBuildingDrawer.position, new Vector2(rBuildingDrawer.position.x, rBuildingDrawer.sizeDelta.y), _speed * Time.fixedDeltaTime);
        }
        else if (!mOpenBuildingDrawer && rBuildingDrawer.position.y > 0.0f)
        {
            rBuildingDrawer.position = Vector2.Lerp(rBuildingDrawer.position, new Vector2(rBuildingDrawer.position.x, -1.0f), _speed * Time.fixedDeltaTime);
        }
    }

    /* ---- Public Methods ---- */

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

        mOpenBuildingDrawer = !mOpenBuildingDrawer;
    }

    public void HandleSettingsButton()
    {
        Debug.Log("You have clicked the Settings button!");
        SceneManager.LoadScene("SettingsScene");
    }
}