using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class cUI_Manager : MonoBehaviour
{

    /* -------- References -------- */

    [SerializeField] private TextMeshProUGUI rBuildingNameField;

    [SerializeField] private RectTransform rBuildingListContent;
    [SerializeField] private RectTransform rBuildingDrawer;
    [SerializeField] private RectTransform rSelectTourDrawer;
    [SerializeField] private RectTransform rCreateTourDrawer;
    [SerializeField] private RectTransform rCreateTourContent;
    [SerializeField] private TextMeshProUGUI rTourQueueContent;

    [SerializeField] private RectTransform rCreateTourButton;
    [SerializeField] private RectTransform rSelectBuildingButton;
    [SerializeField] private RectTransform rSelectTourButton;

    [SerializeField] private TMP_InputField rBuildingSearchInput;

    /* -------- Variables -------- */

    // Bool to control the smaller action buttons
    private bool mActionButtonsVisible = false;

    // Reference to the current cNode_Building instance
    private cNode currentBuildingNode;

    // Whether the building list has been populated.
    private bool mBuildingListPopulated = false;
    private bool mCreateTourListPopulated = false;

    // Separate bools for each drawer component
    private bool isBuildingDrawerOpen = false;
    private bool isSelectTourDrawerOpen = false;
    private bool isCreateTourDrawerOpen = false;


    // Button for each building in instantiated list of drawer
    [SerializeField] private GameObject pBuildingListButton;

    // Dictionary to store each drawerPanel 
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
        rCreateTourButton.transform.localScale = Vector3.zero;
        rSelectBuildingButton.transform.localScale = Vector3.zero;
        rSelectTourButton.transform.localScale = Vector3.zero;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateBuildingNameField();

        PopulateBuildingList();
        PopulateCreateTourList();

        ToggleDrawer(rBuildingDrawer, ref isBuildingDrawerOpen);
        ToggleDrawer(rSelectTourDrawer, ref isSelectTourDrawerOpen);
        ToggleDrawer(rCreateTourDrawer, ref isCreateTourDrawerOpen);
    }

    /* ---- Private Methods ---- */


    /// <summary>
    /// Sets the activity state of action buttons.
    /// </summary>
    /// <param name="active">Boolean indicating the activity state.</param>
    private void SetButtonsActive(bool active)
    {
        rCreateTourButton.GetComponent<Button>().enabled = active;
        rSelectBuildingButton.GetComponent<Button>().enabled = active;
        rSelectTourButton.GetComponent<Button>().enabled = active;
    }

    /// <summary>
    /// Sets the scale of action buttons.
    /// </summary>
    /// <param name="scale">The target scale for the buttons.</param>
    private void SetButtonsScale(Vector3 scale)
    {
        rCreateTourButton.transform.localScale = scale;
        rSelectBuildingButton.transform.localScale = scale;
        rSelectTourButton.transform.localScale = scale;
    }

    /// <summary>
    /// Updates the visibility and content of the building name field based on the selected building node.
    /// </summary>
    private void UpdateBuildingNameField()
    {
        // Check if current building node is selected.
        rBuildingNameField.gameObject.SetActive(currentBuildingNode != null);

        if (rBuildingNameField != null)
        {
            rBuildingNameField.gameObject.SetActive(currentBuildingNode != null);

            // Check if the currentBuildingNode is not null
            if (currentBuildingNode != null)
            {
                // Update the text content of the TextMeshPro field with the building name
                rBuildingNameField.text = currentBuildingNode.GetBuildingName();
            }
        }
    }

    /// <summary>
    /// Populates the building list in the UI based on available building nodes.
    /// </summary>
    private void PopulateBuildingList()
    {
        if (cNode_Manager.mInstance != null && !mBuildingListPopulated)
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

                // Get raw distance between player and target node
                float _distanceFloat = cGPSMaths.GetDistance(cNode_Manager.mInstance.mNodes[i].GetGPSLocation(), cUser_Manager.mInstance.mUserLastLocation);

                // Convert distance to integer
                int _distance = Mathf.FloorToInt(_distanceFloat);

                // Set Values.
                _building.transform.Find("BuildingTag (TMP)").GetComponent<TextMeshProUGUI>().text = cNode_Manager.mInstance.mNodes[i].GetBuildingAbbreviation();
                _building.transform.Find("BuildingName (TMP)").GetComponent<TextMeshProUGUI>().text = cNode_Manager.mInstance.mNodes[i].GetBuildingName();

                string storedPreference = PlayerPrefs.GetString("MetricsPreference", "");
                string distanceUnit = storedPreference == "Kilometres (km)" ? "m" : storedPreference == "Miles (mi)" ? "mi" : "";
                _building.transform.Find("Distance (TMP)").GetComponent<TextMeshProUGUI>().text = $"{_distance} {distanceUnit}";

                // Capture the current cNode for the button click event
                cNode clickedNode = cNode_Manager.mInstance.mNodes[i];

                _building.GetComponent<Button>().onClick.AddListener(() =>
                {
                    if (rBuildingNameField != null)
                    {
                        rBuildingNameField.gameObject.SetActive(true);
                        currentBuildingNode = clickedNode;
                        cUser_Manager.mInstance.SetTargetNode(i);
                    }
                });
            }

            mBuildingListPopulated = true;
        }
        else if (cNode_Manager.mInstance != null && mBuildingListPopulated)
        {
            // Update the building Distance.
        }
    }

    /// <summary>
    /// Populates the create tour list in the UI of all building nodes to add to the queue.
    /// </summary>
    private void PopulateCreateTourList()
    {
        if (cNode_Manager.mInstance != null && !mCreateTourListPopulated)
        {
            // Set the length of the scrollview content.
            rCreateTourContent.sizeDelta = new Vector2(rCreateTourContent.sizeDelta.x, cNode_Manager.mInstance.mNodes.Count * pBuildingListButton.GetComponent<RectTransform>().sizeDelta.y);

            // Create building nodes.
            for (int i = 0; i < cNode_Manager.mInstance.mNodes.Count; i++)
            {
                int currentIndex = i;

                // Instantiate.

                GameObject _building = Instantiate(pBuildingListButton, rCreateTourContent);

                // Position.
                _building.GetComponent<RectTransform>().localPosition = new Vector2(_building.GetComponent<RectTransform>().sizeDelta.x * 0.5f, -(_building.GetComponent<RectTransform>().sizeDelta.y * 0.5f + _building.GetComponent<RectTransform>().sizeDelta.y * i));

                // Get raw distance between player and target node
                float _distanceFloat = cGPSMaths.GetDistance(cNode_Manager.mInstance.mNodes[i].GetGPSLocation(), cUser_Manager.mInstance.mUserLastLocation);

                // Convert distance to integer
                int _distance = Mathf.FloorToInt(_distanceFloat);

                // Set Values.
                _building.transform.Find("BuildingTag (TMP)").GetComponent<TextMeshProUGUI>().text = cNode_Manager.mInstance.mNodes[i].GetBuildingAbbreviation();
                _building.transform.Find("BuildingName (TMP)").GetComponent<TextMeshProUGUI>().text = cNode_Manager.mInstance.mNodes[i].GetBuildingName();

                string storedPreference = PlayerPrefs.GetString("MetricsPreference", "");
                string distanceUnit = storedPreference == "Kilometres (km)" ? "m" : storedPreference == "Miles (mi)" ? "mi" : "";
                _building.transform.Find("Distance (TMP)").GetComponent<TextMeshProUGUI>().text = $"{_distance} {distanceUnit}";

                _building.GetComponent<Button>().onClick.AddListener(() =>
                {
                    // Check if the building is already in the queue content
                    if (rTourQueueContent.text.Contains(cNode_Manager.mInstance.mNodes[currentIndex].GetBuildingAbbreviation()))
                    {
                        // Remove the building node from the queue content
                        rTourQueueContent.text = rTourQueueContent.text.Replace(cNode_Manager.mInstance.mNodes[currentIndex].GetBuildingAbbreviation() + ", ", "");

                        // Change background color back to white
                        Color whiteColor = Color.white;
                        _building.GetComponent<Image>().color = whiteColor;
                    }
                    else
                    {
                        // Add the building to the queue content
                        rTourQueueContent.text += cNode_Manager.mInstance.mNodes[currentIndex].GetBuildingAbbreviation() + ", ";

                        // Change background color to grey
                        Color greyColor = new Color(0.8f, 0.8f, 0.8f); // Adjust the values based on your preference
                        _building.GetComponent<Image>().color = greyColor;

                        if (Application.isEditor)
                        {
                            Debug.Log(cNode_Manager.mInstance.mNodes[currentIndex].GetBuildingName());
                        }
                        cPathfinding.mInstance.AddTourBuilding(currentIndex);
                    }
                });
            }

            mCreateTourListPopulated = true;
        }
        else if (cNode_Manager.mInstance != null && mCreateTourListPopulated)
        {
            // Update the building Distance.
        }
    }

    /// <summary>
    /// Toggles the visibility of a drawer based on its current state.
    /// </summary>
    /// <param name="drawer">The RectTransform of the drawer to toggle.</param>
    /// <param name="isOpen">Reference to the boolean indicating the state of the drawer.</param>
    private void ToggleDrawer(RectTransform drawer, ref bool isOpen)
    {
        if (Application.isEditor)
        {
            Debug.Log($"Toggling drawer {drawer.name}. IsOpen: {isOpen}");
        }
        float targetY = isOpen ? drawer.sizeDelta.y : 0.0f;
        float currentY = drawer.position.y;

        float _speed = 10.0f;
        float newY = Mathf.Lerp(currentY, targetY, _speed * Time.fixedDeltaTime);

        drawer.position = new Vector2(drawer.position.x, newY);
    }

    /// <summary>
    /// Hides a drawer by setting its state to closed and triggering the toggle.
    /// </summary>
    /// <param name="drawer">The RectTransform of the drawer to hide.</param>
    /// <param name="isOpen">Reference to the boolean indicating the state of the drawer.</param>
    private void HideDrawer(RectTransform drawer, ref bool isOpen)
    {
        isOpen = false;
        ToggleDrawer(drawer, ref isOpen);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputText"></param>
    private void UpdateBuildingNodesVisibility(string inputText)
    {
        foreach (Transform child in rBuildingListContent.transform)
        {
            GameObject buildingButton = child.gameObject;
            string buildingName = buildingButton.transform.Find("BuildingName (TMP)").GetComponent<TextMeshProUGUI>().text;


            int levenshteinDistance = CalculateLevenshteinDistance(buildingName, inputText);

            // Check if the search input is a substring of the building name or vice versa
            bool isSubstringMatch = buildingName.ToLower().Contains(inputText.ToLower()) || inputText.ToLower().Contains(buildingName.ToLower());

            // Set your threshold distance as needed
            int levenshteinThreshold = 9;
            int substringThreshold = 0;

            // Enable or disable the button based on combined conditions
            bool isActive = (levenshteinDistance <= levenshteinThreshold) || (isSubstringMatch && levenshteinDistance <= substringThreshold);
            buildingButton.SetActive(isActive);
        }
    }

    /// <summary>
    /// Calculates the Levenshtein Distance between two strings.
    /// </summary>
    /// <param name="s1">The first string.</param>
    /// <param name="s2">The second string.</param>
    /// <returns>
    /// The minimum number of single-character edits required to transform
    /// the first string into the second string (Levenshtein Distance).
    /// </returns>
    private int CalculateLevenshteinDistance(string s1, string s2)
    {
        s1 = s1.ToLower();
        s2 = s2.ToLower();

        int[,] distance = new int[s1.Length + 1, s2.Length + 1];

        for (int i = 0; i <= s1.Length; i++)
        {
            for (int j = 0; j <= s2.Length; j++)
            {
                if (i == 0)
                    distance[i, j] = j;
                else if (j == 0)
                    distance[i, j] = i;
                else
                    distance[i, j] = Mathf.Min(
                        distance[i - 1, j] + 1,
                        distance[i, j - 1] + 1,
                        distance[i - 1, j - 1] + (s1[i - 1] == s2[j - 1] ? 0 : 1)
                    );
            }
        }

        return distance[s1.Length, s2.Length];
    }

    /* ---- Public Methods ---- */

    /// <summary>
    /// Closes all drawers if open.
    /// </summary>
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

    /// <summary>
    /// Hides the building drawer if open.
    /// </summary>
    public void HideBuildingDrawer()
    {
        HideDrawer(rBuildingDrawer, ref isBuildingDrawerOpen);
    }

    /// <summary>
    /// Hides the select tour drawer if open.
    /// </summary>
    public void HideSelectTourDrawer()
    {
        HideDrawer(rSelectTourDrawer, ref isSelectTourDrawerOpen);
    }

    /// <summary>
    /// Hides the create tour drawer if open.
    /// </summary>
    public void HideCreateTourDrawer()
    {
        HideDrawer(rCreateTourDrawer, ref isCreateTourDrawerOpen);
    }

    /// <summary>
    /// Handles the click event of the floating action button, animating buttons.
    /// </summary>
    public void HandleFloatingActionButton()
    {
        Debug.Log("You have clicked the floating button!");
        StartCoroutine(AnimateButtons());
    }

    /// <summary>
    /// Animates the action buttons by scaling them.
    /// </summary>
    /// <returns>An enumerator for coroutine.</returns>
    IEnumerator AnimateButtons()
    {
        float time = 0;
        float duration = 0.25f;
        AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        // Determine the target scale based on visibility, 0 if visible, 2.0f if not visible
        Vector3 targetScale = mActionButtonsVisible ? Vector3.zero : Vector3.one * 2.0f;

        // Grab current scale of one of the buttons (this will be used for all small action buttons)
        Vector3 originalScale = rCreateTourButton.transform.localScale;

        while (time < duration)
        {
            float t = curve.Evaluate(time / duration);
            // Lerp between initial scale that is conditionally 2 or 0 if the buttons are visible
            SetButtonsScale(Vector3.Lerp(originalScale, targetScale, t));

            time += Time.deltaTime;
            yield return null;
        }

        // Toggle visibility and update boolean
        SetButtonsActive(!mActionButtonsVisible);
        mActionButtonsVisible = !mActionButtonsVisible;
    }

    /// <summary>
    /// Handles the click event of the Select Building button, toggling the building drawer.
    /// </summary>
    public void HandleSelectBuildingButton()
    {
        if (Application.isEditor)
        {
            Debug.Log("You have clicked the SelectBuilding button!");
        }
        isBuildingDrawerOpen = !isBuildingDrawerOpen;
        ToggleDrawer(rBuildingDrawer, ref isBuildingDrawerOpen);
    }

    /// <summary>
    /// Handles the click event of the Select Tour button, toggling the select tour drawer.
    /// </summary>
    public void HandleSelectTourButton()
    {
        if (Application.isEditor)
        {
            Debug.Log("You have clicked the SelectTour button!");
        }
        isSelectTourDrawerOpen = !isSelectTourDrawerOpen;
        ToggleDrawer(rSelectTourDrawer, ref isSelectTourDrawerOpen);
    }

    /// <summary>
    /// Handles click event of the Create Tour button, toggling the create tour drawer
    /// </summary>
    public void HandleCreateTourButton()
    {
        if (Application.isEditor)
        {
            Debug.Log("You have clicked the CreateTour button!");
        }
        isCreateTourDrawerOpen = !isCreateTourDrawerOpen;
        ToggleDrawer(rCreateTourDrawer, ref isCreateTourDrawerOpen);
    }

    /// <summary>
    /// Handles the click event of the Settings button, navigates to Settings scene
    /// </summary>
    public void HandleSettingsButton()
    {
        if (Application.isEditor)
        {
            Debug.Log("You have clicked the Settings button!");
        }
        SceneManager.LoadScene("SettingsScene");
    }

    /// <summary>
    /// Building search input value changed.
    /// </summary>
    public void OnSearchInputValueChanged()
    {
        string inputValue = rBuildingSearchInput.text;
        // Call the function to update building nodes based on Levenshtein distance
        UpdateBuildingNodesVisibility(inputValue);
    }
}