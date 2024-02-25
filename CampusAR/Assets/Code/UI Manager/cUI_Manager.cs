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

    // Speed for animating in any bottom drawer component
    [SerializeField] private float mDrawerSpeed = 10.0f;

    // distance between search input and building names (lowercase comparison)
    [SerializeField] private int mLevenshteinDistance = 5;

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

            for (int i = 0; i < cNode_Manager.mInstance.mNodes.Count; i++)
            {
                CreateBuildingNode(i);
            }

            mBuildingListPopulated = true;
        }
        else if (cNode_Manager.mInstance != null && mBuildingListPopulated)
        {
            UpdateBuildingDistances(rBuildingListContent);
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
                CreateTourBuildingNode(i);
            }

            mCreateTourListPopulated = true;
        }
        else if (cNode_Manager.mInstance != null && mCreateTourListPopulated)
        {
            UpdateBuildingDistances(rCreateTourContent);
        }
    }

    /// <summary>
    /// Instantiates a building node in the UI.
    /// Positions the node in the building list content.
    /// Sets values for the building node based on the given index.
    /// Captures the current cNode for the button click event, enabling further actions.
    /// </summary>
    /// <param name="index"></param>
    private void CreateBuildingNode(int index)
    {
        // Instantiate.
        GameObject _building = Instantiate(pBuildingListButton, rBuildingListContent);

        // Set Values.
        SetBuildingNodeValues(_building, index);

        // Capture the current cNode for the button click event
        cNode clickedNode = cNode_Manager.mInstance.mNodes[index];

        _building.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (rBuildingNameField != null)
            {
                rBuildingNameField.gameObject.SetActive(true);
                currentBuildingNode = clickedNode;
                cUser_Manager.mInstance.SetTargetNode(index);
            }
        });
    }

    /// <summary>
    /// Instantiates a building node in the UI for the create tour list.
    /// Positions the node in the create tour content.
    /// Sets values for the building node based on the given index.
    /// Adds an onClick listener to the button, allowing dynamic updates to the tour queue content and button background color.
    /// </summary>
    /// <param name="index"></param>
    private void CreateTourBuildingNode(int index)
    {
        // Instantiate.
        GameObject _building = Instantiate(pBuildingListButton, rCreateTourContent);

        // Set Values.
        SetBuildingNodeValues(_building, index);


        _building.GetComponent<Button>().onClick.AddListener(() =>
        {
            // Check if the building is already in the queue content
            if (rTourQueueContent.text.Contains(cNode_Manager.mInstance.mNodes[index].GetBuildingAbbreviation()))
            {
                // Remove the building node from the queue content
                rTourQueueContent.text = rTourQueueContent.text.Replace(cNode_Manager.mInstance.mNodes[index].GetBuildingAbbreviation() + ", ", "");

                // Change background color back to white
                Color whiteColor = Color.white;
                _building.GetComponent<Image>().color = whiteColor;
            }
            else
            {
                // Add the building to the queue content
                rTourQueueContent.text += cNode_Manager.mInstance.mNodes[index].GetBuildingAbbreviation() + ", ";

                // Change background color to grey
                Color greyColor = new Color(0.8f, 0.8f, 0.8f); // Adjust the values based on your preference
                _building.GetComponent<Image>().color = greyColor;

                if (Application.isEditor)
                {
                    Debug.Log(cNode_Manager.mInstance.mNodes[index].GetBuildingName());
                }
                cPathfinding.mInstance.AddTourBuilding(index);
            }
        });
    }

    /// <summary>
    /// Calculates the distance between the building node and the user's last location using GPS.
    /// Sets the values of the building node, including abbreviation, name, and distance, based on the given index.
    /// </summary>
    /// <param name="building">The GameObject representing the building node.</param>
    /// <param name="index">The index of the building node.</param>
    private void SetBuildingNodeValues(GameObject building, int index)
    {
        float distanceFloat = cGPSMaths.GetDistance(cNode_Manager.mInstance.mNodes[index].GetGPSLocation(), cUser_Manager.mInstance.mUserLastLocation);

        int distance = Mathf.FloorToInt(distanceFloat);

        // Set Values.
        building.transform.Find("BuildingTag (TMP)").GetComponent<TextMeshProUGUI>().text = cNode_Manager.mInstance.mNodes[index].GetBuildingAbbreviation();
        building.transform.Find("BuildingName (TMP)").GetComponent<TextMeshProUGUI>().text = cNode_Manager.mInstance.mNodes[index].GetBuildingName();

        string distanceUnit = cDistanceUnitUtility.GetDistanceUnit();
        building.transform.Find("Distance (TMP)").GetComponent<TextMeshProUGUI>().text = $"{distance} {distanceUnit}";
    }

    /// <summary>
    /// Updates the distances for each building node within the specified content area.
    /// Utilizes the SetBuildingNodeValues function to refresh the distance information in the UI.
    /// </summary>
    /// <param name="content">The RectTransform representing the content area.</param>
    private void UpdateBuildingDistances(RectTransform content)
    {
        for (int i = 0; i < content.childCount; i++)
        {
            GameObject building = content.GetChild(i).gameObject;
            SetBuildingNodeValues(building, i);
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

        float _speed = mDrawerSpeed;
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

    private IEnumerator UpdateNodesWithDelay(string inputText)
    {
        // Wait for a short delay before destroying existing building nodes
        yield return new WaitForSeconds(0.5f);

        // Destroy existing building nodes
        DestroyBuildingNodes();

        // Instantiate matching building nodes
        InstantiateMatchingBuildingNodes(inputText);
    }


    private void InstantiateMatchingBuildingNodes(string inputText)
    {
        int visibleNodeCount = 0;

        for (int i = 0; i < cNode_Manager.mInstance.mNodes.Count; i++)
        {
            string buildingName = cNode_Manager.mInstance.mNodes[i].GetBuildingName();

            int levenshteinDistance = CalculateLevenshteinDistance(buildingName, inputText);
            bool isSubstringMatch = buildingName.ToLower().Contains(inputText.ToLower()) || inputText.ToLower().Contains(buildingName.ToLower());

            int combinedScore = Mathf.Min(levenshteinDistance, Mathf.Abs(buildingName.Length - inputText.Length));

            if (Application.isEditor)
            {
                Debug.Log($"Building: {buildingName}, Levenshtein Distance: {levenshteinDistance}, Combined Score: {combinedScore}");
            }

            // Adjust the threshold based on your observation
            if (combinedScore <= mLevenshteinDistance)
            {
                CreateBuildingNode(i);
                visibleNodeCount++;
            }
        }
    }

    private void DestroyBuildingNodes()
    {
        foreach (Transform child in rBuildingListContent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void ShowAllBuildingNodes()
    {
        // Destroy existing building nodes
        DestroyBuildingNodes();

        // Instantiate all building nodes
        for (int i = 0; i < cNode_Manager.mInstance.mNodes.Count; i++)
        {
            CreateBuildingNode(i);
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

        if (string.IsNullOrEmpty(inputValue))
        {
            // Input is empty, show all buildings
            ShowAllBuildingNodes();
        }
        else
        {
            // Input is not empty, filter and display matching buildings
            StartCoroutine(UpdateNodesWithDelay(inputValue));
        }
    }
}