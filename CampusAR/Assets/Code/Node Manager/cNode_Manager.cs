using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TetraCreations.Attributes;
using Unity.VisualScripting;
using TMPro;

public class cNode_Manager : MonoBehaviour
{
    /* -------- References -------- */

    /* Singleton */
    public static cNode_Manager mInstance;                                  // Singleton instance, used to reference this class globally.

    /* -------- Prefabs -------- */

    [Title("Node Prefabs")]
    [SerializeField] private GameObject pNode_Building;                             // Prefab for the building nodes, used when generating the map.
    [SerializeField] private GameObject pNode_Path;                                 // Prefab for the path nodes, used when generating the map.

    /* -------- Variables -------- */

    public List<cNode>                              mNodes { get; private set; } = new List<cNode>();                     // A list of all the nodes.
    public List<cNode_Building>                     mBuildingNodes { get; private set; } = new List<cNode_Building>();    // A list of all the building nodes. mBuildingNodes[0] is a dummy node. Nodes are loading in index order so mBuildingNodes[5] = node with index 5
    public List<cNode>                              mPathNodes { get; private set; } = new List<cNode>();                 // A list of all the path nodes. mPathNodes[0] is a dummy node. Nodes are loading in index order so mPathNodes[5] = node with index 5
    public List<GameObject>                        mWorldNodes { get; private set; } = new List<GameObject>();            // A list of all the instantiaed nodes in-world.


    /* -------- Unity Methods -------- */

    private void Awake()
    {
        // Check if singleton instance has been assigned.
        if (mInstance == null)
        {
            // Assign the singleton instance.
            mInstance = this;
        }
        else
        {
            // Destroy this object.
            Destroy(this);
        }

        // Load nodes.
        LoadNodeList();
    }

    /* -------- Public Methods -------- */

    /// <summary>
    /// Loads all the node json files from the resources folder directly into mNodes list.
    /// </summary>
    public void LoadNodeList()
    {
        mNodes.Clear();
        mBuildingNodes.Clear();
        mPathNodes.Clear();

        //mBuildingNodes.Add(new cNode_Building());
        //mPathNodes.Add(new cNode());

        // Load all buildings.
        TextAsset[] _buildingNodes = Resources.LoadAll<TextAsset>("Nodes\\Buildings\\");

        for (int i = 0; i < _buildingNodes.Length; i++)
        {
            // Create new node for storing.
            cNode_Building _node = new cNode_Building();

            // Convert file to node.
            JsonUtility.FromJsonOverwrite(_buildingNodes[i].ToString(), _node);

            // ----------

            // ----------

            // Add the node.
            mNodes.Add(_node);
            mBuildingNodes.Add(_node);
        }

        // Load all path nodes.
        TextAsset[] _pathNodes = Resources.LoadAll<TextAsset>("Nodes\\Path\\");

        for (int i = 0; i < _pathNodes.Length; i++)
        {
            //Debug.Log(_pathNodes[i].ToString());

            // Create new node for storing.
            cNode _node = new cNode();

            // Convert file to node.
            JsonUtility.FromJsonOverwrite(_pathNodes[i].ToString(), _node);

            // Add the node.
            mNodes.Add(_node);
            mPathNodes.Add(_node);
        }

        Debug.Log("---- Building Nodes ----");
        foreach (cNode_Building node in cNode_Manager.mInstance.mBuildingNodes)
        {
            Debug.Log("Node Index: " + node.GetNodeIndex() + "; Node ID: " + node.GetNodeID() + "; Position in List: " + cNode_Manager.mInstance.mBuildingNodes.IndexOf(node) + "; Node Name: " + node.GetNodeName());
        }

        Debug.Log("\n\n---- Path Nodes ----");

        foreach (cNode node in cNode_Manager.mInstance.mPathNodes)
        {
            Debug.Log("Node Index: " + node.GetNodeIndex() + "; Node ID: " + node.GetNodeID() + "; Position in List: " + cNode_Manager.mInstance.mPathNodes.IndexOf(node) + "; Node Name: " + node.GetNodeName());
        }
    }

    /// <summary>
    /// Corrects all the nodes that have already been instantiated based on the users current position.
    /// </summary>
    public void CorrectNodes(Vector2 _userPosition)
    {
        // Move all the nodes to the correct positions.
        for (int i = 0; i < mWorldNodes.Count; i++)
        {
            mWorldNodes[i].transform.localPosition = cGPSMaths.GetVector(_userPosition, mNodes[i].GetGPSLocation());
        }

    }

    /// <summary>
    /// Creates all the nodes
    /// </summary>
    public void InstantiateNodes(Vector2 _userPosition)
    {
        // Setup node variable.
        GameObject _node = null;

        // Instantiate all the building nodes.
        foreach (cNode buildingNode in mBuildingNodes)
        {
            // Create the node
            _node = Instantiate(pNode_Building);

            // Position the node correctly, and align it with the North bearing relative to the user.
            _node.transform.position = cGPSMaths.GetVector(_userPosition, buildingNode.GetGPSLocation());

            // Name the in-world object.
            _node.name = "Node - " + buildingNode.GetBuildingName();

            // Find the TextMeshPro component and update the text.
            TextMeshPro tmp = _node.GetComponentInChildren<TextMeshPro>();
            if (tmp != null)
            {
                // Setting the text component of node to building name and description.
                tmp.text = (buildingNode.GetBuildingName()+"\n"+buildingNode.GetBuildingDescription());
            }
            else
            {
                Debug.LogError("TextMeshPro component not found on the instantiated node.");
            }
            // Setting the text to inactive
            tmp.gameObject.SetActive(false);

            // Add node to list of spawned nodes.
            mWorldNodes.Add(_node);
        }

        //Instantiate all the path nodes
        foreach (cNode pathNode in mPathNodes)
        {
            // Create the node
            _node = Instantiate(pNode_Path);

            // Position the node correctly, and align it with the North bearing relative to the user.
            _node.transform.position = cGPSMaths.GetVector(_userPosition, pathNode.GetGPSLocation());

            // Name the in-world object.
            _node.name = "Node - " + pathNode.GetNodeName();

            // Add node to list of spawned nodes.
            mWorldNodes.Add(_node);
        }
    }

}