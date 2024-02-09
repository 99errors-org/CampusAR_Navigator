using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.Properties;

public class cUI_Manager : MonoBehaviour
{

    /* -------- Variables -------- */
    [SerializeField] private TextMeshProUGUI rBuildingNameField;
    [SerializeField] private RectTransform rBuildingListContent;
    [SerializeField] private RectTransform rBuildingDrawer;
    [SerializeField] private RectTransform rSelectTourDrawer;
    [SerializeField] private RectTransform rCreateTourDrawer;

    // Button for each building in instantiated list of drawer
    [SerializeField] private GameObject pBuildingListButton;


    // References for each small action button
    [SerializeField] private RectTransform pCreateTourButton;
    [SerializeField] private RectTransform pSelectBuildingButton;
    [SerializeField] private RectTransform pSelectTourButton;

    private cNode currentBuildingNode; // Reference to the current cNode_Building instance
    private bool mListPopulated = false;    // Whether the building list has been populated.

    // Separate bools for each drawer component
    private bool isBuildingDrawerOpen = false;
    private bool isSelectTourDrawerOpen = false;
    private bool isCreateTourDrawerOpen = false;

    // Bool to control the smaller action buttons
    private bool mActionButtonsVisible = false;

    Dictionary<string, RectTransform> drawerPanels = new Dictionary<string, RectTransform>();

    void Awake()
    {
        // Initialise the dictionary with the serialized RectTransforms
        drawerPanels.Add("BuildingDrawer", rBuildingDrawer);
        drawerPanels.Add("SelectTourDrawer", rSelectTourDrawer);
        drawerPanels.Add("CreateTourDrawer", rCreateTourDrawer);
    }

    void Start()
    {
        // Editor values are set at 2f, this ensures the scale is set to 0f at runtime
        pCreateTourButton.transform.localScale = Vector3.zero;
        pSelectBuildingButton.transform.localScale = Vector3.zero;
        pSelectTourButton.transform.localScale = Vector3.zero;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateBuildingNameField();

        PopulateBuildingList();

        if(Application.isEditor)
        {
            ToggleDrawer(rBuildingDrawer, ref isBuildingDrawerOpen);
            ToggleDrawer(rSelectTourDrawer, ref isSelectTourDrawerOpen);
            ToggleDrawer(rCreateTourDrawer, ref isCreateTourDrawerOpen);
        }
    }

    /* ---- Private Methods ---- */

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

    private void ToggleDrawer(RectTransform drawer, ref bool isOpen)
    {
        Debug.Log($"Toggling drawer {drawer.name}. IsOpen: {isOpen}");
        float targetY = isOpen ? drawer.sizeDelta.y : 0.0f;
        float currentY = drawer.position.y;

        float _speed = 10.0f;
        float newY = Mathf.Lerp(currentY, targetY, _speed * Time.fixedDeltaTime);

        drawer.position = new Vector2(drawer.position.x, newY);
    }

    private void HideDrawer(RectTransform drawer, ref bool isOpen)
    {
        isOpen = false;
        ToggleDrawer(drawer, ref isOpen);
    }

    /* ---- Public Methods ---- */

    public void CloseAllDrawers()
    {
        if (isBuildingDrawerOpen)
        {
            isBuildingDrawerOpen = false;
            ToggleDrawer(rBuildingDrawer, ref isBuildingDrawerOpen);
        }

        if (isSelectTourDrawerOpen)
        {
            isSelectTourDrawerOpen = false;
            ToggleDrawer(rSelectTourDrawer, ref isSelectTourDrawerOpen);
        }

        if (isCreateTourDrawerOpen)
        {
            isCreateTourDrawerOpen = false;
            ToggleDrawer(rCreateTourDrawer, ref isCreateTourDrawerOpen);
        }
    }


    public void HideBuildingDrawer()
    {
        HideDrawer(rBuildingDrawer, ref isBuildingDrawerOpen);
    }

    public void HideSelectTourDrawer()
    {
        HideDrawer(rSelectTourDrawer, ref isSelectTourDrawerOpen);
    }

    public void HideCreateTourDrawer()
    {
        HideDrawer(rCreateTourDrawer, ref isCreateTourDrawerOpen);
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


    public void HandleSelectBuildingButton()
    {
        Debug.Log("You have clicked the SelectBuilding button!");
        isBuildingDrawerOpen = !isBuildingDrawerOpen;
        ToggleDrawer(rBuildingDrawer, ref isBuildingDrawerOpen);
    }

    public void HandleSelectTourButton()
    {
        Debug.Log("You have clicked the SelectTour button!");
        isSelectTourDrawerOpen = !isSelectTourDrawerOpen;
        ToggleDrawer(rSelectTourDrawer, ref isSelectTourDrawerOpen);
    }

    public void HandleCreateTourButton()
    {
        Debug.Log("You have clicked the CreateTour button!");
        isCreateTourDrawerOpen = !isCreateTourDrawerOpen;
        ToggleDrawer(rCreateTourDrawer, ref isCreateTourDrawerOpen);
    }

    public void HandleSettingsButton()
    {
        Debug.Log("You have clicked the Settings button!");
        SceneManager.LoadScene("SettingsScene");
    }
}