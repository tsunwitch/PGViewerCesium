using CesiumForUnity;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class TrackActionHandler : MonoBehaviour
{
    public ActiveCameraController cameraController;
    public GameObject originShifter;
    public GlobalClock clock;

    private void Awake()
    {
        cameraController = FindObjectOfType<ActiveCameraController>();
        originShifter = GameObject.Find("OriginShifter");
        clock = GameObject.FindGameObjectWithTag("GlobalClock").GetComponent<GlobalClock>();
    }

    [ContextMenu("Focus Track")]
    public void focusTrack()
    {
        GameObject pilot = transform.GetChild(transform.childCount - 1).gameObject;

        //set origin to current pilot AND NOT MOVE IT
        originShifter.GetComponent<CesiumGlobeAnchor>().longitudeLatitudeHeight = pilot.GetComponent<CesiumGlobeAnchor>().longitudeLatitudeHeight;

        cameraController.UpdateCameraTarget(pilot);

        //redraw all line renderers
        GameObject[] trackInstances = GameObject.FindGameObjectsWithTag("TrackInstance");

        foreach (GameObject trackInstance in trackInstances)
        {
            PilotMovementHandler movementHandler = trackInstance.GetComponent<PilotMovementHandler>();

            LineRenderer lineRenderer = movementHandler.GetComponent<LineRenderer>();

            //movementHandler.AwaitRedrawLinePositions();
            StartCoroutine(movementHandler.AwaitRedrawLinePositions());
        }
    }

    [ContextMenu("Destroy Track")]
    public void destroyTrack()
    {
        //Move originShifter out of track
        //originShifter.transform.parent = transform.parent;

        //Destroy track container with its contents
        DestroyImmediate(gameObject);

        //Set new timeframe with globalClock
        clock.resetTimeFrame();
    }
}
