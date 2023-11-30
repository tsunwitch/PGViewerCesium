using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GlobalClock : MonoBehaviour
{
    public GameObject uiClock;
    public int simulationSpeed = 5;
    public bool isSimulationPlaying;
    private DateTime startTime, endTime, currentTime;

    public void setTimeframe(DateTime startDateTime, DateTime endDateTime)
    {

        if (startTime.TimeOfDay == TimeSpan.Zero || startDateTime.TimeOfDay < startTime.TimeOfDay)
        {
            startTime = startDateTime;
            currentTime = startTime;
        }

        if (endDateTime.TimeOfDay > endTime.TimeOfDay)
        {
            endTime = endDateTime;
        }

        if (startTime.TimeOfDay == TimeSpan.Zero)
        {
            currentTime = startTime;
        }

        Debug.Log("Timeframe: " + startTime + " To " + endTime);
    }

    [ContextMenu("Start Simulation")]
    public void setisSimulationPlaying()
    {
        isSimulationPlaying = !isSimulationPlaying;
        Debug.Log("Simulation: " + isSimulationPlaying);
    }

    public TimeSpan getCurrentTime()
    {
        return currentTime.TimeOfDay;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        uiClock.GetComponent<TextMeshProUGUI>().SetText(currentTime.ToString("HH:mm:ss"));

        if (currentTime < endTime && isSimulationPlaying)
        {
            float deltaTimeInSeconds = Time.deltaTime;
            currentTime = currentTime.AddSeconds(deltaTimeInSeconds * simulationSpeed);
        }
        else
        {
            isSimulationPlaying = false;
        }
    }
}
