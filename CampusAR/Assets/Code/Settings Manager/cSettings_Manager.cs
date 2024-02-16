using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class cSettings_Manager : MonoBehaviour
{

    [SerializeField] private TMP_Dropdown tmpDropdown;


    // Start is called before the first frame update
    void Start()
    {
        // Fetch the stored metric preference from PlayerPrefs
        string storedPreference = PlayerPrefs.GetString("MetricsPreference", "");

        // If a preference is found in PlayerPrefs, set the dropdown value
        if (!string.IsNullOrEmpty(storedPreference))
        {
            // Find the index of the stored preference in dropdown options
            int index = -1;
            for (int i = 0; i < tmpDropdown.options.Count; i++)
            {
                if (tmpDropdown.options[i].text == storedPreference)
                {
                    index = i;
                    break;
                }
            }

            // If the stored preference is found, set the dropdown value
            if (index != -1)
            {
                tmpDropdown.value = index;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Handle the go back button on Settings scene. Returns to Home scene
    /// </summary>
    public void HandleGoBackButton()
    {
        SceneManager.LoadScene("UserInterface");
    }

    /// <summary>
    /// Stores the selected dropdown value inside Unity's PlayerPrefs class. Will persist between sessions and scenes
    /// </summary>
    public void SaveMetricsPreference()
    {

        string selectionOption = tmpDropdown.options[tmpDropdown.value].text;

        PlayerPrefs.SetString("MetricsPreference", selectionOption);
        PlayerPrefs.Save();

        if (Application.isEditor)
        {
            Debug.Log("Selected option: " + selectionOption);
        }

        if (Application.isEditor)
        {
            string fetchedPreference = PlayerPrefs.GetString("MetricsPreference");
            Debug.Log("Fetched Metrics Preference: " + fetchedPreference);
        }
    }
}
