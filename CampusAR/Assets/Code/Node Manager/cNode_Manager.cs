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
    public static cNode_Manager                     mInstance;                                  // Singleton instance, used to reference this class globally.
    
    /* -------- Prefabs -------- */
    
    [Title("Node Prefabs")]
    [SerializeField] private GameObject             pNode_Building;                             // Prefab for the building nodes, used when generating the map.
    [SerializeField] private GameObject             pNode_Path;                                 // Prefab for the path nodes, used when generating the map.

    /* -------- Variables -------- */
    
    public List<cNode>                             mNodes = new List<cNode>();                 // A list of all the nodes.
    private List<GameObject>                        mWorldNodes = new List<GameObject>();       // A list of all the instantiaed nodes in-world.

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

    /* -------- Private Methods -------- */

    /* -------- Public Methods -------- */

    /// <summary>
    /// Loads all the node json files from the resources folder directly into mNodes list.
    /// </summary>
    public void LoadNodeList()
    {
        // Load all buildings.
        TextAsset[] _buildingNodes = Resources.LoadAll<TextAsset>("Nodes\\Buildings\\");

        for (int i = 0; i < _buildingNodes.Length; i++)
        {
            // Create new node for storing.
            cNode_Building _node = new cNode_Building();

            // Convert file to node.
            JsonUtility.FromJsonOverwrite(_buildingNodes[i].ToString(), _node);

            // Add the node.
            mNodes.Add(_node);
        }

        // Load all buildings.
        TextAsset[] _pathNodes = Resources.LoadAll<TextAsset>("Nodes\\Path\\");

        for (int i = 0; i < _pathNodes.Length; i++)
        {
            // Create new node for storing.
            cNode _node = new cNode();

            // Convert file to node.
            JsonUtility.FromJsonOverwrite(_pathNodes[i].ToString(), _node);

            // Add the node.
            mNodes.Add(_node);
        }
    }

    /// <summary>
    /// Corrects all the nodes that have already been instantiated based on the users current position.
    /// </summary>
    public void CorrectNodes(Vector2 _userPosition, float _userAngle)
    {
        for (int i = 0; i < mNodes.Count; i++)
        {
            mWorldNodes[i].transform.position = Quaternion.Euler(0.0f, _userAngle, 0.0f) * cGPSMaths.GetVector(_userPosition, mNodes[i].GetGPSLocation());
        }
    }

    /// <summary>
    /// Creates all the nodes
    /// </summary>
    public void InstantiateNodes(Vector2 _userPosition, float _userAngle)
    {
        // Instantiate all the nodes.
        for (int i = 0; i < mNodes.Count; i++)
        {
            // Setup node variable.
            GameObject _node = null;

            // Create the correct node.
            switch (mNodes[i].GetNodeType())
            {
                case NodeType.Path:
                {
                    _node = Instantiate(pNode_Path);
                    break;
                }
                case NodeType.Building:
                {
                    _node = Instantiate(pNode_Building);
                    break;
                }
            }

            // Position the node correctly, and align it with the North bearing relative to the user.
            _node.transform.position = Quaternion.Euler(0.0f, _userAngle, 0.0f) * cGPSMaths.GetVector(_userPosition, mNodes[i].GetGPSLocation());

            // Name the in-world object.
            _node.name = "Node - " + mNodes[i].GetBuildingName();

            // Add node to list of spawned nodes.
            mWorldNodes.Add(_node);
        }
    }
}