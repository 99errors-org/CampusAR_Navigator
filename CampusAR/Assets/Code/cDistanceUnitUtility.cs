using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cDistanceUnitUtility : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Gets the preferred distance unit based on user settings.
    /// </summary>
    /// <returns>The preferred distance unit ("m" for kilometers or "mi" for miles).</returns>
    public static string GetDistanceUnit()
    {
        string storedPreference = PlayerPrefs.GetString("MetricsPreference", "");
        return storedPreference == "Kilometres (km)" ? "m" : storedPreference == "Miles (mi)" ? "mi" : "";
    }
}
