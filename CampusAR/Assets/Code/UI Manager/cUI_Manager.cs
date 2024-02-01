using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class cUI_Manager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI rBuildingNameField;
    private cNode currentBuildingNode; // Reference to the current cNode_Building instance

    [SerializeField] private Canvas bottomDrawerCanvas; // Reference to bottom drawer component


    // Start is called before the first frame update
    void Start()
    {
        rBuildingNameField.text = currentBuildingNode.GetBuildingName();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBuildingNameField();
    }

    void UpdateBuildingNameField()
    {
        // Check if the currentBuildingNode is not null
        if (currentBuildingNode != null)
        {
            // Update the text content of the TextMeshPro field with the building name
            rBuildingNameField.text = "Building Name: " + currentBuildingNode.GetBuildingName();
        }
        else
        {
            // Handle the case when there is no currentBuildingNode selected
            rBuildingNameField.text = "No building selected";
        }
    }
    public void HandleFloatingActionButton()
    {
        Debug.Log("You have clicked the floating button!");
    }
    public void HandleSelectBuildingButton()
    {
        Debug.Log("You have clicked the SelectBuilding button!");
        StartCoroutine(AnimateCanvasPosition());
    }
    public void HandleSelectTourButton()
    {
        Debug.Log("You have clicked the SelectTour button!");
    }
    public void HandleCreateTourButton()
    {
        Debug.Log("You have clicked the CreateTour button!");
    }

    public void HandleSettingsButton()
    {
        Debug.Log("You have clicked the Settings button!");
        SceneManager.LoadScene("SettingsScene");
    }

    IEnumerator AnimateCanvasPosition()
    {
        Vector3 startPosition = bottomDrawerCanvas.transform.position;
        Vector3 targetPosition = new Vector3(startPosition.x, 100, startPosition.z);
        float duration = 1.0f;
        float startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            float timeElapsed = Time.time - startTime;
            float t = Mathf.Clamp01(timeElapsed / duration);
            float curveValue = CustomBezier(t, 0.32f, 0.72f, 0.0f, 1.0f);
            bottomDrawerCanvas.transform.position = Vector3.Lerp(startPosition, targetPosition, curveValue);
            yield return null;
        }
    }

    IEnumerator AnimateCanvasPositionBack()
    {
        float duration = 1.0f;
        float startTime = Time.time;
        Vector3 targetPosition = new Vector3(540, -1074, 0);

        while (Time.time - startTime < duration)
        {
            float timeElapsed = Time.time - startTime;
            float t = Mathf.Clamp01(timeElapsed / duration);
            float curveValue = CustomBezier(t, 0.08f, 0.82f, 0.17f, 1.0f);
            bottomDrawerCanvas.transform.position = Vector3.Lerp(bottomDrawerCanvas.transform.position, targetPosition, curveValue * Time.deltaTime);
            yield return null;
        }
    }

    float CustomBezier(float t, float a, float b, float c, float d)
    {
        float s = 1.0f - t;
        float ab = Mathf.Lerp(a, b, t);
        float bc = Mathf.Lerp(b, c, t);
        float cd = Mathf.Lerp(c, d, t);
        float abc = Mathf.Lerp(ab, bc, t);
        float bcd = Mathf.Lerp(bc, cd, t);
        return Mathf.Lerp(abc, bcd, t);
    }
}