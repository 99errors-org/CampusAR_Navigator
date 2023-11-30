using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cArrowManager : MonoBehaviour
{
    [SerializeField]
    private GameObject mArrowPrefab;

    // Stores the Arrow object not the prefab
    private GameObject mArrow;

    [SerializeField]
    private GameObject mCam;

    private const float kDistanceInfrontOfUser = 7.12f;
    // position the arrow a bit to the ground
    private const float kArrowYPosition = -1.04f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Checks if the arrow is instantiated
        if (mArrow != null && mArrow.activeSelf)
        {
            RotateArrow(10.0f*Time.deltaTime);
        }
        else if (mArrow == null)
        {

            mArrow = Instantiate(mArrowPrefab);
        }
    }

    void FixedUpdate()
    {
        // Keeps the arrow in front of the camera
        if (mArrow != null && mArrow.activeSelf)
        {
            mArrow.transform.position = mCam.transform.rotation * new Vector3(mCam.transform.position.x,
                                                                              mCam.transform.position.y + kArrowYPosition, 
                                                                              mCam.transform.position.z + kDistanceInfrontOfUser);
        }
    }

    // Sets the arrow to a rotated position
    public void RotateArrow(float rotationAngle)
    {
        mArrow.transform.Rotate(new Vector3(0.0f, 0.0f, rotationAngle));
    }
}
