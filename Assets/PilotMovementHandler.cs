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
    private GameObject pilotInstance;
    private int currentFixIndex = 0;
    private GlobalClock clock;
    float timestampMargin = 0.5f;

    private void Start()
    {
        clock = GameObject.Find("GlobalClock").GetComponent<GlobalClock>();
        pilotInstance = Instantiate(pilotPrefab, transform);
        pilotInstance.GetComponent<CesiumGlobeAnchor>().longitudeLatitudeHeight = fixes.FirstOrDefault().GetComponent<CesiumGlobeAnchor>().longitudeLatitudeHeight;
    }

    void Update()
    {
        if (clock.isSimulationPlaying)
        {
            MoveToNextWaypoint();
        }
    }

    void MoveToNextWaypoint()
    {
        if (currentFixIndex < fixes.Count)
        {

            // Get the current waypoint
            GameObject currentWaypoint = fixes[currentFixIndex];

            // Move the pilotInstance to the waypoint's position
            pilotInstance.transform.position = currentWaypoint.transform.position;

            // Check if the timestamp is reached in the global clock
            FixData fixData = currentWaypoint.GetComponent<FixData>();
            if (fixData != null && (fixData.timestamp.TotalSeconds - clock.getCurrentTime().TotalSeconds) <= 0.1d)
            {
                Debug.Log("Moving");

                // Move to the next waypoint
                currentFixIndex++;
            }
        }
        else
        {
            // Restart on end (for debugging purposes - to remove finally)
            currentFixIndex = 0;
        }
    }
}
