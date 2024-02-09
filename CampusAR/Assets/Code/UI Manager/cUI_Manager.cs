using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class cUI_Manager : MonoBehaviour
{

    /* -------- Variables -------- */
    [SerializeField] private TextMeshProUGUI rBuildingNameField;
    [SerializeField] private RectTransform rBuildingListContent;
    [SerializeField] private RectTransform rBuildingDrawer;

    [SerializeField] private GameObject pBuildingListButton;

    [SerializeField] private RectTransform pCreateTourButton;
    [SerializeField] private RectTransform pSelectBuildingButton;
    [SerializeField] private RectTransform pSelectTourButton;

    private cNode currentBuildingNode; // Reference to the current cNode_Building instance
    private bool mListPopulated = false;    // Whether the building list has been populated.

    private bool mOpenBuildingDrawer = false;
    private bool mActionButtonsVisible = false;

    void Start()
    {
        pCreateTourButton.transform.localScale = Vector3.zero;
        pSelectBuildingButton.transform.localScale = Vector3.zero;
        pSelectTourButton.transform.localScale = Vector3.zero;
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateBuildingNameField();

        PopulateBuildingList();

        AnimationController();
    }

    /* ---- Private Methods ---- */

    private void ToggleBuildingDrawer()
    {
        mOpenBuildingDrawer = !mOpenBuildingDrawer;
    }

    private void SetButtonsActive(bool active)
    {
        pCreateTourButton.GetComponent<Button>().enabled = active;
        pSelectBuildingButton.GetComponent<Button>().enabled = active;
        pSelectTourButton.GetComponent<Button>().enabled = active;
    }

    private void SetButtonsScale(Vector3 scale)
    {
        pCreateTourButton.transform.localScale = scale;
        pSelectBuildingButton.transform.localScale = scale;
        pSelectTourButton.transform.localScale = scale;
    }

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
            rBuildingListContent.sizeDelta = new Vector2(rBuildingListContent.sizeDelta.x, cNode_Manager.mInstance.mNodes.Count * pBuildingListButton.GetComponent<RectTransform>().sizeDelta.y);

            // Create building nodes.
            for (int i = 0; i < cNode_Manager.mInstance.mNodes.Count; i++)
            {
                // Instantiate.
                GameObject _building = Instantiate(pBuildingListButton, rBuildingListContent);

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

    public void HideBuildingDrawer()
    {
        mOpenBuildingDrawer = false;
    }

    public void HandleFloatingActionButton()
    {
        Debug.Log("You have clicked the floating button!");
        StartCoroutine(AnimateButtons());
    }

    IEnumerator AnimateButtons()
    {
        float time = 0;
        float duration = 0.25f;
        AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        // Determine the target scale based on visibility, 0 if visible, 2.0f if not visible
        Vector3 targetScale = mActionButtonsVisible ? Vector3.zero : Vector3.one * 2.0f;

        // Grab current scale of button
        Vector3 originalScale = pCreateTourButton.transform.localScale;

        while (time < duration)
        {
            float t = curve.Evaluate(time / duration);

            SetButtonsScale(Vector3.Lerp(originalScale, targetScale, t));

            time += Time.deltaTime;
            yield return null;
        }

        // Toggle visibility and update boolean
        SetButtonsActive(!mActionButtonsVisible);
        mActionButtonsVisible = !mActionButtonsVisible;
    }


    void HandleSelectBuildingButton()
    {
        Debug.Log("You have clicked the SelectBuilding button!");
        ToggleBuildingDrawer();
    }

    void HandleSelectTourButton()
    {
        Debug.Log("You have clicked the SelectTour button!");
        ToggleBuildingDrawer();
    }

    void HandleCreateTourButton()
    {
        Debug.Log("You have clicked the CreateTour button!");
        ToggleBuildingDrawer();
    }

    void HandleSettingsButton()
    {
        Debug.Log("You have clicked the Settings button!");
        SceneManager.LoadScene("SettingsScene");
    }
}