using System.Collections;
using System.Collections.Generic;
using TetraCreations.Attributes;
using UnityEngine;

public enum NodeType
{
	Path		= 0,
	Building	= 1,
}

[CreateAssetMenu(fileName = "Path Node", menuName = "Nodes/Path Node")]
public class cNode : ScriptableObject
{
	/* -------- Variables -------- */
	[Title("Node Information")]
	[SerializeField] protected string		mNodeName;						// The name of this node.
	[SerializeField] protected Vector2		mGeoPosition;                   // The GPS location of the Node, X = Latitude, Y = Longitude.
	[SerializeField] protected List<cNode>	mConnectedNodes;                // A list of all the connected nodes that the user can get to from this node.

	[SerializeField] protected NodeType		mNodeType = NodeType.Path;		// The type of node this object is.

	/* -------- Public Methods -------- */

	/// <summary>
	/// Get the type of node.
	/// </summary>
	/// <returns>Returns the currently assigned type (NodeType)</returns>
	public virtual NodeType GetNodeType()
	{
		return mNodeType;
	}

	/// <summary>
	/// Get the position of this node in Latitude and Longitude.
	/// </summary>
	/// <returns>Vector2(Lat, Long) </returns>
	public Vector2 GetGPSLocation()
	{
		return mGeoPosition;
	}
}

[CreateAssetMenu(fileName = "Building Node", menuName = "Nodes/Building Node")]
public class cNode_Building : cNode
{
	/* -------- Variables -------- */
	[Title("Building Specific")]
	[SerializeField] private string			mBuildingName;					// The name of the building at this specific node.
	[SerializeField] private string			mBuildingDescription;           // The description of this nodes building.	

    /* -------- Public Methods -------- */
}