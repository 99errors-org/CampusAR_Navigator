using System.Collections;
using System.Collections.Generic;
using TetraCreations.Attributes;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class cNode
{
    public static cNode nullNode;

	/* -------- Variables -------- */
	[Title("Node Information")]
	[SerializeField] protected string		    mNodeName;                              // The name of this node.
	[SerializeField] protected Vector2          mGeoPosition;                           // The GPS location of the Node, X = Latitude, Y = Longitude.
	[SerializeField] protected List<cNode>      mConnectedNodes = new List<cNode>();    // A list of all the connected nodes that the user can get to from this node.

    /* -------- Constructors -------- */

    public cNode()
    {

    }

    public cNode(string nodeName, Vector2 GPSPosition, List<cNode> connectedNodes)
    {
        mNodeName = nodeName;
        mGeoPosition = GPSPosition;
        mConnectedNodes = connectedNodes;
    }

    /* -------- Public Methods -------- */

    /// <summary>
    /// Returns the name of the node.
    /// </summary>
    /// <returns></returns>
    public string GetNodeName()
    {
        return mNodeName;
    }

	/// <summary>
	/// Get the position of this node in Latitude and Longitude.
	/// </summary>
	/// <returns>Vector2(Lat, Long) </returns>
	public Vector2 GetGPSLocation()
	{
		return mGeoPosition;
	}

    public List<cNode> GetConnectedNodes()
    {
        return mConnectedNodes;
    }

	/// <summary>
	/// Get the name of this building.
	/// </summary>
	public virtual string GetBuildingName()
	{
		// Do nothing;
		return "";
	}

    /// <summary>
    /// Get the description of this building.
    /// </summary>
    public virtual string GetBuildingDescription()
    {
        // Do nothing;
        return "";
    }

    /// <summary>
    /// Get the abbreviation of this building.
    /// </summary>
    public virtual string GetBuildingAbbreviation()
    {
        // Do nothing;
        return "";
    }
}

public class cNode_Building : cNode
{
	/* -------- Variables -------- */
	[Title("Building Specific")]
	[SerializeField] private string			mBuildingName;					// The name of the building at this specific node.
	[SerializeField] private string			mBuildingDescription;           // The description of this nodes building.	
    [SerializeField] private string			mBuildingAbbreviation;          // The abbraviation of this building, for example HB for Harris Building.

    /* -------- Constructors -------- */

    public cNode_Building()
    {

    }

    public cNode_Building(string nodeName, Vector2 GPSPosition, List<cNode> connectedNodes, string buildingName, string buildingDescription, string buildingAbbreviation)
    {
        mNodeName = nodeName;
        mGeoPosition = GPSPosition;
        mConnectedNodes = connectedNodes;

        mBuildingName = nodeName;
        mBuildingDescription = buildingDescription;
        mBuildingAbbreviation = buildingAbbreviation;
    }

    /* -------- Public Methods -------- */

    /// <summary>
    /// Get the name of this building.
    /// </summary>
    public override string GetBuildingName()
    {
        return mBuildingName;
    }

    /// <summary>
    /// Get the description of this building.
    /// </summary>
    public override string GetBuildingDescription()
    {
        return mBuildingDescription;
    }

    /// <summary>
    /// Get the abbreviation of this building.
    /// </summary>
    public override string GetBuildingAbbreviation()
    {
		return mBuildingAbbreviation;
    }

}