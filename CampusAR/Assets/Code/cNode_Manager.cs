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

    /* -------- Constants -------- */

    private const int                               kEarthRadiusMetres = 6371000;               // The radius of the earth in metres.

    /* -------- Variables -------- */
    
    private List<cNode>                             mNodes = new List<cNode>();              // A list of all the nodes.
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
    }

    /* -------- Private Methods -------- */

    /// <summary>
    /// Get the distance between two GPS positions, this is using the Haversine Formula.
    /// </summary>
    /// <param name="_pos1">First GPS position, (Latitude, Longitude)</param>
    /// <param name="_pos2">Second GPS position, (Latitude, Longitude)</param>
    private float GetDistance(Vector2 _pos1, Vector2 _pos2)
    {
        // Convert the Vectors to radians.
        _pos1 *= Mathf.Deg2Rad;
        _pos2 *= Mathf.Deg2Rad;

        // Get the of B relative to A.
        Vector3 _relativePos = _pos2 - _pos1;

        // Get the distance. Using the Haversine formula.
        float _distance = Mathf.Pow(Mathf.Sin(_relativePos.x / 2), 2) + Mathf.Cos(_pos1.x) * Mathf.Cos(_pos2.x) * Mathf.Pow(Mathf.Sin(_relativePos.y / 2), 2);

        _distance = 2 * Mathf.Asin(Mathf.Sqrt(_distance));

        _distance *= kEarthRadiusMetres;

        return _distance;
    }

    /// <summary>
    /// Gets the angle between two GPS positions.
    /// </summary>
    /// <param name="_pos1">First GPS position, (Latitude, Longitude)</param>
    /// <param name="_pos2">Second GPS position, (Latitude, Longitude)</param>
    /// <returns></returns>
    private float GetAngle(Vector2 _pos1, Vector2 _pos2)
    {
        // Calculate the relative X and Y.
        float _x = Mathf.Cos(_pos2.x) * Mathf.Sin((_pos2.y * -1.0f) - (_pos1.y * -1.0f));
        float _y = Mathf.Cos(_pos1.x) * Mathf.Sin(_pos2.x) - Mathf.Sin(_pos1.x) * Mathf.Cos(_pos2.x) * (Mathf.Cos((_pos2.y * -1.0f) - (_pos1.y * -1.0f)));

        // Get the Angle using the X and Y.
        float _angle = Mathf.Atan2(_y, _x) * Mathf.Rad2Deg;

        // Clamp the angle between 0 and 360.
        _angle %= 360.0f;

        if (_angle < 0)
        {
            _angle += 360.0f;
        }

        return _angle;
    }

    /// <summary>
    /// Calculates the in-unity position of "_pos2" relative to "_pos1", by using their GPS positions in the real world.
    /// </summary>
    /// <param name="_pos1">First GPS position, (Latitude, Longitude)</param>
    /// <param name="_pos2">Second GPS position, (Latitude, Longitude)</param>
    /// <returns></returns>
    private Vector3 GetVector(Vector2 _pos1, Vector2 _pos2)
    {
        // Get the Distance.
        float _distance = GetDistance(_pos1, _pos2);

        // Get angle.
        float _angle = GetAngle(_pos1, _pos2);

        // Get the Vector.
        Vector3 _vector = new Vector3(_distance * Mathf.Cos(_angle * Mathf.Deg2Rad), 0.0f, _distance * Mathf.Sin(_angle * Mathf.Deg2Rad));

        return _vector;
    }

    /* -------- Public Methods -------- */

    /// <summary>
    /// Generates the world, based on the users current position.
    /// </summary>
    public void HandleNodes(Vector2 _userPosition, float _userAngle)
    {
        // Check if buildings have been loaded into memory.
        if (mNodes.Count <= 0)
        {
            // Read all the nodes into memory.
            
        }

        // Check if nodes have been generated.
        if (mWorldNodes.Count > 0)
        {
            // Correct the nodes.
        }
        else // Generate the nodes.
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
                _node.transform.position = Quaternion.Euler(0.0f, _userAngle, 0.0f) * GetVector(_userPosition, mNodes[i].GetGPSLocation());

                // Add node to list of spawned nodes.
                mWorldNodes.Add(_node);
            }
        }
    }
}