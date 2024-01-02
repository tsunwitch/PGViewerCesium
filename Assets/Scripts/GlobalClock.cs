using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlobalClock : MonoBehaviour
{
    public GameObject uiClock;
    public int simulationSpeed;
    public bool isSimulationPlaying;
    public GameObject UITimeline;
    public GameObject trackController;
    public GameObject simSpeedText;
    private bool isSliderBeingDragged = false;
    private DateTime startTime, endTime, currentTime;
    private int simSpeedIndex = 0;
    private int[] simSpeedPool = {1, 2, 4, 10, 32};

    private void Start()
    {
        simulationSpeed = simSpeedPool[0];
        simSpeedText.GetComponent<TextMeshProUGUI>().SetText(simulationSpeed.ToString() + "x");
    }

    public void setTimeframe(DateTime startDateTime, DateTime endDateTime)
    {

        if (startTime.TimeOfDay == TimeSpan.Zero || startDateTime.TimeOfDay < startTime.TimeOfDay)
        {
            startTime = startDateTime;
            if(trackController.transform.childCount == 0) currentTime = startTime;
        }

        if (endDateTime.TimeOfDay > endTime.TimeOfDay)
        {
            endTime = endDateTime;
        }

        if (startTime.TimeOfDay == TimeSpan.Zero)
        {
            currentTime = startTime;
        }

        Slider uiSlider = UITimeline.GetComponent<Slider>();
        uiSlider.minValue = (float)startTime.TimeOfDay.TotalSeconds;
        uiSlider.maxValue = (float)endTime.TimeOfDay.TotalSeconds;

        Debug.Log("Timeframe: " + startTime + " To " + endTime);
    }

    public void resetTimeFrame()
    {
        List<GameObject> trackInstances = GameObject.FindGameObjectsWithTag("TrackInstance").ToList();

        //Set the timeframe to 0 before resetting according to existing tracks
        startTime = endTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, 0, 0, 0);

        foreach(GameObject trackInstance in trackInstances)
        {
            PilotMovementHandler movementHandler = trackInstance.GetComponent<PilotMovementHandler>();
            setTimeframe(movementHandler.trackTimeframe[0], movementHandler.trackTimeframe[1]);
        }
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

    void UpdateSliderWithCurrentValue()
    {
        Slider uiSlider = UITimeline.GetComponent<Slider>();
        uiSlider.value = (float)currentTime.TimeOfDay.TotalSeconds;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Set the slider to match current time
        if (!isSliderBeingDragged) UpdateSliderWithCurrentValue();

        //Set the UI clock
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

    public void JumpToTimestamp()
    {
        float sliderValue = UITimeline.GetComponent<Slider>().value;
        TimeSpan targetValue = TimeSpan.FromSeconds(sliderValue);
        GameObject[] trackInstances = GameObject.FindGameObjectsWithTag("TrackInstance");

        //Set waypoint for pilot instances
        foreach (GameObject trackInstance in trackInstances)
        {
            PilotMovementHandler movementHandler = trackInstance.GetComponent<PilotMovementHandler>();
            movementHandler.SetCurrentWaypoint(targetValue);
            movementHandler.RedrawLinePositions();
        }

        //Set currentTime to selected
        currentTime = currentTime.Date + targetValue;
    }

    public void increaseSimulationSpeed()
    {
        if (simSpeedIndex < simSpeedPool.Length)
            simSpeedIndex += 1;
        simulationSpeed = simSpeedPool[simSpeedIndex];
        simSpeedText.GetComponent<TextMeshProUGUI>().SetText(simulationSpeed.ToString() + "x");
    }

    public void decreaseSimulationSpeed()
    {
        if (simSpeedIndex > 0)
            simSpeedIndex -= 1;
        simulationSpeed = simSpeedPool[simSpeedIndex];
        simSpeedText.GetComponent<TextMeshProUGUI>().SetText(simulationSpeed.ToString() + "x");
    }

    public void setSliderBeingDraggedTrue()
    {
        isSliderBeingDragged = true;
    }

    public void setSliderBeingDraggedFalse()
    {
        isSliderBeingDragged = false;
    }
}
