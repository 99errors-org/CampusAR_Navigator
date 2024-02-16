using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class cGPSMaths
{
    public const int kEarthRadiusMetres = 6371000;                      // The radius of the earth in metres.

    /// <summary>
    /// Get the distance between two GPS positions, this is using the Haversine Formula.
    /// </summary>
    /// <param name="_pos1">First GPS position, (Latitude, Longitude)</param>
    /// <param name="_pos2">Second GPS position, (Latitude, Longitude)</param>
    public static float GetDistance(Vector2 _pos1, Vector2 _pos2)
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

    public static float GetShortDistance(Vector2 pos1, Vector2 pos2)
    {
        // Earth's radius in meters
        float R = 6371000;

        // Convert latitudes and longitudes from degrees to radians
        float lat1 = pos1.x * Mathf.Deg2Rad;
        float lat2 = pos2.x * Mathf.Deg2Rad;
        float lon1 = pos1.y * Mathf.Deg2Rad;
        float lon2 = pos2.y * Mathf.Deg2Rad;

        // Convert lat/long to Cartesian coordinates
        float x1 = R * Mathf.Cos(lat1) * Mathf.Cos(lon1);
        float y1 = R * Mathf.Cos(lat1) * Mathf.Sin(lon1);

        float x2 = R * Mathf.Cos(lat2) * Mathf.Cos(lon2);
        float y2 = R * Mathf.Cos(lat2) * Mathf.Sin(lon2);

        // Calculate Euclidean distance
        float distance = Mathf.Sqrt(Mathf.Pow(x2 - x1, 2) + Mathf.Pow(y2 - y1, 2));

        return distance;
    }

    public static float GetUnityDistance(Vector3 _pos1, Vector3 _pos2)
    {
        return Vector3.Distance(_pos1, _pos2);
    }

    /// <summary>
    /// Gets the angle between two GPS positions.
    /// </summary>
    /// <param name="_pos1">First GPS position, (Latitude, Longitude)</param>
    /// <param name="_pos2">Second GPS position, (Latitude, Longitude)</param>
    /// <returns></returns>
    public static float GetAngle(Vector2 _pos1, Vector2 _pos2)
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
    public static Vector3 GetVector(Vector2 _pos1, Vector2 _pos2)
    {
        // Get the Distance.
        float _distance = GetDistance(_pos1, _pos2);

        // Get angle.
        float _angle = GetAngle(_pos1, _pos2);

        // Get the Vector.
        Vector3 _vector = new Vector3(_distance * Mathf.Cos(_angle * Mathf.Deg2Rad), 0.0f, _distance * Mathf.Sin(_angle * Mathf.Deg2Rad));

        return _vector;
    }

}
