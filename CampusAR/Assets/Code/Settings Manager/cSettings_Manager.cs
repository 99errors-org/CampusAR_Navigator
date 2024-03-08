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
        cUser_Manager.mInstance.LoadUserDistancePreference();
        // Set the dropdown value based on the loaded preference
        tmpDropdown.value = (int)cUser_Manager.mInstance.GetUserDistancePreference();
    }

    // Handle dropdown value change
    public void OnDropdownValueChanged()
    {
        Debug.Log($"Dropdown value changed to: {tmpDropdown.value}");
        // Set index of dropdown item into player prefs
        cUser_Manager.mInstance.SaveUserDistancePreference((cUser_Manager.kDistanceUnit)tmpDropdown.value);

        // Save the user's distance preference
        PlayerPrefs.Save();

    }

    /// <summary>
    /// Handle the go back button on Settings scene. Returns to Home scene
    /// </summary>
    public void HandleGoBackButton()
    {
        SceneManager.LoadScene(0);
    }
}
