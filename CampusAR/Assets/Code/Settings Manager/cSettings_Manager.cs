using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class cSettings_Manager : MonoBehaviour
{

    [SerializeField] private TMPro.TMP_Dropdown tmpDropdown;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HandleGoBackButton()
    {
        SceneManager.LoadScene("UserInterface");
    }

    public void SaveMetricsPreference()
    {
        Debug.Log(tmpDropdown.options[tmpDropdown.value].text);
    }
}
