using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cDistanceUnitUtility : MonoBehaviour
{

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
