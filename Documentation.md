# Campus AR Navigator Documentation
Class, function and variable documentation

- cNode
- cNode_Manager
- cPathfinding

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
public bool PathfindingV2(cNode startNode, cNode targetNode
```

