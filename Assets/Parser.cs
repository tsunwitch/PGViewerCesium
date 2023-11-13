using CesiumForUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

public class Fix
{
    public double3 coordinates;
    public DateTime timestamp;

    public Fix(double lat, double lon, int alt, DateTime time)
    {
        coordinates = new double3(lon, lat, alt);
        timestamp = time;
    }

    public string getFixData()
    {
        return "Latitude: " + coordinates.y + " Longitude: " + coordinates.x + " Altitude: " + coordinates.z + " Timestamp: " + timestamp;
    }
}

public class Parser : MonoBehaviour
{
    public List<Fix> fixes = new List<Fix>();
    public FixSpawner fixSpawner;

    [SerializeField]
    private UnityEngine.Object _fileToParse;
    public UnityEngine.Object fileToParse
    {
        get
        {
            return _fileToParse;
        }
        set
        {
            _fileToParse = value;

            if (fileToParse != null)
            {
                List<Fix> fixes = parseIGC(fileToParse);
                Debug.Log("Parsed IGC file");
                Fix startingFix = fixes.FirstOrDefault();
                //GameObject.Find("CesiumGeoreference").GetComponent<CesiumGeoreference>().SetOriginLongitudeLatitudeHeight(startingFix.coordinates.x,startingFix.coordinates.y,startingFix.coordinates.z);
                //Debug.Log("Set Origin according to track");
                fixSpawner.SpawnFixes(fixes);
            }

        }
    }

    [ContextMenu("Load IGC")]
    void loadIGC()
    {
        if (_fileToParse == null) { Debug.Log("Load an IGC file to get started!"); }
        fileToParse = _fileToParse;
    }

    //Old OnValidate()
    //private void OnValidate()
    //{
    //    if (_fileToParse == null) { Debug.Log("Load an IGC file to get started!"); }
    //    fileToParse = _fileToParse;
    //}

    private List<Fix> parseIGC(UnityEngine.Object fileToParse)
    {
        List<Fix> fixes = new List<Fix>();
        var fullpath = AssetDatabase.GetAssetPath(fileToParse);

        using (StreamReader sr = new StreamReader(fullpath))
        {
            string line;
            Fix previousFix = null;

            while ((line = sr.ReadLine()) != null)
            {
                if (line.StartsWith("B"))
                {
                    //Parse latitude and longitude using functions
                    double fixLatitude = parseLatitude(line.Substring(7, 8));
                    double fixLongitude = parseLongitude(line.Substring(15, 9));
                    int fixAltitude;
                    char altitudeMarker = line[24];
                    if(altitudeMarker == 'A')
                    {
                        fixAltitude = int.Parse(line.Substring(30, 5));
                    } 
                    else
                    {
                        fixAltitude = int.Parse(line.Substring(25, 5));
                    }
                    DateTime fixTimestamp = DateTime.Parse(line.Substring(1, 6).Insert(2, ":").Insert(5, ":"));
                    

                    Fix fixToAdd = new Fix(fixLatitude, fixLongitude, fixAltitude, fixTimestamp);

                    if (fixToAdd != previousFix)
                    {
                        fixes.Add(fixToAdd);
                        previousFix = fixToAdd;
                    }
                }
            }

            Debug.Log(fixes.FirstOrDefault().getFixData());
        }

        if (fixes == null)
        {
            Debug.Log("No Fixes to load");
        }

        return fixes;
    }

    private double parseLatitude(string latitude)
    {
        double result = 0.0f;

        double degrees = double.Parse(latitude.Substring(0, 2));
        double minutes = double.Parse(latitude.Substring(2, latitude.Length - 3));
        double parsedMinutes = (double)(minutes * 0.001) / 60;
        result = degrees + parsedMinutes;

        if (latitude[latitude.Length - 1] == 'N')
        {
            return result;
        }
        else
        {
            return -result;
        }
    }

    private double parseLongitude(string longitude)
    {
        double result = 0.0f;

        double degrees = double.Parse(longitude.Substring(0, 3));
        double minutes = double.Parse(longitude.Substring(3, longitude.Length - 4));
        double parsedMinutes = (double)(minutes * 0.001) / 60;
        result = degrees + parsedMinutes;

        if (longitude[longitude.Length - 1] == 'E')
        {
            return result;
        }
        else
        {
            return -result;
        }
    }
}
