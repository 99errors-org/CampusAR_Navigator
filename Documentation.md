# Campus AR Navigator Documentation
Class, function and variable documentation

- cNode
- cNode_Manager
- cPathfinding
- cGPSMaths
- cArrowManager
- cUser_Manager
- cDisplayBuildingInfo

## cNode.cs
*Nodes are individual points on the map that users can navigate to.*

-- Class Variables --
```C#
protected string       mNodeName;                              // The name of this node.
protected Vector2      mGeoPosition;                           // The GPS location of the Node, X = Latitude, Y = Longitude.
protected List<cNode>  mConnectedNodes = new List<cNode>();    // A list of all the connected nodes that the user can get to from this node.
```

-- Class Constructor --
```C#
public cNode() {  }

public cNode(string nodeName, Vector2 GPSPosition, List<cNode> connectedNodes)
{
    mNodeName       =  nodeName;
    mGeoPosition    =  GPSPosition;
    mConnectedNodes =  connectedNodes;
}
```

-- Public Functions --
```C#
public string          GetNodeName()
public Vector2         GetGeoPosition()
public List<cNode>     GetConnectedNodes()
```
### cNode_Building : cNode
*A child class of cNode. Can be found within the cNode.cs file*

-- Class Variables --
```C#
private string         mBuildingName;          // The name of the building at this specific node.
private string         mBuildingDescription;   // The description of this nodes building.	
private string	       mBuildingAbbreviation;  // The abbraviation of this building, for example HB for Harris Building.
```

-- Class Constructor --
```C#
public cNode_Building() {  }

public cNode_Building(string nodeName, Vector2 GPSPosition, List<cNode> connectedNodes, string buildingName, string buildingDescription, string buildingAbbreviation)
{
    mNodeName        = nodeName;
    mGeoPosition     = GPSPosition;
    mConnectedNodes  = connectedNodes;

    mBuildingName    = buildingName;
    mBuildingDescription = buildingDescription;
    mBuildingAbbreviation = buildingAbbreviation;
}
```

-- Public Functions --
```C#
public string          GetBuildingName()
public string          GetBuildingAbbreviation()
public string          GetBuildingDescription()
```

## cNode_Manager.cs
*Singleton class to load/manage/store all the nodes in the unity world.*

-- Singleton --
```C#
public static cNode_Manager  mInstance;
```

-- Prefabs --
```C#
private GameObject           pNode_Building;  // Prefab for the building nodes, used when generating the map
private GameObject           pNode_Path;      // Prefab for the path nodes, used when generating the map.
```

-- Class Variables --
```C#
public List<cNode>           mNodes { get; private set; } = new List<cNode>();                   // A list of all the nodes.
public List<cNode_Building>  mBuildingNodes { get; private set; } = new List<cNode_Building>();  // A list of all the building nodes
public List<cNode>           mPathNodes { get; private set; } = new List<cNode>();               // A list of all the path nodes 
private List<GameObject>     mWorldNodes = new List<GameObject>();                               // A list of all the instantiaed nodes in-world.
```

--- Unity Functions --
```C#
private void Awake()         // Creates a singleton instance if one is not already created
```

-- Public Functions --
```C#
public void LoadNodeList()                                              // Loads all the node json files from the resources folder directly into mNodes list.
public void CorrectNodes(Vector2 _userPosition, float _userAngle)       // Corrects all the nodes that have already been instantiated based on the users current position.
public void InstantiateNodes(Vector2 _userPosition, float _userAngle)   // Creates all the nodes
```

## cPathfinding.cs
*Singleton class to run the logic/algorithms for pathfinding between the users current node and the target node*

-- Singleton --
```C#
public static cPathfinding mInstance;
```

-- Class Variables --
```C#
private List<cNode> mPathingQueue = new List<cNode>();          // Queue of target nodes the user wishes to be navigated to
private List<cNode> mCurrentPath = new List<cNode>();           // Chain of nodes working from the users start position to the final target destination
private int mCurrentPathPosition = 0;                           // Index of how far into the list Path the user has traversed
private int mNodeReachThreshold = 10;                           // How close the user must be to the node before the node is considered to be reached
public float DistanceToNextNode { get;}                         // Returns the distance from the user to the next node in thier path
```

-- Unity Functions --
```C#
void Update()    // Calls the pathfinding functions every frame of the game running
```

-- Private Functions --
```C#
private static cNode FindNextNode(cNode currentNode, cNode targetNode)      // Finds the next node to travel to from the current node to reach the target node
private static List<cNode> FindNodePath(cNode startNode, cNode targetNode)  // Finds a path of nodes from the start position to the end postion. Returns a list of nodes
```

-- Public Functions --
```C#
public bool PathfindingV2(cNode startNode, cNode targetNode)                // Runs version 2 of the pathfinding algorithm. Uses path nodes to create complex paths to the target
```

## cGPSMaths.cs
*Static class to run GPS maths functions*

-- Class Variables --
```C#
public const int kEarthRadiusMetres;
```

-- Public Functions --
``` C#
public static float GetDistance(Vector2 _pos1, Vector2 _pos2)  // Get the distance between two GPS positions, this is using the Haversine Formula
public static float GetAngle(Vector2 _pos1, Vector2 _pos2)     // Gets the angle between two GPS positions
public static Vector3 GetVector(Vector2 _pos1, Vector2 _pos2)  // Calculates the in-unity position of "_pos2" relative to "_pos1", by using their GPS positions in the real world
```

## cArrowManager.cs
*Singleton class to manager the arrow used to direct the user to a node*

-- Singleton --
```C#
public static cArrowManager mInstance;
```

-- Prefabs --
```C#
private GameObject mArrowPrefab;
```

-- Class Variables --
```C#
private GameObject mArrow;
private GameObject mCam;
private const float kDistanceInforntOfUser =  1.0f;
private const float kArrowYPosition        = -0.4f;
```

-- Unity Functions --
```C#
private void Awake()
private void Start()
void   FixedUpdate()
```

-- Public Functions --
```C#
public void DirectArrow(cNode targetNode)    // Rotates the arrow to point at the selected node
```

## cUser_Manager.cs
*Singleton class to handle/store all user information*

-- Singleton --
```C#
public static cUser_Manager mInstance;
```

-- Class Variables --
```C#
private GameObject             rInitPanel;                                             // The initialisation panel.
private TextMeshProUGUI        rDebug_CurrentNodeText;                                 // Text used for debugging the initialisation screen.
private Transform              mCamera;                                                // Reference to the camera.

private const float            kLocationTimeout = 20.0f;                               // The amount of seconds before the location services times out and starts again.
public const int               kNullTargetNodeIndex = -1;                              // The index when there is no target node selected

private bool                   mCoroutine = false;
public Vector2                 mUserLastLocation { get; private set; }                 // The users last GPS location, used for maintaining accuracy.
public float                   mUserLastCompassRotation { get; private set; } = 0.0f;  // The users last compass bearing, this is stored to not overwhelm the phone.
private int                    mTargetNodeIndex = kNullTargetNodeIndex;                // The index of the target building/node, if -1 no node is selected.
```

-- Unity Functions --
```C#
private void Awake()
private void Start()
private void FixedUpdate()
```

-- Coroutines --
```C#
IEnumerator LocationCompassSetup()    // Initialises the location services and compass
```

-- Private Functions --
```C#
private void SetUserData()            // Makes a call to the Phones GPS functionality and gets the users current position
private void Debug_CurrentNode()
```

-- Public Functions --
```C#
public void SetTargetNode(int _index)            // Sets the target node that the user will be guided to. Set to -1 to disable guiding
public int GetTargetNodeIndex()                  // Returns the target nodes index if it is there or returns -1 if there is no target node
public cNode GetTargetNode()                     // Returns the target node, as a cNode object
public void ChangeCurrentNode(bool _increment)   // Changes the currently selected node, increments down the list
```

## cDisplayBuildingInfo.cs

-- Class Variables --
```C#
private TextMeshPro _tmp_text;                              // Stores the text object of the current building in loop
private float mTimer;                                       // Stores the time from to see if certain time has passed
private float mDelayTime = 5.0f;                            // Time to pause between calling the building function
```

-- Unity Functions --
```C#
private void FixedUpdate();
```

-- Private Functions --
```C#
private void DisplayBuildingText()                          // Checks if the building is near then sets building text to active
```

## cUI_Manager.cs

-- References --
```C#
[SerializeField] private TextMeshProUGUI rBuildingNameField; // Reference to the TextMeshProUGUI for displaying building names.

[SerializeField] private RectTransform rBuildingListContent; // Reference to the RectTransform for building list content.
[SerializeField] private RectTransform rBuildingDrawer; // Reference to the RectTransform for the building drawer.
[SerializeField] private RectTransform rSelectTourDrawer; // Reference to the RectTransform for the select tour drawer.
[SerializeField] private RectTransform rCreateTourDrawer; // Reference to the RectTransform for the create tour drawer.
[SerializeField] private RectTransform rCreateTourContent; // Reference to the RectTransform for create tour content.
[SerializeField] private TextMeshProUGUI rTourQueueContent; // Reference to the TextMeshProUGUI for tour queue content.

[SerializeField] private RectTransform rCreateTourButton; // Reference to the RectTransform for the create tour button.
[SerializeField] private RectTransform rSelectBuildingButton; // Reference to the RectTransform for the select building button.
[SerializeField] private RectTransform rSelectTourButton; // Reference to the RectTransform for the select tour button.

[SerializeField] private TMP_InputField rBuildingSearchInput; // Reference to the TMP_InputField for building search input.

```
-- Class Variables --

```C#
private bool mActionButtonsVisible = false; // Controls the visibility of smaller action buttons.
private cNode currentBuildingNode; // Reference to the current cNode_Building instance.
private bool mBuildingListPopulated = false; // Indicates whether the building list has been populated.
private bool mCreateTourListPopulated = false; // Indicates whether the create tour list has been populated.

// Separate bools for each drawer component
private bool isBuildingDrawerOpen = false; // Indicates whether the building drawer is open.
private bool isSelectTourDrawerOpen = false; // Indicates whether the select tour drawer is open.
private bool isCreateTourDrawerOpen = false; // Indicates whether the create tour drawer is open.

```

-- Prefabs --
```C#
[SerializeField] private GameObject pBuildingListButton; // Prefab for building list buttons.
```

-- Structures --
```C#
Dictionary<string, RectTransform> drawerPanels = new Dictionary<string, RectTransform>(); // Dictionary to store each drawerPanel
```

-- Unity Functions --

```C#
private void Awake(); // Called when the script instance is being loaded.
private void Start(); // Called before the first frame update.
private void FixedUpdate(); // Called every fixed frame-rate frame.
```

-- Coroutines --

```C#
IEnumerator AnimateButtons(); // Coroutine for animating buttons.
IEnumerator UpdateNodesWithDelay(string inputText) // Coroutine for reinstantiating nodes matching with input.
```

-- Private Functions --

```C#
private void SetButtonsActive(bool active); // Sets the activity of smaller action buttons.
private void SetButtonsScale(Vector3 scale); // Sets the scale of smaller action buttons.
private void UpdateBuildingNameField(); // Updates the building name field based on the current building node.
private void PopulateBuildingList(); // Populates the building list in the drawer.
private void PopulateCreateTourList(); // Populates the create tour list in the drawer.
private void CreateBuildingNode(int index); // Create node for Select Building panel
private void CreateTourBuildingNode(int index); // Create node for Create Tour drawer panel
private void SetBuildingNodeValues(GameObject building, int index); // Sets the value of each node with the abbreviation, name and distance from last known GPS location.
private void UpdateBuildingDistances(RectTransform content); // Update the distances in the drawer once list is populated.
private void ToggleDrawer(RectTransform drawer, ref bool isOpen); // Toggles the visibility of a drawer.
private void HideDrawer(RectTransform drawer, ref bool isOpen); // Hides a specific drawer.
private void UpdateBuildingNodesVisibility(string inputText); // Updates the visibility of building nodes based on search input.
private void InstantiateMatchingBuildingNodes(string inputText); // Instantiate nodes that its building name match the input value
private void DestroyBuildingNodes(); // Destroy all existing building nodes
private void ShowAllBuildingNodes(); // Destroy existing building nodes and instantiate all nodes.
private int CalculateLevenshteinDistance(string s1, string s2); // Calculates the Levenshtein distance between two strings.
```

-- Public Functions --

```C#
public void CloseAllDrawers();            // Closes all drawers if open.
public void HideBuildingDrawer();            // Hides the building drawer if open.
public void HideSelectTourDrawer();            // Hides the select tour drawer if open.
public void HideCreateTourDrawer();            // Hides the create tour drawer if open.
public void HandleFloatingActionButton();            // Handles the click event of the floating action button, animating buttons.
public void HandleSelectBuildingButton();            // Handles the click event of the Select Building button, toggling the building drawer.
public void HandleSelectTourButton();            // Handles click event of the Select Tour button, toggling the select tour drawer.
public void HandleCreateTourButton();            // Handles click event of the Create Tour button, toggling the create tour drawer
public void HandleSettingsButton();            // Handles the click event of the Settings button, navigates to Settings scene
public void OnSearchInputValueChanged();            // Building search input value changed.
```

## cDistanceUnitUtility.cs

-- Public Functions --
```C#
public static string GetDistanceUnit()            // Gets the preferred distance unit based on user settings.
```

**Returns:**
- `string`: The preferred distance unit ("m" for kilometers or "mi" for miles).

**Example:**
```csharp
string distanceUnit = cDistanceUnitUtility.GetDistanceUnit();
// Example usage: If distanceUnit is "mi", the preferred unit is miles.