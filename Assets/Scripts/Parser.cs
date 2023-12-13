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
    private UnityEngine.Object fileToParse;
    private GlobalClock clock;

    private void Start()
    {
        clock = GameObject.Find("GlobalClock").GetComponent<GlobalClock>();
    }

    [ContextMenu("Load IGC")]
    public void loadIGC(string fileToParse)
    {

        //OLD - works with unity editor
        //if (fileToParse != null)
        //{
        //    List<Fix> fixes = parseIGC(fileToParse);
        //    Debug.Log("Parsed IGC file");
        //    fixSpawner.SpawnFixes(fixes);
        //}

        //NEW - works with native file explorer
        if(fileToParse != null)
        {
            List<Fix> fixes = parseIGC(fileToParse);
            fixSpawner.SpawnFixes(fixes);
        }
    }

    private List<Fix> parseIGC(string fileToParse)
    {
        List<Fix> fixes = new List<Fix>();
        //var fullpath = AssetDatabase.GetAssetPath(fileToParse);
        string fullpath = fileToParse;

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
            clock.setTimeframe(fixes.FirstOrDefault().timestamp, fixes.LastOrDefault().timestamp);

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
