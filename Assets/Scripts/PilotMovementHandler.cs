using CesiumForUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PilotMovementHandler : MonoBehaviour
{
    public List<GameObject> fixes;
    public GameObject pilotPrefab;
    public GameObject currentWaypoint;
    private GameObject pilotInstance;
    public int currentFixIndex;
    private GlobalClock clock;
    public DateTime[] trackTimeframe = new DateTime[2];
    double timestampMargin = 0.1d;
    private float timer = 0f;
    private Color[] colorPool = { Color.red, Color.green, Color.blue, Color.yellow, Color.cyan, Color.magenta };
    private LineRenderer lineRenderer;
    private List<Vector3> linePositions = new List<Vector3>();

    private void Start()
    {
        clock = GameObject.Find("GlobalClock").GetComponent<GlobalClock>();
        System.Random rnd = new System.Random();
        Color randomColor = colorPool[rnd.Next() % colorPool.Length];

        //Instantiate the pilot at the currentWaypoint
        pilotInstance = Instantiate(pilotPrefab, transform);
        pilotInstance.GetComponent<CesiumGlobeAnchor>().longitudeLatitudeHeight = fixes[currentFixIndex].GetComponent<CesiumGlobeAnchor>().longitudeLatitudeHeight;
        Debug.Log("Instantiated " + pilotInstance.name);

        //Set current waypoint on start, so the pilot starts at currentTime
        SetCurrentWaypoint(clock.getCurrentTime());

        //Set the line renderer
        lineRenderer = pilotInstance.GetComponent<LineRenderer>();

        //Parent the OriginShifter to pilotInstance
        GameObject.Find("OriginShifter").transform.parent = pilotInstance.transform;

        //Set up LineRenderer
        SetLineRenderer(randomColor);

        // Initialize the linePositions list with the initial position
        linePositions.Add(pilotInstance.transform.position);
    }

    void Update()
    {
        if (clock.isSimulationPlaying)
        {
            MoveToNextWaypoint();
            UpdateLinePositions();
        }
    }

    void MoveToNextWaypoint()
    {
        if (currentFixIndex < fixes.Count)
        {
            // Get the current waypoint
            currentWaypoint = fixes[currentFixIndex];
            FixData fixData = currentWaypoint.GetComponent<FixData>();

            if (currentFixIndex > 0)
            {
                //Get the previous waypoint
                GameObject previousWaypoint = fixes[currentFixIndex - 1];
                FixData previousFixData = previousWaypoint.GetComponent<FixData>();

                // Calculate the total time needed to reach the current waypoint from the previous waypoint
                float totalTime = (float)((fixData.timestamp.TotalSeconds - previousFixData.timestamp.TotalSeconds) / clock.simulationSpeed);

                // Update the timer based on the total time
                timer += Mathf.Clamp01(Time.deltaTime / totalTime);

                Vector3 interpolatedPosition = Vector3.Lerp(previousWaypoint.transform.position, currentWaypoint.transform.position, timer);

                // Smoothly rotate the pilotInstance towards the movement direction on the y-axis
                Vector3 directionToInterpolated = interpolatedPosition - pilotInstance.transform.position;
                directionToInterpolated.y = 0f; // Set the y-component to zero to keep rotation on the y-axis only

                Quaternion targetRotation = Quaternion.LookRotation(directionToInterpolated);

                float rotationSpeed = 1.5f * clock.simulationSpeed; // Adjust the rotation speed as needed
                pilotInstance.transform.rotation = Quaternion.Lerp(pilotInstance.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);


                // Move the pilotInstance to the waypoint's position
                pilotInstance.transform.position = interpolatedPosition;
            }

            // Check if the timestamp is reached in the global clock
            if (fixData != null && (fixData.timestamp.TotalSeconds - clock.getCurrentTime().TotalSeconds) <= timestampMargin)
            {
                // Move to the next waypoint
                currentFixIndex++;
                timer = 0f;
            }
        }
        //else
        //{
        //    // Restart on end (for debugging purposes - to remove finally)
        //    currentFixIndex = 0;
        //}
    }

    public void SetCurrentWaypoint(TimeSpan selectedTime)
    {
        //Use LINQ to find the closest fix
        currentFixIndex = fixes.IndexOf(fixes.OrderBy(fix => Math.Abs(selectedTime.TotalSeconds - fix.GetComponent<FixData>().timestamp.TotalSeconds)).First());


        if (currentFixIndex != -1)
        {
            //Move pilotInstance to found fix
            pilotInstance.GetComponent<CesiumGlobeAnchor>().longitudeLatitudeHeight = fixes[currentFixIndex].GetComponent<CesiumGlobeAnchor>().longitudeLatitudeHeight;
        }
        else
        {
            Debug.Log("No fix found for the given timestamp.");
        }
    }

    public void SetLineRenderer(Color color)
    {
        lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        lineRenderer.startWidth = 1f;
        lineRenderer.endWidth = 1f;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        // Set the LineRenderer to use the object's transform rotation
        lineRenderer.useWorldSpace = true;
    }

    void UpdateLinePositions()
    {
        if (lineRenderer != null)
        {
            // Add the current position to the list
            linePositions.Add(pilotInstance.transform.position);

            // Trim the list if it's too long
            while (linePositions.Count > 100) // Adjust the maximum number of vertices as needed
            {
                linePositions.RemoveAt(0);
            }

            // Set positions in the LineRenderer
            lineRenderer.positionCount = linePositions.Count;
            lineRenderer.SetPositions(linePositions.ToArray());
        }
    }
}
